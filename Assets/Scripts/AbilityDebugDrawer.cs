using UnityEngine;

public class AbilityDebugDrawer : MonoBehaviour
{
    [SerializeField] public AbilityBasic debugAction;

    private void OnDrawGizmos()
    {
        if (debugAction == null || !debugAction.ShouldShowDebug()) return;

        // Debug.Log("Drawing Gizmos");

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(debugAction.GetDebugOrigin(), debugAction.GetDebugDirection() * debugAction.GetDebugDistance());
        Gizmos.DrawWireSphere(debugAction.GetDebugOrigin() + debugAction.GetDebugDirection() * debugAction.GetDebugDistance(), debugAction.GetDebugRadius());
    }
}
