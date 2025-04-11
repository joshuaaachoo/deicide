using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy Definition")]
public class EnemyDefinition : ScriptableObject
{
    public enum EnemySize
    {
        Small,
        Medium,
        Large
    };
    public enum MovementType
    {
        Grounded,
        Aerial
    }
    public string enemyName;

    [Header("Physics Stats")]
    public EnemySize size;
    public MovementType movementType;
    public float mass;
    public float moveSpeed;
    public float airSpeed;
    public float airAcceleration;
    public float jumpSpeed;

    [Header("Gameplay Stats")]
    public float maxHealth;
    public float baseHealthRegen;
    public float armor;
}
