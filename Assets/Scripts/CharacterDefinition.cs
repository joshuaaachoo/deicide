using UnityEngine;

[CreateAssetMenu(menuName = "Characters/Character Definition")]
public class CharacterDefinition : ScriptableObject
{
    public string characterName;

    [Header("Stats (hover for default values)")]
    [Tooltip("Default: 20")]
    public float walkSpeed;
    [Tooltip("Default: 15")]
    public float airSpeed;
    [Tooltip("Default: 70")]
    public float airAcceleration;
    [Tooltip("Default: 27")]
    public float jumpSpeed;
    [Tooltip("Default: 0")]
    public float armor;
    [Tooltip("Default: 45")]
    public float dashSpeed;
    [Tooltip("Default: 1")]
    public int airJumpCount;

    [Header("Abilities")]
    public AbilityData secondaryData;
    public string secondaryLogicClassName;

    public AbilityData mobilityData;
    public string mobilityLogicClassName;

    public AbilityData miscData;
    public string miscLogicClassName;
}
