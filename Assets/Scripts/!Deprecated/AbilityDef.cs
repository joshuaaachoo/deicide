/*  Base ability scriptable object
 *  This is just for surface information about the ability (name, description, etc.)
 */

using UnityEngine;

// [CreateAssetMenu(menuName = "Abilities/Ability Definition")]
public class AbilityDef : ScriptableObject
{
    public new string name;
    public string description;
    public Sprite icon;

    public float cooldown = 0f;
    public float duration = 0f;

    [Tooltip("The C# class name that implements this ability's logic.")]
    public string logicClassName; //<name><ability slot> e.g. GabrielMisc
}
