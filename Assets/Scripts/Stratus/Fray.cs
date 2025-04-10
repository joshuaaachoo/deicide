using UnityEngine;

public class Fray : AbilityBasic, ISecondaryAbility
{
    public void ActivateSecondary() => Activate();
    public void TickSecondary(float dt) => Tick(dt);
    public void DeactivateSecondary() => Deactivate();

    protected override void OnActivate()
    {
        // on activate
        Debug.Log("Fray activated");
    }
    protected override void OnTick(float deltaTime)
    {
        // updates every tick
    }
    protected override void OnDeactivate()
    {
        // on deactivate
        Debug.Log("Fray finished");
    }
}
