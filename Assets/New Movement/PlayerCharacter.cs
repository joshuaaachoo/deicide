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
}

public class PlayerCharacter : MonoBehaviour, ICharacterController
{

    [SerializeField] private KinematicCharacterMotor motor;
    [SerializeField] private Transform cameraTarget;
    [Space]
    [SerializeField] private float walkSpeed = 20f;
    [SerializeField] private float walkResponse = 25f; // acceleration kind of?
    [SerializeField] private float airSpeed = 15f;
    [SerializeField] private float airAcceleration = 70f; // airSpeed and airAcceleration will be different for certain characters (looking at nyx), we can feel free to experiment
    [Space]
    [SerializeField] private float jumpSpeed = 27f; // jump speed which is technically jump height but with more math
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float gravity = -90f;

    private Quaternion _requestedRotation;
    private Vector3 _requestedMovement;
    private bool _requestedJump;
    private bool _requestedDash;

    private float _timeSinceUngrounded;
    private float _timeSinceJumpRequest;
    private bool _ungroundedDueToJump;
    private bool _isDashing;
    private float _dashTimer;
    private Vector3 _dashDirection;
    private float _dashSpeed = 5f;

    public void Initialize()
    {
        motor.CharacterController = this;
    }

    public void UpdateInput(CharacterInput input)
    {
        _requestedRotation = input.Rotation;
        
        _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y); // turns 2d input vector into 3d movement vector on xz plane
        _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f); // normalizes vector
        _requestedMovement = input.Rotation * _requestedMovement;

        _dashDirection = new Vector3(input.Move.x, 0f, input.Move.y);
        _dashDirection = Vector3.ClampMagnitude(_dashDirection, 1f);
        _dashDirection = input.Rotation * _dashDirection;

        var wasRequestingJump = _requestedJump;
        _requestedJump = _requestedJump || input.Jump;
        if (_requestedJump && !wasRequestingJump)
            _timeSinceJumpRequest = 0f;

        _requestedDash = input.Dash;
    }

    public void UpdateBody(float deltaTime)
    {
        
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        // dashing
        if(_requestedDash && _dashDirection != Vector3.zero && !_isDashing) // if you req dash + not standing still + not already dashing
        {
            _requestedDash = false;
            _isDashing = true;
            _dashTimer = 0f;
        }

        if (_isDashing)
        {
            // dash cooldown timer
            _dashTimer += deltaTime;
            float dashDuration = 0.2f;

            float slowdown = Mathf.Exp(-10f * _dashTimer);
            Vector3 dashVelocity = _dashDirection * _dashSpeed * slowdown; // THIS IS NOT THE PROBLEM

            // project on ground plane to avoid vertical movement
            dashVelocity = Vector3.ProjectOnPlane(dashVelocity, motor.CharacterUp);

            // unstick from ground
            motor.ForceUnground(0.2f); // THIS IS ALSO NOT THE PROBLEM

            currentVelocity = dashVelocity;

            if (_dashTimer >= dashDuration || dashVelocity.magnitude < 1f)
                _isDashing = false;
            return;
        }

        if (motor.GroundingStatus.IsStableOnGround) // if on ground
        {
            _timeSinceUngrounded = 0f;
            _ungroundedDueToJump = false;

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
                var planarMovement = Vector3.ProjectOnPlane // req movement projected onto movement plane
                (
                    vector: _requestedMovement,
                    planeNormal: motor.CharacterUp
                ) * _requestedMovement.magnitude;

                // current vel on movement plane
                var currentPlanarVelocity = Vector3.ProjectOnPlane
                (
                    vector: currentVelocity,
                    planeNormal: motor.CharacterUp
                );

                // calculate movement force
                var movementForce = planarMovement * airAcceleration * deltaTime;

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
            else // stop movement if you're not inputting anything
            {
                Vector3 verticalVelocity = Vector3.Project(currentVelocity, motor.CharacterUp);
                float haltLerpSpeed = 5f; // higher number = faster halt in air
                currentVelocity = Vector3.Lerp(currentVelocity, verticalVelocity, haltLerpSpeed * deltaTime);
            }
            // gravity
            currentVelocity += motor.CharacterUp * gravity * deltaTime;
        }

        if(_requestedJump)
        {
            var grounded = motor.GroundingStatus.IsStableOnGround;
            var canCoyoteJump = _timeSinceUngrounded < coyoteTime && !_ungroundedDueToJump;

            if (grounded || canCoyoteJump) // CHANGE THIS LATER FOR DOUBLE-JUMPING
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
            }
            else
            {
                _timeSinceJumpRequest += deltaTime;
                
                // defer jump request until coyote time has passed
                var canJumpLater = _timeSinceJumpRequest < coyoteTime;
                _requestedJump = canJumpLater;
            }
        }
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
}
