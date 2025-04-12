using UnityEngine;

public class GodstainedTripleBarrel : PrimaryFireBasic
{
    private int pelletCount = 12;
    private float spreadAngle = 5f;
    private float damage = 10f;
    private LayerMask hitMask = ~0; 

    private Transform cachedFirePoint;
    private Camera cachedCamera;

    protected override void OnFire()
    {
        if (character == null)
        {
            Debug.LogWarning("GodstainedTripleBarrel: Missing character.");
            return;
        }
        if (cachedFirePoint == null)
        {
            cachedFirePoint = character.transform.Find("FirePoint");
            if (cachedFirePoint == null)
            {
                Debug.LogWarning("GodstainedTripleBarrel: 'FirePoint' transform not found under character.");
                return;
            }
        }
       if (cachedCamera == null)
        {
        cachedCamera = character.GetComponentInChildren<Camera>();
        cachedCamera = Camera.main;
            if (cachedCamera == null)
            {
                Debug.LogWarning("GodstainedTripleBarrel: No camera found in character hierarchy.");
                return;
            }
        }

        Vector3 origin = cachedFirePoint.position;
        Vector3 aimDirection = cachedCamera.transform.forward;

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 direction = GetSpreadDirection(aimDirection, spreadAngle);

            if (Physics.Raycast(origin, direction, out RaycastHit hit, data.range, hitMask))
            {
                Debug.DrawLine(origin, hit.point, Color.red, 2f);

                // if (hit.collider.TryGetComponent<IDamageable>(out var target))
                // {
                //     target.TakeDamage(damage);
                // }
            }
        }
    }

    private Vector3 GetSpreadDirection(Vector3 forward, float maxAngle)
    {
        float yaw = Random.Range(-maxAngle, maxAngle);
        float pitch = Random.Range(-maxAngle, maxAngle);
        Quaternion spreadRotation = Quaternion.Euler(pitch, yaw, 0f);
        return spreadRotation * forward;
    }
}
