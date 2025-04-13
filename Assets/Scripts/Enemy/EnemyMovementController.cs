using UnityEngine;

public class EnemyMovementController : MonoBehaviour
{
    private Rigidbody rigidbody;
    public void Initialize(EnemyDefinition def)
    {
        rigidbody = rigidbody == null ? gameObject.AddComponent<Rigidbody>() : gameObject.GetComponent<Rigidbody>();
        rigidbody.mass = def.mass;
        rigidbody.useGravity = def.movementType == EnemyDefinition.MovementType.Grounded;
        rigidbody.isKinematic = true;
        rigidbody.includeLayers = LayerMask.GetMask("Terrain", "Wall", "Platform");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        Vector3 flatDirection = Vector3.ProjectOnPlane(direction, gameObject.transform.up);
        rigidbody.AddForce(direction * force, mode: ForceMode.Impulse);
    }
}
