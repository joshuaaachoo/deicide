using UnityEngine;

public abstract class AbilityBasic
{
    protected Player player;
    protected PlayerCharacter character;
    protected AbilityData data;

    protected float cooldownTimer;
    protected float activeTimer;
    protected bool isActive;

    protected bool successful = true;

    public void Initialize(Player player, PlayerCharacter character, AbilityData data)
    {
        this.player = player;
        this.character = character;
        this.data = data;
    }

    public virtual void Activate()
    {
        if(IsReady())
        {
            OnActivate();
            if (successful)
            {
                Debug.Log("Ability used successfully! Ability name: " + data.name);

                activeTimer = data.activeTime;
                isActive = true;
            }
        }
        else
        {
            Debug.LogWarning($"{data.name} is not ready! Cooldown remaining: {cooldownTimer:F2}, isActive: {isActive}");
        }
    }

    public virtual void Tick(float deltaTime)
    {
        if (isActive)
        {
            activeTimer -= deltaTime;
            OnTick(deltaTime);

            if (activeTimer <= 0f)
                Deactivate();
        }
        else if (cooldownTimer > 0f)
            cooldownTimer -= deltaTime;
    }

    public virtual void Deactivate()
    {
        isActive = false;
        cooldownTimer = data.cooldownTime;
        OnDeactivate();
    }

    public bool IsReady() => cooldownTimer <= 0f && !isActive;
    public float GetCooldownPercent() => Mathf.Clamp01(cooldownTimer / data.cooldownTime);

    protected abstract void OnActivate();
    protected virtual void OnTick(float deltaTime) { }
    protected virtual void OnDeactivate() { }
}
