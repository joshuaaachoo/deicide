using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    public Ability ability;
    float cooldownTime;
    float activeTime;

    enum AbilityState
    {
        ready, // can be used
        active, // is being used
        cooldown // can't be used
    }
    AbilityState state = AbilityState.ready;

    public KeyCode key; // even though there are only 3 possibilities for this, it's useful to have for when players want to change their keybinds

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case AbilityState.ready:
                if (Input.GetKeyDown(key))
                {
                    // use ability + make it active
                    ability.Activate();
                    state = AbilityState.active;
                    activeTime = ability.activeTime;
                }
                break;
            case AbilityState.active:
                if (activeTime > 0) { activeTime -= Time.deltaTime; }
                else
                {
                    state = AbilityState.cooldown;
                    cooldownTime = ability.cooldownTime;
                }
                break;
            case AbilityState.cooldown: // this could also be default
                if (cooldownTime > 0) { cooldownTime -= Time.deltaTime; }
                else
                {
                    state = AbilityState.ready;
                }
                break;
        }
    }
}
