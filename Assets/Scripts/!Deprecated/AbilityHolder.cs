/* Handles Ability cooldown timers
 * 
 */
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    public Ability ability;
    float cooldownTime; // actually being subtracted from
    float activeTime; // same here

    enum AbilityState
    {
        ready,
        active,
        cooldown
    }
    AbilityState _state = AbilityState.ready;

    public KeyCode key;

    void Update() {
        switch (_state)
        {
            case AbilityState.ready:
                if (Input.GetKeyDown(key))
                {
                    Debug.Log("Ability used: " + ability.name);
                    ability.Activate();
                    //ability.Activate(gameObject);
                    _state = AbilityState.active;
                    activeTime = ability.activeTime;
                }
                break;

            case AbilityState.active:
                if (activeTime > 0)
                {
                    ability.Tick(Time.deltaTime);
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
                    _state = AbilityState.ready;
                    Debug.Log("Cooldown finished, ready again.");
                }
                break;
        }
    }
    public void EndAbility()
    {
        ability.Deactivate();
        //ability.Deactivate(gameObject);
        _state = AbilityState.cooldown;
        cooldownTime = ability.cooldownTime;
    }
    public Ability GetAbility()
    {
        return ability;
    }

    public bool IsOnCooldown()
    {
        return _state == AbilityState.cooldown;
    }

    public float GetCooldownPercent()
    {
        return Mathf.Clamp01(cooldownTime / ability.cooldownTime);
    }


}
