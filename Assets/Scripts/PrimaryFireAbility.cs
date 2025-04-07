using UnityEngine;

public class PrimaryFireAbility : AbilityBase
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public int maxAmmo = 7;
    
    private int currentAmmo;
    
    void Start()
    {
        currentAmmo = maxAmmo;
    }
    
    public override void ActivateAbility()
    {
        if (currentAmmo <= 0)
        {
            Reload();
            return;
        }
        
        // Create bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        
        // Add force to bullet
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = Vector3.zero; // Reset any existing velocity
            bulletRb.AddForce(firePoint.forward * bulletSpeed, ForceMode.VelocityChange);
        }
        
        // Destroy bullet after 2 seconds
        Destroy(bullet, 2f);
        
        // Reduce ammo
        currentAmmo--;
        
        // Special effect for seventh bullet
        if (currentAmmo == 0)
        {
            // Here you'd add code for the special seventh bullet effect
            Debug.Log("Fired special seventh bullet!");
        }
    }
    
    void Reload()
    {
        Debug.Log("Reloading...");
        // In a real implementation, you might want to add a delay here
        currentAmmo = maxAmmo;
    }
}