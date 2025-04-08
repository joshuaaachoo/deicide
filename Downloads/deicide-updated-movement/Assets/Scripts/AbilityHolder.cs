using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    public Ability ability;
    float cooldownTime;
    float activeTime;

    enum AbilityState
    {
        ready,
        active,
        cooldown
    }
    AbilityState state = AbilityState.ready;

    public KeyCode key;

    void Update() {
        switch (state)
        {
            case AbilityState.ready:
                if (Input.GetKeyDown(key))
                {
                    Debug.Log("Ability used: " + ability.name);
                    ability.Activate(gameObject);
                    state = AbilityState.active;
                    activeTime = ability.activeTime;
                }
                break;

            case AbilityState.active:
                if (activeTime > 0)
                {
                    activeTime -= Time.deltaTime;
                    if (activeTime <= 0f)
                    {
                        EndAbility();
                    }
                }
                break;

            case AbilityState.cooldown:
                cooldownTime -= Time.deltaTime;
                if (cooldownTime <= 0f)
                {
                    state = AbilityState.ready;
                    Debug.Log("Cooldown finished, ready again.");
                }
                break;
        }
    }
    public void EndAbility()
    {
        ability.Deactivate(gameObject);
        state = AbilityState.cooldown;
        cooldownTime = ability.cooldownTime;
    }
    public Ability GetAbility()
    {
        return ability;
    }

    public bool IsOnCooldown()
    {
        return state == AbilityState.cooldown;
    }

    public float GetCooldownPercent()
    {
        return Mathf.Clamp01(cooldownTime / ability.cooldownTime);
    }


}
