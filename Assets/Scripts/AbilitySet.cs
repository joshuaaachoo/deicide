using System;
using UnityEngine;

public class AbilitySet : MonoBehaviour
{
    private PassiveBasic passiveLogic;
    private PassiveData passiveData;

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

        passiveData = def.passiveData;
        primaryData = def.primaryData;
        secondaryData = def.secondaryData;
        mobilityData = def.mobilityData;
        miscData = def.miscData;

        passiveLogic = CreatePassiveLogic(def.passiveLogicClassName);
        primaryLogic = CreatePrimaryLogic(def.primaryLogicClassName);
        secondaryLogic = CreateAbilityLogic(def.secondaryLogicClassName);
        mobilityLogic = CreateAbilityLogic(def.mobilityLogicClassName);
        miscLogic = CreateAbilityLogic(def.miscLogicClassName);

        passiveLogic.Initialize(player, character, passiveData);
        primaryLogic.Initialize(player, character, player.GetCamera(), primaryData);
        secondaryLogic.Initialize(player, character, secondaryData);
        mobilityLogic.Initialize(player, character, mobilityData);
        miscLogic.Initialize(player, character, miscData);

        secondary = secondaryLogic as ISecondaryAbility;
        mobility = mobilityLogic as IMobilityAbility;
        misc = miscLogic as IMiscAbility;
    }

    private PassiveBasic CreatePassiveLogic(string className)
    {
        Type type = Type.GetType(className);
        if(type == null || !typeof(PassiveBasic).IsAssignableFrom(type))
        {
            Debug.LogError($"Invalid passive logic class: {className}");
            return null;
        }
        return (PassiveBasic)Activator.CreateInstance(type);
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
        passiveLogic.Tick(deltaTime);
        primaryLogic.Tick(deltaTime);
        secondary?.TickSecondary(deltaTime);
        mobility?.TickMobility(deltaTime);
        misc?.TickMisc(deltaTime);
    }
    
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
    public PassiveData GetPassiveData() => passiveData;
    public PrimaryFireData GetPrimaryData() => primaryData;
    public AbilityData GetSecondaryData() => secondaryData;
    public AbilityData GetMobilityData() => mobilityData;
    public AbilityData GetMiscData() => miscData;
    public PassiveBasic GetPassive() => passiveLogic;
    public PrimaryFireBasic GetPrimaryLogic() => primaryLogic;
    public AbilityBasic GetSecondaryLogic() => secondaryLogic;
    public AbilityBasic GetMobilityLogic() => mobilityLogic;
    public AbilityBasic GetMiscLogic() => miscLogic;
}
