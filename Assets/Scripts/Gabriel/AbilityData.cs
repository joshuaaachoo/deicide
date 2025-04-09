using UnityEngine;

[CreateAssetMenu(menuName = "Modern Abilities/Ability Data")]
public class AbilityData : ScriptableObject
{
    public new string name;
    public Sprite icon;
    public float cooldownTime;
    public float activeTime;
    public string description;
}
