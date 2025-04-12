using UnityEngine;

public class PartingGift : AbilityBasic, IMiscAbility
{
    public void ActivateMisc() => Activate();
    public void TickMisc(float dt) => Tick(dt);
    public void DeactivateMisc() => Deactivate();

    protected override void OnActivate()
    {
        Debug.Log("Parting Gift activated");
        // equip grenade
    }
    protected override void OnTick(float deltaTime)
    {
        // logic
    }

    protected override void OnDeactivate()
    {
        Debug.Log("Parting Gift finished");
    }
}
