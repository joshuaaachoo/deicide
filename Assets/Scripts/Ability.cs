using UnityEngine;

public class Ability : ScriptableObject
{
    public new string name; // name of the ability
    public float cooldownTime; // max cooldown time
    public float activeTime; // active time for abilities that don't start cooldowns immediately

    public virtual void Activate() { 
        // empty
    }
}
