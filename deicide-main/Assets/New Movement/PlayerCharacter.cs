using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;

public struct CharacterInput
{
    public Quaternion Rotation;
    public Vector2 Move;
    public bool Jump;
    public bool Dash;
    public bool Fire;
    public bool Abil2;
    public bool Abil3;
    public bool Abil4;
}

public class PlayerCharacter : MonoBehaviour, ICharacterController
{
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform cameraTarget;

    [Header("🔫 Combat Targets")]
    [SerializeField] private Transform firePoint;              
    [SerializeField] private Camera characterCamera;          
    public Transform FirePoint => firePoint;                   
    public Camera Camera => characterCamera;                  

    private AbilitySet abilitySet;

    private float walkSpeed;
    private float walkResponse;
    private float airSpeed;
    private float airAcceleration;

    private float jumpSpeed;
    private int airJumpCount;
    private float coyoteTime = 0.15f;
    private float gravity = -90f;
    private float dashSpeed;

    private Quaternion _requestedRotation;
    private Vector3 _requestedMovement;
    private bool _requestedFire;
    private bool _requestedJump;
    private bool _requestedDash;
    private bool _requestedAbil2;
    private bool _requestedAbil3;
    private bool _requestedAbil4;

    private float _timeSinceUngrounded;
    private float _timeSinceJumpRequest;
    private int _remainingJumps;
    private bool _ungroundedDueToJump;
    private bool _isDashing;
    private float _dashTimer;
    private float _dashCooldown;
    private Vector3 _dashDirection;

    private Vector3 _externalVelocity;
    private float _externalVelocityTimer = 0f;
    private bool _killMomentum = false;

    public void Initialize(AbilitySet abilitySet, CharacterDefinition def)
    {
        motor.CharacterController = this;
        this.abilitySet = abilitySet;

        walkSpeed = def.walkSpeed;
        walkResponse = walkSpeed * 1.25f;
        airSpeed = def.airSpeed;
        airAcceleration = def.airAcceleration;
        jumpSpeed = def.jumpSpeed;
        airJumpCount = def.airJumpCount;
        dashSpeed = def.dashSpeed;
    }

