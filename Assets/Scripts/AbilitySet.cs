using System;
using UnityEngine;

public class AbilitySet : MonoBehaviour
{
    // private PassiveBasic passiveLogic;

    private PrimaryFireBasic primaryLogic;
    private PrimaryFireData primaryData;
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

        primaryData = def.primaryData;
        secondaryData = def.secondaryData;
        mobilityData = def.mobilityData;
        miscData = def.miscData;

        primaryLogic = CreatePrimaryLogic(def.primaryLogicClassName);
        secondaryLogic = CreateAbilityLogic(def.secondaryLogicClassName);
        mobilityLogic = CreateAbilityLogic(def.mobilityLogicClassName);
        miscLogic = CreateAbilityLogic(def.miscLogicClassName);

        primaryLogic.Initialize(player, character, player.GetCamera(), primaryData);
        secondaryLogic.Initialize(player, character, secondaryData);
        mobilityLogic.Initialize(player, character, mobilityData);
        miscLogic.Initialize(player, character, miscData);

        secondary = secondaryLogic as ISecondaryAbility;
        mobility = mobilityLogic as IMobilityAbility;
        misc = miscLogic as IMiscAbility;
    }

    private PrimaryFireBasic CreatePrimaryLogic(string className)
    {
        Type type = Type.GetType(className);
        if(type == null || !typeof(PrimaryFireBasic).IsAssignableFrom(type))
        {
            Debug.LogError($"Invalid primary fire logic class: {className}");
            return null;
        }
        return (PrimaryFireBasic)Activator.CreateInstance(type);
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
        // passiveLogic.Tick(deltaTime);
        primaryLogic.Tick(deltaTime);
        secondary?.TickSecondary(deltaTime);
        mobility?.TickMobility(deltaTime);
        misc?.TickMisc(deltaTime);
    }
    // public PassiveBasic GetPassive() => passiveLogic;
    public void TryFirePrimary()
    {
        primaryLogic.Fire();
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
    public PrimaryFireData GetPrimaryData() => primaryData;
    public AbilityData GetSecondaryData() => secondaryData;
    public AbilityData GetMobilityData() => mobilityData;
    public AbilityData GetMiscData() => miscData;
    public PrimaryFireBasic GetPrimaryLogic() => primaryLogic;
    public AbilityBasic GetSecondaryLogic() => secondaryLogic;
    public AbilityBasic GetMobilityLogic() => mobilityLogic;
    public AbilityBasic GetMiscLogic() => miscLogic;
}
