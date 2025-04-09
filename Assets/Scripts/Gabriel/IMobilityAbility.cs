using UnityEngine;

public interface IMobilityAbility
{
    void ActivateMobility();
    void TickMobility(float deltaTime);
    void DeactivateMobility();
}
