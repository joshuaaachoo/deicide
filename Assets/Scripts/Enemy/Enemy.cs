/*  Alternative name is EnemyBase.cs, but you probably get the point
 * 
 */

using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    private EnemyCharacterMotor motor;
    [SerializeField] public EnemyDefinition enemyDefinition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        motor = motor == null ? gameObject.AddComponent<EnemyCharacterMotor>() : gameObject.GetComponent<EnemyCharacterMotor>();
        motor.Initialize(enemyDefinition);
    }

    // Update is called once per frame
    void Update()
    {
        var deltaTime = Time.deltaTime;
    }

    public EnemyCharacterMotor GetCharacterMotor() => motor;
}
