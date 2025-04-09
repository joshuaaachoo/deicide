using UnityEngine;

public class CorrectiveAction : AbilityBasic, ISecondaryAbility
{
    private Vector3 _lungeDirection;
    private float _lungeSpeed = 50f; // 40 - 50 range seems good
    private float _lungeDuration = 0.2f;

    public void ActivateSecondary() => Activate();
    public void TickSecondary(float dt) => Tick(dt);
    public void DeactivateSecondary() => Deactivate();

    protected override void OnActivate()
    {
        Debug.Log("Corrective Action activated");

        // lunge in direction of camera
        _lungeDirection = player.GetCamera().transform.forward.normalized;

        // inject dash vel
        character.InjectExternalVelocity(_lungeDirection * _lungeSpeed, _lungeDuration, false);
    }

    protected override void OnTick(float deltaTime)
    {
        // logic
    }

    protected override void OnDeactivate()
    {
        Debug.Log("Corrective Action finished");
    }
}
