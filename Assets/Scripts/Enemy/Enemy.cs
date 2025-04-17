/*  Alternative name is EnemyBase.cs, but you probably get the point
 * 
 */

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyCharacterController character;
    [SerializeField] public EnemyDefinition enemyDefinition;

    // ai
    private float detectionRadius;
    private PlayerCharacter playerTarget;

    private bool isKnockedBack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // motor = motor == null ? gameObject.AddComponent<EnemyCharacterMotor>() : gameObject.GetComponent<EnemyCharacterMotor>();
        // motor.Initialize(enemyDefinition);
        character.Initialize(enemyDefinition);
        detectionRadius = enemyDefinition.detectionRadius;
    }

    // Update is called once per frame
    void Update()
    {
        // debug results:
        // playerTarget is updated correctly
        // playerTarget position is updated correctly
        // look vector is updated correctly
        // look quaternion is updated correctly
        // everything is updated correctly
        // The enemy is Just Suicidal for some reason

        var deltaTime = Time.deltaTime;
        if(!isKnockedBack && playerTarget == null) UpdateAggro();
        Debug.Log($"playerTarget.transform.position: {playerTarget.transform.position}");

        Vector3 look = Vector3.zero;

        if (playerTarget != null)
        {
            look = (playerTarget.transform.position - transform.position).normalized;
            look.y = 0f;
        }

        Vector2 move = Vector2.up;

        var input = new EnemyInput
        {
            Rotation = Quaternion.LookRotation(look),
            Move = move,
            Jump = false, // temp
            Attack = false // temp
        };

        character.UpdateInput(input);
    }

    void UpdateAggro()
    {
        if (playerTarget != null)
        {
            // Debug.Log($"{enemyDefinition.enemyName} already has a player target");
            return; // if already have target, don't update aggro
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, LayerMask.GetMask("Player")); // gets all colliders in sphere with detectionRadius

        foreach (Collider hit in hits)
        {
            playerTarget = hit.GetComponent<PlayerCharacter>(); // get Player component in each hit
            if (playerTarget != null)
            {
                Debug.Log($"{enemyDefinition.enemyName} found a player");
                break; // if it worked, break
            }
        }
    }
    /*
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
    */
    public EnemyCharacterController GetEnemyCharacter() => character;
    public EnemyDefinition GetEnemyDefinition() => enemyDefinition;
}
