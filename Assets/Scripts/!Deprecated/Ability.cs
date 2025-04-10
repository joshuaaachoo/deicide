/* Handles Ability logic + info, such as the ability's name, description, icon, and base times
 * Naming convention: <character name><2/3/4>, where 2 is secondary fire, 3 is mobility, 4 is misc
 */
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Ability Object")]
public class Ability : ScriptableObject
{
    public new string name; // name of the ability
    public string description; // description of the ability
    public Sprite icon; // icon

    public float cooldownTime; // max cooldown time
    public float activeTime; // active time for abilities that don't start cd immediately (duration)

    [SerializeField] private Player player; // used for health/dmg tracking, ui interaction, etc
    [SerializeField] private PlayerCharacter character; // used for physics
    [SerializeField] private PlayerCamera camera; // used for camera

    public virtual void Activate() { 
        // empty
    }
    public virtual void Activate(GameObject user) { 
        // empty, legit just for stratus for nowlol
    }
    public virtual void Deactivate() {
        // empty
    }
    public virtual void Deactivate(GameObject user) { 
        // empty
    }
    public virtual void Tick(float deltaTime) {
        // empty
    }
}
