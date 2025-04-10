using UnityEngine;

public class RaptureAndRevelation : AbilityBasic, IMobilityAbility
{
    public void ActivateMobility() => Activate();
    public void TickMobility(float dt) => Tick(dt);
    public void DeactivateMobility() => Deactivate();

    protected override void OnActivate()
    {
        Debug.Log("Rapture and Revelation activated");
        // ryze
    }
    protected override void OnTick(float deltaTime)
    {
        // logic
    }

    protected override void OnDeactivate()
    {
        Debug.Log("Rapture and Revelation finished");
    }
}
