using KinematicCharacterController;
using UnityEngine;

public abstract class EnemyMovementBase : MonoBehaviour, ICharacterController
{
    public struct EnemyInput
    {
        public Quaternion Rotation;
        public Vector2 MoveGrounded;
        public Vector3 MoveAerial;
        public bool Jump;
    }

    [SerializeField] private KinematicCharacterMotor motor;

    private float walkSpeed;
    private float walkResponse;
    private float airSpeed;
    private float airAcceleration;

    private float jumpSpeed;
    private EnemyDefinition.MovementType movementType;

    private float gravity = -90f;

    private Quaternion _requestedRotation;
    private Vector3 _requestedMovement;
    private bool _requestedJump;
    public void Initialize(EnemyDefinition def)
    {
        motor.CharacterController = this;

        walkSpeed = def.moveSpeed;
        walkResponse = walkSpeed * 1.25f;
        movementType = def.movementType;
        airSpeed = def.airSpeed;
        airAcceleration = def.airAcceleration;
        jumpSpeed = def.jumpSpeed;
    }
    public void UpdateInput()
    {

    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        // gravity
        currentVelocity += motor.CharacterUp * gravity * deltaTime;
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
}
