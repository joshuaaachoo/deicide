/*  Alternative name is EnemyBase.cs, but you probably get the point
 * 
 */

using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyMovementBase enemyCharacter;
    [SerializeField] private EnemyDefinition enemyDefinition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyCharacter.Initialize(enemyDefinition);
    }

    // Update is called once per frame
    void Update()
    {
        var deltaTime = Time.deltaTime;
        // logic
    }
}
