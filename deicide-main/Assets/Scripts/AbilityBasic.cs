using UnityEngine;

public abstract class AbilityBasic
{
    protected Player player;
    protected PlayerCharacter character;
    protected AbilityData data;

    protected float cooldownTimer;
    protected float activeTimer;
    protected bool isActive;
    protected int charges;

    protected bool successful;

    // debug and gizmos
    protected bool _showDebug; // change this within unique abilities' OnTick()
    protected Vector3 _debugOrigin;
    protected Vector3 _debugDirection;
    protected float _debugDistance;
    protected float _debugRadius;
    public void Initialize(Player player, PlayerCharacter character, AbilityData data)
    {
        this.player = player;
        this.character = character;
        this.data = data;
        charges = this.data.charges;
        successful = true;
        _showDebug = false;

        AbilityDebugDrawer drawer = character.gameObject.AddComponent<AbilityDebugDrawer>();
    }

    // active and cooldown timer handlers
    public virtual void Activate()
    {
        if(IsReady())
        {
            OnActivate();
            if (successful)
            {
                Debug.Log($"{data.name} used successfully!");
                charges--; // consume a charge


                activeTimer = data.activeTime;
                isActive = true;
            }
        }
        else
        {
            Debug.LogWarning($"{data.name} is not ready! Cooldown remaining: {cooldownTimer:F2}, isActive: {isActive}, charges left: {charges}");
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
        else if (cooldownTimer <= 0f && charges < data.charges) 
        {
            charges++;
            Debug.Log($"{data.name} regenerated a charge. Charges left: {charges}, max charges: {data.charges}");
            if (charges < data.charges) cooldownTimer = data.cooldownTime;
        }
    }

    public virtual void Deactivate()
    {
        isActive = false;
        if(charges < data.charges && cooldownTimer <= 0f) cooldownTimer = data.cooldownTime;
        OnDeactivate();
    }

    public bool IsReady() => charges >= 1f && !isActive; // ready if more than 1 charge and not active
    public float GetCooldownPercent() => Mathf.Clamp01(cooldownTimer / data.cooldownTime);
    public bool ShouldShowDebug() => _showDebug;
    public Vector3 GetDebugDirection() => _debugDirection;
    public Vector3 GetDebugOrigin() => _debugOrigin;
    public float GetDebugDistance() => _debugDistance;
    public float GetDebugRadius() => _debugRadius;

    // logic to be overridden
    protected abstract void OnActivate();
    protected virtual void OnTick(float deltaTime) { }
    protected virtual void OnDeactivate() { }
}
