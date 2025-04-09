using UnityEngine;

public class StraightShooter : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 30f;
    public float fireRate = 0.2f;
    public float bulletLifetime = 3f;

    [Header("References")]
    public Transform firePoint;
    public Camera playerCamera;

    private float nextFireTime = 0f;

    void Start()
    {
        // Auto-assign references if not set
        if (firePoint == null) firePoint = transform;
        if (playerCamera == null) playerCamera = Camera.main;
    }

    void Update()
    {
        // Fire on left mouse button
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Fire()
    {
        // Create a ray from camera center
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        // Instantiate bullet at firePoint
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        
        // Add a BulletController component to handle movement
        BulletController bulletController = bullet.AddComponent<BulletController>();
        bulletController.Initialize(ray.direction, bulletSpeed, bulletLifetime);
        
        // Debug visualization
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 0.5f);
    }
}

// Separate controller for bullet movement
public class BulletController : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    
    public void Initialize(Vector3 dir, float spd, float lifetime)
    {
        direction = dir;
        speed = spd;
        
        // Orient bullet to face movement direction
        transform.forward = direction;
        
        // Destroy after lifetime
        Destroy(gameObject, lifetime);
    }
    
    void Update()
    {
        // Move bullet manually instead of using physics
        transform.position += direction * speed * Time.deltaTime;
    }
    
    void OnCollisionEnter(Collision collision)
    {   
    // Check if the collided object has the specific tag
        if (collision.gameObject.CompareTag("Enemy")) 
        {
            Destroy(gameObject); // Destroy the bullet only if it hits an enemy
        }
    }

}