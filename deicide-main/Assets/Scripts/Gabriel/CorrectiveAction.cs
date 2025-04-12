using UnityEngine;

public class CorrectiveAction : AbilityBasic, ISecondaryAbility
{
    private Vector3 _lungeDirection;
    private float _lungeSpeed = 50f; // 40 - 50 range seems good
    private float _lungeDuration = 0.2f;

    private bool _hasHit = false;
    private Vector3 origin;
    private float detectionRadius = 1f;
    private float detectionRange = 1f;
    private float punchForce = 200f;

    public void ActivateSecondary() => Activate();
    public void TickSecondary(float dt) => Tick(dt);
    public void DeactivateSecondary() => Deactivate();

    protected override void OnActivate()
    {
        Debug.Log("Corrective Action activated");

        // lunge in direction of camera
        _lungeDirection = player.GetCamera().transform.forward.normalized;

        // inject dash vel
        character.InjectExternalVelocity(_lungeDirection * _lungeSpeed, _lungeDuration, false);
        character.GetMotor().ForceUnground(data.activeTime); // ensures you unstick from ground on use

        // debug gizmo drawing
        var drawer = character.gameObject.GetComponent<AbilityDebugDrawer>();
        drawer.debugAction = this;
    }

    protected override void OnTick(float deltaTime)
    {
        _showDebug = true;

        // check for hits
        if (_hasHit) return;

        origin = player.GetCamera().transform.position;

        if (Physics.SphereCast(origin, detectionRadius, _lungeDirection, out RaycastHit hit, detectionRange, LayerMask.GetMask("Enemy")))
        {
            Enemy enemy = hit.collider.GetComponentInParent<Enemy>();
            if(enemy != null)
            {
                Debug.Log($"Corrective Action hit: {enemy.name}");

                _hasHit = true;
                activeTimer = 0.05f;

                // apply kb and damage
                // switch checks for enemy size
                // small: knockback, if hit wall => more damag
                // medium: waiting for lockstun response
                // large: knock self back, stun enemy
                // boss: knock self back

                switch(enemy.enemyDefinition.size)
                {
                    case EnemyDefinition.EnemySize.Large:
                        break;
                    default:
                        Vector3 knockDirection = _lungeDirection.normalized;
                        character.InjectExternalVelocity(_lungeDirection * _lungeSpeed / (data.activeTime / 0.05f), 0.05f, false); // this equation stinks
                        enemy.GetEnemyCharacter().ApplyKnockback(knockDirection, punchForce, 1f);
                        break;
                }
            }
        }

        if (_showDebug)
        {
            _debugDirection = _lungeDirection;
            _debugOrigin = origin;
            _debugRadius = detectionRadius;
            _debugDistance = detectionRange;
        }
    }

    protected override void OnDeactivate()
    {
        Debug.Log("Corrective Action finished");

        _hasHit = false;
        _showDebug = false;
    }
}
