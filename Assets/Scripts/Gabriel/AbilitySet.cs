using System;
using UnityEngine;

public class AbilitySet : MonoBehaviour
{
    public AbilityData secondaryData, mobilityData, miscData;
    public AbilityBasic secondaryLogic, mobilityLogic, miscLogic;

    private ISecondaryAbility secondary;
    private IMobilityAbility mobility;
    private IMiscAbility misc;

    private Player player;
    private PlayerCharacter character;

    public void Initialize(Player p, PlayerCharacter c, CharacterDefinition def)
    {
        player = p;
        character = c;

        secondaryLogic = CreateAbilityLogic(def.secondaryLogicClassName);
        // mobilityLogic
        // miscLogic

        secondaryLogic.Initialize(player, character, secondaryData);
        // init mobi logic
        // init misc logic

        secondary = secondaryLogic as ISecondaryAbility;
        // interface mobi
        // interface misc
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
