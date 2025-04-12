using UnityEngine;

public abstract class AbilityLogic
{
    protected Player player; // used for health/dmg tracking, ui interaction, etc
    protected PlayerCharacter character; // used for physics
    protected PlayerCamera camera; // used for camera
    protected AbilityDef definition; // info about the ability

    protected float durationTimer;

    public void Initialize(Player player, PlayerCharacter character, AbilityDef definition)
    {
        this.player = player;
        this.character = character;
        this.definition = definition;
    }

    public virtual void OnActivate()
    {
        durationTimer = definition.duration;
    }

    public virtual void Update(float deltaTime)
    {
        if(definition.duration > 0f)
        {
            durationTimer -= deltaTime;
            if(durationTimer <= 0f)
            {
                OnDurationEnd();
            }
        }
    }

    public virtual void OnDurationEnd()
    {

    }
    public virtual void Cancel()
    {

    }
}
