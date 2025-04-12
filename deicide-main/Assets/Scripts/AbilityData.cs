using UnityEngine;

[CreateAssetMenu(menuName = "Modern Abilities/Ability Data")]
public class AbilityData : ScriptableObject
{
    [Tooltip("Name of the ability, displayed in game.")]
    public new string name;
    [Tooltip("Sprite icon of the ability, displayed in game.")]
    public Sprite icon;
    [Tooltip("Number of seconds this ability cannot be used for after using it.")]
    public float cooldownTime;
    [Tooltip("Number of seconds this ability takes to execute.")]
    public float activeTime;
    [Tooltip("Maximum number of separate uses of this ability (usually 1).")]
    public int charges;
    [Tooltip("Description of the ability, displayed in game.")]
    public string description;
}
