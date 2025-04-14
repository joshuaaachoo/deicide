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
    [SerializeField] private KinematicCharacterMotor motor; // prefab this later
    [SerializeField] private Transform cameraTarget;
    private AbilitySet abilitySet;
    /* DEFAULT VALUES
     * walkSpeed = 20f
     * airSpeed = 15f
     * airAcceleration = 70f
     * jumpSpeed = 27f
     * airJumpCount = 1
     */
    private float walkSpeed;
    private float walkResponse; // acceleration kind of?
    private float airSpeed;
    private float airAcceleration;
    
    private float jumpSpeed; // jump speed which is technically jump height but with more math
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

        _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y); // turns 2d input vector into 3d movement vector on xz plane
        _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f); // normalizes vector
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

        if (_requestedDash && !_isDashing && _dashDirection != Vector3.zero && _dashCooldown <= 0f) // if you req dash + not standing still + not already dashing
        {
            _requestedDash = false;
            _isDashing = true;
            _dashTimer = 0f;

            _dashCooldown = 0.3f;
        }

        _requestedFire = input.Fire;

        _requestedAbil2 = input.Abil2;
        _requestedAbil3 = input.Abil3;
        _requestedAbil4 = input.Abil4; // these might be unnecessary?

        if (_requestedFire)
        {
            abilitySet.TryFirePrimary();
        }
        if (_requestedAbil2)
        {
            abilitySet.TryActivateSecondary();
            //Debug.Log("Trying to activate secondary");
        }
        if (_requestedAbil3)
        {
            abilitySet.TryActivateMobility();
            //Debug.Log("Trying to activate mobility");
        }
        if (_requestedAbil4)
        {
            abilitySet.TryActivateMisc();
            //Debug.Log("Trying to activate misc");
        }
    }

    public void UpdateBody(float deltaTime)
    {
        
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if(_externalVelocityTimer > 0f)
        {
            _externalVelocityTimer -= deltaTime;
            currentVelocity = _externalVelocity;

            if(_externalVelocityTimer <= 0f)
            {
                _externalVelocity = Vector3.zero;
                currentVelocity = _killMomentum ? Vector3.zero : currentVelocity;
                _killMomentum = false;
            }
            return;
        }

        // dashing
        if (_isDashing)
        {
            // dash active timer
            _dashTimer += deltaTime;
            float dashDuration = 0.2f;

            float slowdown = Mathf.Exp(-10f * _dashTimer);
            Vector3 dashVelocity = _dashDirection * dashSpeed * Mathf.Max(slowdown, 1.1f); // THIS IS NOT THE PROBLEM

            // project on ground plane to avoid vertical movement
            dashVelocity = Vector3.ProjectOnPlane(dashVelocity, motor.CharacterUp);

            // unstick from ground
            motor.ForceUnground(0.2f); // THIS IS ALSO NOT THE PROBLEM

            currentVelocity = dashVelocity;

            if (_dashTimer >= dashDuration || dashVelocity.magnitude < 1f)
                _isDashing = false;
            return;
        }
        else if(_dashCooldown > 0f)
        {
            _dashCooldown -= deltaTime;
        }

        // if on ground
        if (motor.GroundingStatus.IsStableOnGround) 
        {
            _timeSinceUngrounded = 0f;
            _ungroundedDueToJump = false;
            _remainingJumps = airJumpCount;
            _killMomentum = _externalVelocityTimer <= 0f;

            var groundedMovement = motor.GetDirectionTangentToSurface
            (
                direction: _requestedMovement,
                surfaceNormal: motor.GroundingStatus.GroundNormal
            ) * _requestedMovement.magnitude;

            var targetVelocity = groundedMovement * walkSpeed;
            currentVelocity = Vector3.Lerp
            (
                a: currentVelocity,
                b: targetVelocity,
                t: 1f - Mathf.Exp(-walkResponse * deltaTime)
            );
        }
        else // if in air
        {
            _timeSinceUngrounded += deltaTime;
            // air movement
            if(_requestedMovement.sqrMagnitude > 0f)
            {
                var requestedPlanarMovement = Vector3.ProjectOnPlane // req movement projected onto movement plane
                (
                    vector: _requestedMovement,
                    planeNormal: motor.CharacterUp
                ) * _requestedMovement.magnitude;

                // current vel on movement plane
                Vector3 currentPlanarVelocity;
                currentPlanarVelocity = Vector3.ProjectOnPlane
                (
                    vector: currentVelocity,
                    planeNormal: motor.CharacterUp
                );

                // calculate movement force
                var movementForce = requestedPlanarMovement * airAcceleration * deltaTime;

                // add it to current planar vel for a target vel
                var targetPlanarVelocity = currentPlanarVelocity + movementForce;

                // limit target vel to air speed
                targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, airSpeed);

                // steer towards current vel. i have no idea what this means
                currentVelocity += targetPlanarVelocity - currentPlanarVelocity;

                // prevent air-climbing steep slopes (unintentional wall-riding)
                if (motor.GroundingStatus.FoundAnyGround)
                {
                    if(Vector3.Dot(movementForce, currentVelocity + movementForce) > 0f) // if moving in the same direction as resultant vel
                    {
                        // calculate obstruction normal
                        var obstructionNormal = Vector3.Cross
                        (
                            motor.CharacterUp,
                            Vector3.Cross
                            (
                                motor.CharacterUp,
                                motor.GroundingStatus.GroundNormal
                            )
                        ).normalized;

                        // project movement force onto obstruction plane
                        movementForce = Vector3.ProjectOnPlane(movementForce, obstructionNormal);
                    }
                }
            }
            else if(_externalVelocityTimer <= 0f) // stop movement if you're not inputting anything
            {
                Vector3 verticalVelocity = Vector3.Project(currentVelocity, motor.CharacterUp);
                float haltLerpSpeed = 5f; // higher number = faster halt in air
                currentVelocity = Vector3.Lerp(currentVelocity, verticalVelocity, haltLerpSpeed * deltaTime);
            }
            // gravity
            var terminalVel = 60f;
            if (_externalVelocityTimer <= 0f) currentVelocity += currentVelocity.y > -terminalVel ? motor.CharacterUp * gravity * deltaTime : Vector3.zero;
        }

        if(_requestedJump)
        {
            var grounded = motor.GroundingStatus.IsStableOnGround;
            var canCoyoteJump = _timeSinceUngrounded < coyoteTime && !_ungroundedDueToJump;

            if (grounded || canCoyoteJump || _remainingJumps > 0)
            {
                _requestedJump = false;

                // unstick from ground
                motor.ForceUnground(time: 0.1f); // has to be >0 for steep slope interaction
                _ungroundedDueToJump = true;

                // set min vertical speed to jump speed
                var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp); // dot product of current vel against character's up axis = vertical speed (is this true josh)
                var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpSpeed);

                // add difference btwn current and target vertical speed to character's vel
                currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);

                // subtract one from remaining jumps
                if (!(grounded || canCoyoteJump)) _remainingJumps--;
            }
            else
            {
                _timeSinceJumpRequest += deltaTime;
                
                // defer jump request until coyote time has passed
                var canJumpLater = _timeSinceJumpRequest < coyoteTime;
                _requestedJump = canJumpLater;
            }
        }

        /*
        float currentSpeed = currentVelocity.magnitude;
        // float t = Mathf.Clamp01(currentSpeed / maxSpeedForFOV);

        playerCamera.UpdateFOV(currentSpeed);
        // playerCamera.UpdateFOV(t);
        */
    }
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        // Update the character's rotation to face in the same direction as the requested rotation (camera rotation)

        // Don't want the character to pitch up and down, so the direction character looks should always be flattened.
        // This is done by projecting a vector pointing in the same direction that the player is looking onto a flat plane

        var forward = Vector3.ProjectOnPlane
        (
            _requestedRotation * Vector3.forward,
            motor.CharacterUp
        );

        currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
    }
    
    public void InjectExternalVelocity(Vector3 velocity, float duration)
    {
        // Debug.Log($"Injected velocity: {velocity} for {duration} seconds");
        _externalVelocity = velocity;
        _externalVelocityTimer = duration;
        _killMomentum = false;
    }

    #region Probably don't touch any of this?
    public void BeforeCharacterUpdate(float deltaTime)
    {
        
    }
    public void PostGroundingUpdate(float deltaTime)
    {
        
    }
    public void AfterCharacterUpdate(float deltaTime)
    {
        
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        
    }
    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        
    }
    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }
    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
        
    }
    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
        
    }
    #endregion

    public Transform GetCameraTarget() => cameraTarget;
    public KinematicCharacterMotor GetMotor() => motor;
}
