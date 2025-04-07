using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBar;
    
    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            Die();
        }
        
        UpdateHealthBar();
    }
    
    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }
    }
    
    void Die()
    {
        Debug.Log("Player died!");
        // In a real game, you'd handle game over here
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Check if collision is with enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(10f); // Example damage value
        }
    }
}