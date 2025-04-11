using UnityEngine;

[CreateAssetMenu(menuName = "Modern Abilities/Primary Fire Data")]
public class PrimaryFireData : ScriptableObject
{
    public enum RangeType
    {
        Melee,
        Ranged
    }
    [Tooltip("Name of the primary fire, displayed in game.")]
    public new string name;
    [Tooltip("Sprite icon of the primary fire, displayed in game.")]
    public Sprite icon;
    [Tooltip("Number of seconds one attack takes to execute.")]
    public float attackDuration; // how long does one attack take
    [Tooltip("Number of seconds it takes to reload ammo to its capacity.")]
    public float reloadSpeed;
    [Space]
    [Tooltip("Range type of the primary fire.")]
    public RangeType rangeType;
    [Tooltip("Range of the primary fire.")]
    public float range;
    [Tooltip("Ammo capacity for the primary fire. Set to 1 for melee.")]
    public int ammo;
    [Tooltip("Description of the primary fire, displayed in game.")]
    public string description;
}
