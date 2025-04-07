using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 2f;
    
    [Header("References")]
    public Transform cameraTransform;
    
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public UnityEngine.UI.Image healthBarFill;
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;
    
    [Header("Knockback Settings")]
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.25f;
    
    private Rigidbody rb;
    private float verticalLookRotation;
    private bool isGrounded;
    private Vector3 moveDirection;
    private bool isDead = false;
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Find camera if not assigned
        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null)
                cameraTransform = cam.transform;
        }
        
        // Initialize health
        currentHealth = maxHealth;
        UpdateHealthBar();
    }
    
    void Update()
    {
        // Don't process input if player is dead
        if (isDead) return;
        
        // Handle knockback timer
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
            {
                isKnockedBack = false;
            }
            return; // Skip regular movement while knocked back
        }
        
        // Check if grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
        
        // Handle mouse look
        HandleMouseLook();
        
        // Get movement input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        // Determine if running
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        
        // Calculate move direction relative to where we're looking
        moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;
        moveDirection *= currentSpeed;
        
        // Handle jumping
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        
        // Toggle cursor lock with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorLock();
        }
    }
    
    void FixedUpdate()
    {
        // Skip regular movement during knockback
        if (isKnockedBack || isDead) return;
        
        // Apply movement
        rb.linearVelocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
    }
    
    void HandleMouseLook()
    {
        // Horizontal rotation (player body turns)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
        
        // Vertical rotation (camera looks up/down)
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalLookRotation -= mouseY; // Subtract because mouse Y is inverted
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f); // Limit look angle
        
        if (cameraTransform != null)
        {
            cameraTransform.localEulerAngles = Vector3.right * verticalLookRotation;
        }
    }
    
    void ToggleCursorLock()
    {
        Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = !Cursor.visible;
    }
    
    // Health related methods
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        // Clamp health to valid range
//        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        
        // Update UI
        UpdateHealthBar();
        
        // Check for death
        if (currentHealth <= 0 && !isDead)
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
        UpdateHealthBar();
    }
    
    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            // Update fill amount
            healthBarFill.fillAmount = currentHealth / maxHealth;
            
            // Update color based on health percentage
            float healthPercentage = currentHealth / maxHealth;
            healthBarFill.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercentage);
        }
    }
    
    void Die()
    {
        isDead = true;
        Debug.Log("Player died!");
        
        // Disable movement
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        // You can add more death effects here
        // Example: Play death animation, show game over screen, etc.
    }
    
    public void ApplyKnockback(Vector3 direction, float force)
    {
        // Start knockback
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
        
        // Apply force
        rb.linearVelocity = Vector3.zero; // Clear current velocity
        rb.AddForce(direction.normalized * force, ForceMode.Impulse);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Check if collision is with enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Take damage
            TakeDamage(10f); // Take 10 damage when hit by enemy
            
            // Calculate knockback direction (away from enemy)
            Vector3 knockbackDir = transform.position - collision.transform.position;
            knockbackDir.y = 0.3f; // Add slight upward force
            
            // Apply knockback
            ApplyKnockback(knockbackDir, knockbackForce);
        }
    }
}