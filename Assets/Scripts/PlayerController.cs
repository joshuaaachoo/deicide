using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("UI")]
    public Image tetherBar;
    public Image[] tetherChargeIcons;

    [Header("Movement Settings")]
    public float walkSpeed = 15f;
    public float dashMultiplier;
    public float dashTime;
    public float dashCoolDown;
    public float jumpForce = 13f;
    public float gravity = -20f;
    public float mouseSensitivity = 2f;

    enum DashState
    {
        ready, // can be used
        active, // is being used
        cooldown // can't be used
    }
    DashState state = DashState.ready;

    [Header("References")]
    public Transform cameraTransform;
    public CharacterController controller;
    public StratusAbilities abilities; // ✅ Reference to tether logic
    // public List<AbilityHolder> abilities = new List<AbilityHolder>(3); // 0 1 2 = 2ndFire Mobi Misc respectively

    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBarFill;
    public Color fullHealthColor = Color.green;
    public Color lowHealthColor = Color.red;

    [Header("Knockback Settings")]
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.25f;

    private float verticalVelocity;
    private float verticalLookRotation;
    private bool isDead = false;
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private Vector3 knockbackVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null) cameraTransform = cam.transform;
        }

        if (controller == null)
        {
            controller = GetComponent<CharacterController>();
        }

        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    void Update()
    {
        if (isDead) return;

        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            controller.Move(knockbackVelocity * Time.deltaTime);
            if (knockbackTimer <= 0)
            {
                isKnockedBack = false;
            }
            return;
        }

        HandleMouseLook();
        HandleMovement();
        UpdateTetherUI(); // ✅ Call UI update every frame

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorLock();
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalLookRotation -= mouseY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        if (cameraTransform != null)
        {
            cameraTransform.localEulerAngles = Vector3.right * verticalLookRotation;
        }
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        float speed = walkSpeed;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move = move.normalized * speed;

        // Jumping
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f; // stick to ground
        }

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            verticalVelocity = jumpForce;
        }

        // Apply gravity
        verticalVelocity += gravity * Time.deltaTime;
        move.y = verticalVelocity;

        controller.Move(move * Time.deltaTime);

        // Dashing
        switch(state)
        {
            case DashState.ready:
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    // use dash + make it active
                    Vector3 dashVector = new Vector3(moveX * dashMultiplier, 0, moveZ * dashMultiplier);
                    controller.Move(dashVector * dashTime);
                    state = DashState.active;
                    dashTime = 0.2f;
                }
                break;
            case DashState.active:
                if (dashTime > 0) { dashTime -= Time.deltaTime; }
                else
                {
                    state = DashState.cooldown;
                    dashCoolDown = 0.3f;
                }
                break;
            case DashState.cooldown: // this could also be default
                if (dashCoolDown > 0) { dashCoolDown -= Time.deltaTime; }
                else
                {
                    state = DashState.ready;
                }
                break;
        }
    }

    void ToggleCursorLock()
    {
        Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked)
            ? CursorLockMode.None
            : CursorLockMode.Locked;
        Cursor.visible = !Cursor.visible;
    }

    void UpdateTetherUI()
    {
        if (abilities == null) return;

        // Cooldown fill bar
        if (tetherBar != null && abilities.currentTetherCharges < abilities.maxTetherCharges)
        {
            tetherBar.fillAmount = abilities.tetherRechargeTimer / abilities.tetherCooldown;
        }
        else if (tetherBar != null)
        {
            tetherBar.fillAmount = 1f;
        }

        // Charge icons
        if (tetherChargeIcons != null && tetherChargeIcons.Length > 0)
        {
            for (int i = 0; i < tetherChargeIcons.Length; i++)
            {
                tetherChargeIcons[i].enabled = (i < abilities.currentTetherCharges);
            }
        }
    }

    // ===== HEALTH & KNOCKBACK =====

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            float pct = (float)currentHealth / maxHealth;
            healthBarFill.fillAmount = pct;
            healthBarFill.color = Color.Lerp(lowHealthColor, fullHealthColor, pct);
        }
    }

    void Die()
    {
        isDead = true;
        verticalVelocity = 0f;
        Debug.Log("Player died.");
        // Optional: Play animation, game over screen, etc.
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
        knockbackVelocity = direction.normalized * force;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(10f);
            Vector3 knockbackDir = transform.position - hit.transform.position;
            knockbackDir.y = 0.3f;
            ApplyKnockback(knockbackDir, knockbackForce);
        }
    }
}
