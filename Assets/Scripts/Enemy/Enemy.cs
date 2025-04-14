/*  Alternative name is EnemyBase.cs, but you probably get the point
 * 
 */

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    private NavMeshAgent navAgent;
    [SerializeField] public EnemyDefinition enemyDefinition;

    // physics
    private Vector3 currentVelocity;
    private float gravity = -10f;

    private bool isKnockedBack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // motor = motor == null ? gameObject.AddComponent<EnemyCharacterMotor>() : gameObject.GetComponent<EnemyCharacterMotor>();
        // motor.Initialize(enemyDefinition);

        navAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        var deltaTime = Time.deltaTime;
        if(enemyDefinition.movementType == EnemyDefinition.MovementType.Grounded)
        {
            Ray wallRay = new Ray(transform.position, currentVelocity.normalized);
            bool hittingSlope = Physics.Raycast(wallRay, out RaycastHit hitSlope, 0.5f, LayerMask.GetMask("Terrain"));

            Vector3 normalGround = currentVelocity.normalized == Vector3.zero ? Vector3.down : -hitSlope.normal;
            Ray groundRay = new Ray(transform.position, normalGround);
            bool onGround = Physics.Raycast(groundRay, out RaycastHit hitGround, 0.5f, LayerMask.GetMask("Terrain", "Platform", "Wall"));
            // float radius = 0.5f;

            // bool onGround = Physics.Raycast(ray, out RaycastHit hitGround, 0.1f, LayerMask.GetMask("Terrain", "Platform", "Wall"));
            // bool onGround = Physics.SphereCast(transform.position, radius, Vector3.down, out RaycastHit hitGround, 0.5f, LayerMask.GetMask("Terrain", "Platform", "Wall"));
            // if (!onGround)
            // {
            //     transform.position += Vector3.up * gravity * deltaTime;
            // }
            // else if (!isKnockedBack && !navAgent.enabled) navAgent.enabled = true;

            // apply gravity if not on ground

            if (!onGround) // dont change to while
            {
                if (!hittingSlope) currentVelocity.y += gravity * deltaTime;
                else
                {
                    currentVelocity = Vector3.ProjectOnPlane(currentVelocity, hitSlope.normal);
                }
            }
            // enable navAgent if on ground and not knocked back
            else if (!isKnockedBack)
            {
                currentVelocity.y = 0f;
                navAgent.enabled = true;
            }
            else currentVelocity.y = 0f;
        }

        if(!navAgent.enabled) transform.position += currentVelocity;
        currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, 1f - Mathf.Exp(-25f * deltaTime));
    }

    public NavMeshAgent GetNavAgent() => navAgent;
    public void ApplyKnockback(Vector3 direction, float force)
    {
        if(!isKnockedBack)
        {
            float duration = (force / enemyDefinition.mass) * 0.05f;
            StartCoroutine(KnockbackRoutine(direction.normalized, force, duration));
        }
    }
    private IEnumerator KnockbackRoutine(Vector3 direction, float force, float duration)
    {
        isKnockedBack = true;
        navAgent.enabled = false;

        float timer = 0f;
        float maxDistancePerFrame;
        Vector3 currentDirection = direction;

        float deltaTime = Time.deltaTime;

        while (timer < duration)
        {
            float t = timer / duration;
            float currentForce = Mathf.Lerp(force, 0, t);
            maxDistancePerFrame = currentForce * deltaTime;

            Ray ray = new Ray(transform.position, currentDirection);
            if (Physics.Raycast(ray, out RaycastHit hitStop, maxDistancePerFrame, LayerMask.GetMask("Wall"))) break;
            if (Physics.Raycast(ray, out RaycastHit hitSlide, maxDistancePerFrame + 0.05f, LayerMask.GetMask("Terrain")))
            {
                Debug.DrawRay(transform.position, currentDirection * hitSlide.distance, Color.red, 0.1f);
                currentDirection = Vector3.ProjectOnPlane(currentDirection, hitSlide.normal).normalized;
                if (currentDirection.magnitude < 0.1f) break;
            }

            currentVelocity = currentDirection * currentForce * deltaTime;
            timer += deltaTime;
            yield return null;
        }

        isKnockedBack = false;
    }
}
