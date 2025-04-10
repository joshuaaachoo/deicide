using System;
using UnityEngine;

public class AbilitySet : MonoBehaviour
{
    private AbilityData secondaryData, mobilityData, miscData;
    private AbilityBasic secondaryLogic, mobilityLogic, miscLogic;

    private ISecondaryAbility secondary;
    private IMobilityAbility mobility;
    private IMiscAbility misc;

    private Player player;
    private PlayerCharacter character;

    public void Initialize(Player p, PlayerCharacter c, CharacterDefinition def)
    {
        player = p;
        character = c;

        secondaryData = def.secondaryData;
        mobilityData = def.mobilityData;
        miscData = def.miscData;

        secondaryLogic = CreateAbilityLogic(def.secondaryLogicClassName);
        mobilityLogic = CreateAbilityLogic(def.mobilityLogicClassName);
        miscLogic = CreateAbilityLogic(def.miscLogicClassName);

        secondaryLogic.Initialize(player, character, secondaryData);
        mobilityLogic.Initialize(player, character, mobilityData);
        miscLogic.Initialize(player, character, miscData);

        secondary = secondaryLogic as ISecondaryAbility;
        mobility = mobilityLogic as IMobilityAbility;
        misc = miscLogic as IMiscAbility;
    }

    private AbilityBasic CreateAbilityLogic(string className)
    {
        Type type = Type.GetType(className);
        if(type == null || !typeof(AbilityBasic).IsAssignableFrom(type))
        {
            Debug.LogError($"Invalid ability logic class: {className}");
            return null;
        }
        return (AbilityBasic)Activator.CreateInstance(type);
    }

    public void UpdateAbilities(float deltaTime)
    {
        secondary?.TickSecondary(deltaTime);
        mobility?.TickMobility(deltaTime);
        misc?.TickMisc(deltaTime);
    }

    public void TryActivateSecondary()
    {
        secondary?.ActivateSecondary();
    }
    public void TryActivateMobility()
    {
        mobility?.ActivateMobility();
    }
    public void TryActivateMisc()
    {
        misc?.ActivateMisc();
    }
}
