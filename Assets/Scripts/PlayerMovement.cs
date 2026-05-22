using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float turnSpeed = 12f;
    public float jumpForce = 7f;
    public float dashSpeed = 16f;
    public float dashDuration = 0.18f;
    public float dashCooldown = 0.8f;
    public Transform cameraTransform;
    public Vector3 rotationOffset;
    public TMP_Text dashIndicatorText;
    public bool createDashIndicator = true;
    public Color dashReadyColor = new Color(0.3f, 1f, 0.45f);
    public Color dashCooldownColor = new Color(1f, 0.85f, 0.25f);

    private Rigidbody rb;
    private bool isGrounded;
    private bool isDashing;

    private float verticalInput;
    private float horizontalInput;
    private float dashTimer;
    private float dashCooldownTimer;
    private Vector3 dashDirection;

    public GameObject gameOverText;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (gameOverText != null)
        {
            gameOverText.SetActive(false);
        }

        SetupDashIndicator();
        UpdateDashIndicator();
    }

    void Update()
    {
        verticalInput = 0f;
        horizontalInput = 0f;

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        if (keyboard.wKey.isPressed)
        {
            verticalInput += 1f;
        }

        if (keyboard.sKey.isPressed)
        {
            verticalInput -= 1f;
        }

        if (keyboard.aKey.isPressed)
        {
            horizontalInput -= 1f;
        }

        if (keyboard.dKey.isPressed)
        {
            horizontalInput += 1f;
        }

        if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
        {
            Jump();
        }

        if (keyboard.leftShiftKey.wasPressedThisFrame && dashCooldownTimer <= 0f)
        {
            StartDash();
        }

        UpdateDashIndicator();
    }

    void FixedUpdate()
    {
        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.fixedDeltaTime;
        }

        rb.angularVelocity = Vector3.zero;
        MovePlayer();
        UpdateDashIndicator();
    }

    void MovePlayer()
    {
        Vector3 movement = GetMovementDirection();

        if (isDashing)
        {
            dashTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = new Vector3(
                dashDirection.x * dashSpeed,
                rb.linearVelocity.y,
                dashDirection.z * dashSpeed
            );

            if (dashTimer <= 0f)
            {
                isDashing = false;
            }

            RotateToward(dashDirection);
            return;
        }

        RotateToward(movement);

        movement *= moveSpeed;

        Vector3 newVelocity = new Vector3(
            movement.x,
            rb.linearVelocity.y,
            movement.z
        );

        rb.linearVelocity = newVelocity;
    }

    Vector3 GetMovementDirection()
    {
        Vector3 cameraForward = Vector3.forward;
        Vector3 cameraRight = Vector3.right;

        if (cameraTransform != null)
        {
            cameraForward = cameraTransform.forward;
            cameraRight = cameraTransform.right;
        }

        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 movement = cameraForward * verticalInput + cameraRight * horizontalInput;
        return Vector3.ClampMagnitude(movement, 1f);
    }

    void RotateToward(Vector3 movement)
    {
        if (movement.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement) * Quaternion.Euler(rotationOffset);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime));
        }
    }

    void StartDash()
    {
        dashDirection = GetMovementDirection();

        if (dashDirection.sqrMagnitude <= 0.001f)
        {
            dashDirection = transform.forward;
            dashDirection.y = 0f;
            dashDirection.Normalize();
        }

        if (dashDirection.sqrMagnitude <= 0.001f)
        {
            dashDirection = Vector3.forward;
        }

        isDashing = true;
        dashTimer = dashDuration;
        dashCooldownTimer = dashCooldown;
        UpdateDashIndicator();
    }

    void SetupDashIndicator()
    {
        if (dashIndicatorText == null)
        {
            GameObject dashIndicatorObject = GameObject.Find("DashIndicatorText");

            if (dashIndicatorObject != null)
            {
                dashIndicatorText = dashIndicatorObject.GetComponent<TMP_Text>();
            }
        }

        if (dashIndicatorText != null || !createDashIndicator)
        {
            return;
        }

        Canvas canvas = FindObjectOfType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("Canvas", typeof(RectTransform));
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        GameObject textObject = new GameObject("DashIndicatorText", typeof(RectTransform));
        textObject.transform.SetParent(canvas.transform, false);

        dashIndicatorText = textObject.AddComponent<TextMeshProUGUI>();
        dashIndicatorText.alignment = TextAlignmentOptions.Center;
        dashIndicatorText.fontSize = 28f;
        dashIndicatorText.fontStyle = FontStyles.Bold;

        RectTransform rectTransform = dashIndicatorText.rectTransform;
        rectTransform.anchorMin = new Vector2(1f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 0f);
        rectTransform.pivot = new Vector2(1f, 0f);
        rectTransform.anchoredPosition = new Vector2(-32f, 32f);
        rectTransform.sizeDelta = new Vector2(220f, 50f);
    }

    void UpdateDashIndicator()
    {
        if (dashIndicatorText == null)
        {
            return;
        }

        bool dashReady = !isDashing && dashCooldownTimer <= 0f;
        dashIndicatorText.gameObject.SetActive(true);
        dashIndicatorText.text = dashReady ? "Dash Ready" : "Dash " + Mathf.CeilToInt(dashCooldownTimer * 10f) / 10f + "s";
        dashIndicatorText.color = dashReady ? dashReadyColor : dashCooldownColor;
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        isGrounded = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (gameOverText != null)
            {
                gameOverText.SetActive(true);
            }
        }
    }
}
