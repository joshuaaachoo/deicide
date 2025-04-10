using UnityEngine;

public class SleekAsSteel : AbilityBasic, IMiscAbility
{
    public void ActivateMisc() => Activate();
    public void TickMisc(float dt) => Tick(dt);
    public void DeactivateMisc() => Deactivate();

    protected override void OnActivate()
    {
        // on activate
        Debug.Log("Sleek as Steel activated");
    }
    protected override void OnTick(float deltaTime)
    {
        // updates every tick
    }
    protected override void OnDeactivate()
    {
        // on deactivate
        Debug.Log("Sleek as Steel finished");
    }
}