    public void UpdateInput(CharacterInput input)
    {
        _requestedRotation = input.Rotation;

        _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y);
        _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f);
        _requestedMovement = input.Rotation * _requestedMovement;

        if (!_isDashing)
        {
            _dashDirection = new Vector3(input.Move.x, 0f, input.Move.y);
            _dashDirection = Vector3.ClampMagnitude(_dashDirection, 1f);
            _dashDirection = input.Rotation * _dashDirection;
        }

        var wasRequestingJump = _requestedJump;
        _requestedJump = _requestedJump || input.Jump;
        if (_requestedJump && !wasRequestingJump)
            _timeSinceJumpRequest = 0f;

        _requestedDash = input.Dash;

        if (_requestedDash && !_isDashing && _dashDirection != Vector3.zero && _dashCooldown <= 0f)
        {
            _requestedDash = false;
            _isDashing = true;
            _dashTimer = 0f;
            _dashCooldown = 0.3f;
        }

        _requestedFire = input.Fire;
        _requestedAbil2 = input.Abil2;
        _requestedAbil3 = input.Abil3;
        _requestedAbil4 = input.Abil4;

        if (_requestedFire)
            abilitySet.TryFirePrimary();
        // if (_requestedAbil2)
        //     abilitySet.TryActivateSecondary();
        // if (_requestedAbil3)
        //     abilitySet.TryActivateMobility();
        // if (_requestedAbil4)
        //     abilitySet.TryActivateMisc();
    }

    public void UpdateBody(float deltaTime) { }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (_externalVelocityTimer > 0f)
        {
            _externalVelocityTimer -= deltaTime;
            currentVelocity = _externalVelocity;
            if (_externalVelocityTimer <= 0f)
            {
                _externalVelocity = Vector3.zero;
                currentVelocity = _killMomentum ? Vector3.zero : currentVelocity;
                _killMomentum = false;
            }
            return;
        }

        if (_isDashing)
        {
            _dashTimer += deltaTime;
            float dashDuration = 0.2f;
            float slowdown = Mathf.Exp(-10f * _dashTimer);
            Vector3 dashVelocity = _dashDirection * dashSpeed * Mathf.Max(slowdown, 1.1f);
            dashVelocity = Vector3.ProjectOnPlane(dashVelocity, motor.CharacterUp);
            motor.ForceUnground(0.2f);
            currentVelocity = dashVelocity;

            if (_dashTimer >= dashDuration || dashVelocity.magnitude < 1f)
                _isDashing = false;

            return;
        }
        else if (_dashCooldown > 0f)
            _dashCooldown -= deltaTime;

        if (motor.GroundingStatus.IsStableOnGround)
        {
            _timeSinceUngrounded = 0f;
            _ungroundedDueToJump = false;
            _remainingJumps = airJumpCount;

            var groundedMovement = motor.GetDirectionTangentToSurface(
                _requestedMovement,
                motor.GroundingStatus.GroundNormal
            ) * _requestedMovement.magnitude;

            var targetVelocity = groundedMovement * walkSpeed;
            currentVelocity = Vector3.Lerp(
                currentVelocity,
                targetVelocity,
                1f - Mathf.Exp(-walkResponse * deltaTime)
            );
        }
        else
        {
            _timeSinceUngrounded += deltaTime;

            if (_requestedMovement.sqrMagnitude > 0f)
            {
                var planarMovement = Vector3.ProjectOnPlane(_requestedMovement, motor.CharacterUp) * _requestedMovement.magnitude;
                var currentPlanarVelocity = Vector3.ProjectOnPlane(currentVelocity, motor.CharacterUp);
                var movementForce = planarMovement * airAcceleration * deltaTime;
                var targetPlanarVelocity = Vector3.ClampMagnitude(currentPlanarVelocity + movementForce, airSpeed);

                currentVelocity += targetPlanarVelocity - currentPlanarVelocity;

                if (motor.GroundingStatus.FoundAnyGround)
                {
                    if (Vector3.Dot(movementForce, currentVelocity + movementForce) > 0f)
                    {
                        var obstructionNormal = Vector3.Cross(
                            motor.CharacterUp,
                            Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal)
                        ).normalized;

                        movementForce = Vector3.ProjectOnPlane(movementForce, obstructionNormal);
                    }
                }
            }
            else
            {
                Vector3 verticalVelocity = Vector3.Project(currentVelocity, motor.CharacterUp);
                currentVelocity = Vector3.Lerp(currentVelocity, verticalVelocity, 5f * deltaTime);
            }

            currentVelocity += motor.CharacterUp * gravity * deltaTime;
        }

        if (_requestedJump)
        {
            var grounded = motor.GroundingStatus.IsStableOnGround;
            var canCoyoteJump = _timeSinceUngrounded < coyoteTime && !_ungroundedDueToJump;

            if (grounded || canCoyoteJump || _remainingJumps > 0)
            {
                _requestedJump = false;
                motor.ForceUnground(0.1f);
                _ungroundedDueToJump = true;

                var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpSpeed);
                currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);

                if (!(grounded || canCoyoteJump)) _remainingJumps--;
            }
            else
            {
                _timeSinceJumpRequest += deltaTime;
                _requestedJump = _timeSinceJumpRequest < coyoteTime;
            }
        }
    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        var forward = Vector3.ProjectOnPlane(
            _requestedRotation * Vector3.forward,
            motor.CharacterUp
        );
        currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
    }

    public void InjectExternalVelocity(Vector3 velocity, float duration, bool killMomentum)
    {
        _externalVelocity = velocity;
        _externalVelocityTimer = duration;
        _killMomentum = killMomentum;
    }

    public Transform GetCameraTarget() => cameraTarget;
    public KinematicCharacterMotor GetMotor() => motor;

    #region Interface Methods
    public void BeforeCharacterUpdate(float deltaTime) { }
    public void PostGroundingUpdate(float deltaTime) { }
    public void AfterCharacterUpdate(float deltaTime) { }
    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
    public bool IsColliderValidForCollisions(Collider coll) => true;
    public void OnDiscreteCollisionDetected(Collider hitCollider) { }
    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }
    #endregion
}
