using UnityEngine;

[CreateAssetMenu(menuName = "Characters/Character Definition")]
public class CharacterDefinition : ScriptableObject
{
    public AbilityData secondaryData;
    public string secondaryLogicClassName;

    public AbilityData mobilityData;
    public string mobilityLogicClassName;

    public AbilityData miscData;
    public string miscLogicClassName;
}
