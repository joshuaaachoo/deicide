using KinematicCharacterController;
using UnityEngine;

public struct EnemyInputObsolete
{
    public Quaternion Rotation;
    public Vector2 MoveGrounded;
    public Vector3 MoveAerial;
    public bool Jump;
}
public abstract class EnemyMovementBase : MonoBehaviour, ICharacterController
{
    [SerializeField] private KinematicCharacterMotor motor;

    private float walkSpeed;
    private float walkResponse;
    private float airSpeed;
    private float airAcceleration;
    private float mass;

    private float jumpSpeed;
    private EnemyDefinition.MovementType movementType;

    private float gravity = -90f;

    private Quaternion _requestedRotation;
    private Vector3 _requestedMovement;
    private bool _requestedJump;

    private Vector3 _externalVelocity;
    private float _externalVelocityTimer;

    private float _timeSinceUngrounded;
    public void Initialize(EnemyDefinition def)
    {
        motor.CharacterController = this;

        walkSpeed = def.moveSpeed;
        walkResponse = walkSpeed * 1.25f;
        movementType = def.movementType;
        airSpeed = def.airSpeed;
        airAcceleration = def.airAcceleration;
        mass = def.mass;
        jumpSpeed = def.jumpSpeed;
    }
    public void UpdateInput(EnemyInput input)
    {

    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if(movementType == EnemyDefinition.MovementType.Grounded)
        {
            if(motor.GroundingStatus.IsStableOnGround)
            {
                _timeSinceUngrounded = 0f;

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
            else
            {
                _timeSinceUngrounded += deltaTime;

                if(_requestedMovement.sqrMagnitude > 0f)
                {
                    var planarMovement = Vector3.ProjectOnPlane
                    (
                        vector: _requestedMovement,
                        planeNormal: motor.CharacterUp
                    ) * _requestedMovement.magnitude;

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

                    /*
                    if (motor.GroundingStatus.FoundAnyGround)
                    {
                        if (Vector3.Dot(movementForce, currentVelocity + movementForce) > 0f) // if moving in the same direction as resultant vel
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
                    */
                }
                else
                {
                    Vector3 verticalVelocity = Vector3.Project(currentVelocity, motor.CharacterUp);
                    float haltLerpSpeed = 5f; // higher number = faster halt in air
                    currentVelocity = Vector3.Lerp(currentVelocity, verticalVelocity, haltLerpSpeed * deltaTime);
                }
                // gravity
                currentVelocity += motor.CharacterUp * gravity * deltaTime;
            }
            // jump logic here
            if(_requestedJump)
            {

            }
        }
        else if(movementType == EnemyDefinition.MovementType.Aerial)
        {
            // im cooked
        }

        currentVelocity += _externalVelocity;
        _externalVelocity = _externalVelocity.magnitude >= 1f ? Vector3.Lerp(_externalVelocity, Vector3.zero, deltaTime * Mathf.Sqrt(mass)) : Vector3.zero;
        if (_externalVelocity.magnitude >= 1f) motor.ForceUnground();
    }

    #region Probably don't touch this
    void ICharacterController.BeforeCharacterUpdate(float deltaTime)
    {
        
    }

    void ICharacterController.PostGroundingUpdate(float deltaTime)
    {
        
    }

    void ICharacterController.AfterCharacterUpdate(float deltaTime)
    {
        
    }

    bool ICharacterController.IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    void ICharacterController.OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        
    }

    void ICharacterController.OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        
    }

    void ICharacterController.ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
        
    }

    void ICharacterController.OnDiscreteCollisionDetected(Collider hitCollider)
    {
        
    }
    #endregion

    public void ApplyKnockback(Vector3 direction, float force)
    {
        Vector3 flatDirection = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;
        float kbMagnitude = force / mass;
        _externalVelocity = motor.GroundingStatus.IsStableOnGround ? flatDirection * kbMagnitude : direction * kbMagnitude;

        // _externalVelocity = motor.GroundingStatus.IsStableOnGround ? flatDirection * force / mass : direction * force / mass;
        // _externalVelocityTimer = duration;
    }
}
