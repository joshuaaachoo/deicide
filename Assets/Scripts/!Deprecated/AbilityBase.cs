using UnityEngine;

public abstract class AbilityBase : MonoBehaviour
{
    public float cooldownTime = 1f;
    protected bool isOnCooldown = false;
    protected float cooldownTimer = 0f;
    
    protected virtual void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                isOnCooldown = false;
            }
        }
    }
    
    public virtual bool CanUseAbility()
    {
        return !isOnCooldown;
    }
    
    public abstract void ActivateAbility();
    
    protected void StartCooldown()
    {
        isOnCooldown = true;
        cooldownTimer = cooldownTime;
    }
}