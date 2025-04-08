using UnityEngine;

public class Ability : ScriptableObject
{
    public new string name; // name of the ability
    public float cooldownTime; // max cooldown time
    public float activeTime; // active time for abilities that don't start cd immediately

    public virtual void Activate() { 
        // empty
    }
    public virtual void Activate(GameObject user) { 
        // empty, legit just for stratus for nowlol
    }
    public virtual void Deactivate(GameObject user) { 
        // empty
    }
}
