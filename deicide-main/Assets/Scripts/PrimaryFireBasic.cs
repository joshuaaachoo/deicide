using UnityEngine;

public abstract class PrimaryFireBasic
{
    protected enum PrimaryRange
    {
        Melee,
        Ranged
    }

    protected Player player;
    protected PlayerCharacter character;
    protected PlayerCamera camera;
    protected PrimaryFireData data;

    protected PrimaryRange rangeType;
    protected float attackTimer;
    protected float reloadTimer;
    protected bool isFiring;
    protected bool isReloading;
    protected int ammo;

    public void Initialize(Player player, PlayerCharacter character, PlayerCamera camera, PrimaryFireData data)
    {
        this.player = player;
        this.character = character;
        this.camera = camera;
        this.data = data;

        ammo = data.ammo;
        isReloading = false;
    }

    public virtual void Fire()
    {
        if(CanFire())
        {
            Debug.Log($"{data.name} fired!");
            OnFire();
            ammo--;

            attackTimer = data.attackDuration;
            isFiring = true;
        }
        else if (!isFiring && !isReloading && rangeType == PrimaryRange.Ranged)
        {
            Debug.Log($"{data.name} reloading...");
            Reload();
        }
        else
        {
            //Debug.Log($"{data.name} cannot be fired! Reload timer: {(int)reloadTimer}, isFiring: {isFiring}, ammo left: {ammo}");
        }
    }

    public virtual void Tick(float deltaTime)
    {
        if (isFiring) // already currently attacking
        {
            attackTimer -= deltaTime;
            OnTick(deltaTime);

            if (attackTimer <= 0f)
            {
                isFiring = false;
                if (rangeType == PrimaryRange.Melee) Reload();
            }
        }
        else if (reloadTimer > 0f) // not attacking but reloading
            reloadTimer -= deltaTime;
        else if (isReloading) // not attacking or reloading,
        {
            ammo = data.ammo;
            isReloading = false;
        }
    }

    public virtual void Reload()
    {
        reloadTimer = data.reloadSpeed;
        isReloading = true;
    }

    public bool CanFire() => ammo > 0f && !isFiring && !isReloading;

    protected abstract void OnFire();
    protected virtual void OnTick(float deltaTime) { }
}
