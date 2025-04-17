using KinematicCharacterController;
using UnityEngine;

public struct EnemyInput
{
    public Quaternion Rotation;
    public Vector2 Move;
    public bool Jump;
    public bool Attack;
}
public class EnemyCharacterController : MonoBehaviour, ICharacterController
{
    [SerializeField] private KinematicCharacterMotor motor;
    
    private float walkSpeed;
    private float walkResponse;
    private float airSpeed;
    private float airAcceleration;

    private float jumpSpeed;
    private float gravity = -90f;

    private float mass;

    private EnemyDefinition.MovementType movementType;
    private EnemyDefinition.EnemySize size;

    private Quaternion _requestedRotation;
    private Vector3 _requestedMovement;
    private bool _requestedJump;
    private bool _requestedAttack;

    private Vector3 _externalVelocity;
    private float _externalVelocityTimer = 0f;
    public void Initialize(EnemyDefinition def)
    {
        motor.CharacterController = this;

        walkSpeed = def.moveSpeed;
        walkResponse = walkSpeed * 1.25f;
        airSpeed = walkSpeed;
        airAcceleration = airSpeed * 10f;

        jumpSpeed = def.jumpSpeed;

        mass = def.mass;

        movementType = def.movementType;
        size = def.size;
    }
    public void UpdateInput(EnemyInput input)
    {
        _requestedRotation = input.Rotation;

        _requestedMovement = new Vector3(input.Move.x, 0f, input.Move.y); // turns 2d input vector into 3d movement vector on xz plane
        _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f); // normalizes vector
        _requestedMovement = input.Rotation * _requestedMovement;

        _requestedJump = input.Jump;
        _requestedAttack = input.Attack;
    }
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        var forward = Vector3.ProjectOnPlane
        (
            _requestedRotation * Vector3.forward,
            motor.CharacterUp
        );

        currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
    }
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (_externalVelocityTimer > 0f)
        {
            _externalVelocityTimer -= deltaTime;
            currentVelocity = _externalVelocity;

            if (_externalVelocityTimer <= 0f)
            {
                _externalVelocity = Vector3.zero;
            }
            return;
        }

        if (motor.GroundingStatus.IsStableOnGround)
        {
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
            // air movement
            if (_requestedMovement.sqrMagnitude > 0f)
            {
                // Debug.Log("Moving in air");
                var requestedPlanarMovement = motor.GetDirectionTangentToSurface // req movement projected onto movement plane
                (
                    direction: _requestedMovement,
                    surfaceNormal: Vector3.up
                ) * _requestedMovement.magnitude;

                // current vel on movement plane
                Vector3 currentPlanarVelocity;
                currentPlanarVelocity = Vector3.ProjectOnPlane
                (
                    vector: currentVelocity,
                    planeNormal: Vector3.up
                );

                // calculate movement force
                var movementForce = airAcceleration * deltaTime * requestedPlanarMovement;

                // add it to current planar vel for a target vel
                var targetPlanarVelocity = currentPlanarVelocity + movementForce;

                // limit target vel to air speed
                targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, airSpeed);

                // steer towards current vel. i have no idea what this means
                currentVelocity += targetPlanarVelocity - currentPlanarVelocity;

                // prevent air-climbing steep slopes (unintentional wall-riding)
                if (motor.GroundingStatus.FoundAnyGround)
                {
                    // Debug.Log("Preventing air-climbing");
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
            }
            else if (_externalVelocityTimer <= 0f) // stop movement if you're not inputting anything
            {
                Vector3 verticalVelocity = Vector3.Project(currentVelocity, Vector3.up);
                float haltLerpSpeed = walkResponse / 2; // higher number = faster halt in air
                currentVelocity = Vector3.Lerp(currentVelocity, verticalVelocity, haltLerpSpeed * deltaTime);
            }
            // gravity
            var terminalVel = 60f;
            if (_externalVelocityTimer <= 0f) currentVelocity += currentVelocity.y > -terminalVel ? Vector3.up * gravity * deltaTime : Vector3.zero;
        }

        if (_requestedJump)
        {
            var grounded = motor.GroundingStatus.IsStableOnGround;

            if (grounded)
            {
                _requestedJump = false;

                // unstick from ground
                motor.ForceUnground(time: 0.1f); // has to be >0 for steep slope interaction

                // set min vertical speed to jump speed
                var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp); // dot product of current vel against character's up axis = vertical speed (is this true josh)
                var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpSpeed);

                // add difference btwn current and target vertical speed to character's vel
                currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);
            }
        }
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
}
