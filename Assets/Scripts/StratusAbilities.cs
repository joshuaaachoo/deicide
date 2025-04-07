using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StratusAbilities : MonoBehaviour
{
    [Header("References")]
    public CharacterController controller;
    public Transform cameraTransform; // for direction
    public Transform tetherOrigin; // where the tether shoots from (e.g., hips)
    public LineRenderer tetherLine;

    [Header("Tether Settings")]
    public float tetherSpeed = 30f;
    public int maxTetherCharges = 2;
    public float tetherCooldown = 5f;
    public float tetherStopDistance = 1.5f;
    public float maxTetherDuration = 2f;

    [Header("Fray Settings")]
    public float frayDashSpeed = 50f;
    private bool canFray = false;
    private bool isFraying = false;

    [Header("Shatterstrike Settings")]
    public float meleeRange = 2f;
    public float meleeDamage = 10f;

    [Header("UI")]
    public Image tetherCooldownBar;
    public Image[] tetherChargeIcons;

    [HideInInspector] public int currentTetherCharges;
    [HideInInspector] public float tetherRechargeTimer;

    private bool isTethering = false;
    private Vector3 tetherTarget;
    private float tetherTimer = 0f;

    void Start()
    {
        currentTetherCharges = maxTetherCharges;

        if (tetherLine != null)
        {
            tetherLine.positionCount = 2;
            tetherLine.enabled = false;
        }
    }

    void Update()
    {
        HandleTetherRecharge();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryTether();
        }

        if (Input.GetMouseButtonDown(1) && canFray)
        {
            StartCoroutine(DoFray());
        }

        if (Input.GetMouseButtonDown(0))
        {
            DoShatterstrike();
        }

        if (isTethering)
        {
            // Cancel tether if player moves
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                StopTether();
            }
            else
            {
                HandleTetherMovement();
            }
        }

        UpdateTetherUI();
    }

    void TryTether()
    {
        if (currentTetherCharges <= 0) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            tetherTarget = hit.point;
            isTethering = true;
            tetherTimer = 0f;
            currentTetherCharges--;
            tetherRechargeTimer = 0f;
            canFray = false;

            if (tetherLine != null)
            {
                tetherLine.enabled = true;
                tetherLine.SetPosition(0, tetherOrigin.position);
                tetherLine.SetPosition(1, tetherTarget);
            }
        }
    }

    void HandleTetherMovement()
    {
        Vector3 toTarget = tetherTarget - transform.position;

        tetherTimer += Time.deltaTime;
        if (tetherTimer > maxTetherDuration || toTarget.magnitude <= tetherStopDistance ||
            Physics.Raycast(transform.position, toTarget.normalized, 1.5f))
        {
            StopTether();
            return;
        }

        if (tetherLine != null)
        {
            tetherLine.SetPosition(0, tetherOrigin.position);
            tetherLine.SetPosition(1, tetherTarget);
        }

        Vector3 moveDir = toTarget.normalized;
        controller.Move(moveDir * tetherSpeed * Time.deltaTime);
    }

    void StopTether()
    {
        isTethering = false;
        canFray = true;

        if (tetherLine != null)
        {
            tetherLine.enabled = false;
        }
    }

    void HandleTetherRecharge()
    {
        if (currentTetherCharges < maxTetherCharges)
        {
            tetherRechargeTimer += Time.deltaTime;
            if (tetherRechargeTimer >= tetherCooldown)
            {
                currentTetherCharges++;
                tetherRechargeTimer = 0f;
            }
        }
    }

    IEnumerator DoFray()
    {
        isFraying = true;

        Vector3 dashDir = transform.forward;
        float dashTime = 0.3f;
        float timer = 0f;

        while (timer < dashTime)
        {
            controller.Move(dashDir * frayDashSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        isFraying = false;
        canFray = false;
    }

    void DoShatterstrike()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, meleeRange))
        {
            Debug.Log("Shatterstrike hit: " + hit.collider.name);
            // Damage logic here
        }
    }

    void UpdateTetherUI()
    {
        if (tetherCooldownBar != null)
        {
            tetherCooldownBar.fillAmount = currentTetherCharges < maxTetherCharges
                ? tetherRechargeTimer / tetherCooldown
                : 1f;
        }

        if (tetherChargeIcons != null && tetherChargeIcons.Length > 0)
        {
            for (int i = 0; i < tetherChargeIcons.Length; i++)
            {
                tetherChargeIcons[i].enabled = (i < currentTetherCharges);
            }
        }
    }
}
