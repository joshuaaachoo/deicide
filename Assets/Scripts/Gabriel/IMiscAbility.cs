using UnityEngine;

public interface IMiscAbility
{
    void ActivateMisc();
    void TickMisc(float deltaTime);
    void DeactivateMisc();
}
