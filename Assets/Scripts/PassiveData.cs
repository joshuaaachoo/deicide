using UnityEngine;

[CreateAssetMenu(menuName = "Modern Abilities/Passive Data")]
public class PassiveData : ScriptableObject
{
    public new string name;
    public string description;
    public Sprite icon;
    public float cooldownTime;
}
