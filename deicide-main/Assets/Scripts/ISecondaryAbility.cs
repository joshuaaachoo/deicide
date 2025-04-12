using UnityEngine;

public interface ISecondaryAbility
{
    void ActivateSecondary();
    void TickSecondary(float deltaTime);
    void DeactivateSecondary();
}
