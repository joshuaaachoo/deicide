using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public float fireRate = 0.2f;
    
    private float nextFireTime = 0f;
    
    void Update()
    {
        // Fire weapon on left mouse click
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }
    
    void Fire()
    {
        // Create bullet at fire point
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = Vector3.zero; // Reset any existing velocity
                bulletRb.AddForce(firePoint.forward * bulletSpeed, ForceMode.VelocityChange);
            }
            
            // Destroy bullet after 2 seconds
            Destroy(bullet, 2f);
        }
    }
}