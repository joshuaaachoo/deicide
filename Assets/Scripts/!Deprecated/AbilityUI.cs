using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [Header("UI References")]
    public AbilityHolder mobilitySlot;
    public Image cooldownFill;

    void Update()
    {
        if (mobilitySlot == null || mobilitySlot.GetAbility() == null)
        {
            cooldownFill.fillAmount = 0f;
            return;
        }

        if (mobilitySlot.IsOnCooldown())
        {
            cooldownFill.fillAmount = mobilitySlot.GetCooldownPercent();
        }
        else
        {
            cooldownFill.fillAmount = 0f;
        }
    }
}
