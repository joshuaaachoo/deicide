using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float health = 100f;
    public float moveSpeed = 3f;
    public Transform player;
    
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Find player if not set
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }
    
    void Update()
    {
        if (player == null)
            return;
            
        // Simple follow behavior
        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 movement = direction * moveSpeed * Time.deltaTime;
        
        // Keep y position constant
        movement.y = 0;
        
        rb.MovePosition(transform.position + movement);
        
        // Look at player
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }
    
    public void TakeDamage(float damage)
    {
        health -= damage;
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        Destroy(gameObject);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Check if collision is with bullet
        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(20f); // Example damage value
            Destroy(collision.gameObject);
        }
    }
}