using System.Collections;
using UnityEngine;

public class EnemyCharacterMotor : MonoBehaviour
{
    private float moveSpeed;
    private float acceleration;
    private float mass;

    private float gravity;
    private float groundCheckDistance;

    private float maxSlopeAngle;

    private Vector3 _velocity;
    private Vector3 _inputDirection;
    private bool _isGrounded;
    private Vector3 _groundNormal = Vector3.up;

    private CapsuleCollider _collider;

    // read-only
    public bool IsGrounded => _isGrounded;
    public Vector3 Velocity => _velocity;

    private void Awake()
    {
        _collider = GetComponentInChildren<CapsuleCollider>();
    }

    public void Initialize(EnemyDefinition enemyDef)
    {
        moveSpeed = enemyDef.moveSpeed;
        mass = enemyDef.mass;
        acceleration = 5f;
        gravity = 90f;
        groundCheckDistance = 0.3f;
        maxSlopeAngle = 45f;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        CheckGround();

        ApplyGravity(deltaTime);
        ApplyMovement(deltaTime);

        transform.position += _velocity * deltaTime;
    }

    public void SetInput(Vector3 direction)
    {
        _inputDirection = direction.normalized;
    }

    public void SetExternalVelocity(Vector3 velocity)
    {
        _velocity = velocity;
    }

    private void CheckGround()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        RaycastHit hit;

        if(Physics.SphereCast(origin, _collider.radius * 0.9f, Vector3.down, out hit, groundCheckDistance + 0.1f, LayerMask.GetMask("Terrain")))
        {
            _isGrounded = Vector3.Angle(hit.normal, Vector3.up) <= maxSlopeAngle;
            _groundNormal = hit.normal;
        }
        else
        {
            _isGrounded = false;
            _groundNormal = Vector3.up;
        }
    }

    private void ApplyGravity(float deltaTime)
    {
        if (!_isGrounded) _velocity += Vector3.down * Mathf.Abs(gravity) * deltaTime;
        else if (_velocity.y < 0f) _velocity.y = 0f;
    }

    private void ApplyMovement(float deltaTime)
    {
        Vector3 targetVelocity = _inputDirection * moveSpeed;

        if(_isGrounded)
        {
            Vector3 slopeRight = Vector3.Cross(Vector3.up, _groundNormal);
            Vector3 slopeForward = Vector3.Cross(slopeRight, _groundNormal);
            targetVelocity = Vector3.Project(targetVelocity, slopeForward);
        }
        Vector3 horizontal = Vector3.MoveTowards(_velocity, targetVelocity, acceleration * deltaTime);
        _velocity.x = horizontal.x;
        _velocity.z = horizontal.z;
    }

    public void ApplyKnockback(Vector3 direction, float force, float duration)
    {
        StartCoroutine(KnockbackRoutine(direction, force, duration));
    }
    private IEnumerator KnockbackRoutine(Vector3 direction, float force, float duration)
    {
        float timer = 0f;
        Vector3 knockVelocity = direction * force/mass;

        while (timer < duration)
        {
            _velocity.x = knockVelocity.x;
            _velocity.z = knockVelocity.z;

            knockVelocity = Vector3.Lerp(knockVelocity, Vector3.zero, Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

    }
}
