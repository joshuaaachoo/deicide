using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this if you want to display health numbers

public class HealthSystem : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("UI References")]
    public Image healthBarFill;
    public Image healthBarBackground;
    public TextMeshProUGUI healthText; // Optional, for showing numbers
    
    [Header("Visual Settings")]
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;
    public float lowHealthThreshold = 0.3f; // Below 30% is low health
    public bool smoothHealthBar = true;
    public float smoothSpeed = 5f;
    
    // For smooth health bar
    private float targetFillAmount;

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
        targetFillAmount = 1f;
        
        // Update UI immediately
        UpdateHealthBar(false);
    }
    
    void Update()
    {
        // Smooth health bar animation if enabled
        if (smoothHealthBar && healthBarFill.fillAmount != targetFillAmount)
        {
            healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, targetFillAmount, Time.deltaTime * smoothSpeed);
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        // Clamp health to valid range
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        
        // Update UI
        UpdateHealthBar(smoothHealthBar);
        
        // Check for death
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(float amount)
    {
        currentHealth += amount;
        
        // Clamp health to valid range
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        
        // Update UI
        UpdateHealthBar(smoothHealthBar);
    }
    
    void UpdateHealthBar(bool smooth)
    {
        // Calculate fill amount (0-1 range)
        float fillAmount = currentHealth / maxHealth;
        
        if (smooth)
        {
            // Set target for smooth transition
            targetFillAmount = fillAmount;
        }
        else
        {
            // Update immediately
            healthBarFill.fillAmount = fillAmount;
            targetFillAmount = fillAmount;
        }
        
        // Update health text if available
        if (healthText != null)
        {
            healthText.text = Mathf.RoundToInt(currentHealth) + " / " + Mathf.RoundToInt(maxHealth);
        }
        
        // Change color based on health percentage
        if (fillAmount <= lowHealthThreshold)
        {
            healthBarFill.color = lowHealthColor;
        }
        else
        {
            // Optional: gradient between low and full health
            healthBarFill.color = Color.Lerp(lowHealthColor, fullHealthColor, (fillAmount - lowHealthThreshold) / (1 - lowHealthThreshold));
        }
    }
    
    void Die()
    {
        Debug.Log("Player died!");
        // Implement your game over logic here
        // For example:
        // GameManager.Instance.GameOver();
    }
    
    // Optional: Public method to get health percentage
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
}