using UnityEngine;

public abstract class PassiveBasic
{
    protected Player player;
    protected PlayerCharacter character;
    protected PassiveData data;

    protected float cooldownTimer;
    protected bool onCooldown;

    public bool OnCooldown => onCooldown;

    public void Initialize(Player player, PlayerCharacter character, PassiveData data)
    {
        this.player = player;
        this.character = character;
        this.data = data;
    }

    public virtual void Tick(float deltaTime)
    {
        if (onCooldown)
        {
            cooldownTimer -= deltaTime;
            if (cooldownTimer <= 0f)
            {
                onCooldown = false;
                Debug.Log($"{data.name} is ready.");
            }
        }
        else OnTick(deltaTime);
    }

    protected virtual void OnTick(float deltaTime) { }
}
