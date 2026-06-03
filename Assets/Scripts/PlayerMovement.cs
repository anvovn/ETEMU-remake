using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float turnSpeed = 12f;
    public float jumpForce = 7f;
    public float dashSpeed = 16f;
    public float dashDuration = 0.18f;
    public float dashCooldown = 0.8f;
    public float fallMultiplier = 2.5f;
    public Transform cameraTransform;
    public Vector3 rotationOffset;
    public TMP_Text dashIndicatorText;
    public bool createDashIndicator = true;
    public Color dashReadyColor = new Color(0.3f, 1f, 0.45f);
    public Color dashCooldownColor = new Color(1f, 0.85f, 0.25f);
    public Material dashStatusParticleMaterial;
    public ParticleSystem dashStatusParticles;
    public bool createDashStatusParticles = true;
    public Vector3 dashStatusParticlesOffset = new Vector3(0f, 0.2f, 0.25f);

    public AudioSource deathAudio;

    private Rigidbody rb;
    private bool isGrounded;
    private bool isDashing;

    private float verticalInput;
    private float horizontalInput;
    private float dashTimer;
    private float dashCooldownTimer;
    private Vector3 dashDirection;
    private bool dashStatusWasReady;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isGameOver;

    public GameObject gameOverText;
    public float gameOverResetDelay = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        startPosition = rb.position;
        startRotation = rb.rotation;

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (gameOverText != null)
        {
            gameOverText.SetActive(false);
        }

        SetupDashStatusEffect();
        UpdateDashStatusEffect(true);


    }

    void Update()
    {
        if (isGameOver)
        {
            return;
        }

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

        UpdateDashStatusEffect();
    }

    void FixedUpdate()
    {
        if (isGameOver)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        if (dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.fixedDeltaTime;
        }

        rb.angularVelocity = Vector3.zero;
        MovePlayer();
        ApplyExtraGravity();
        UpdateDashStatusEffect();
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
        UpdateDashStatusEffect(true);
    }

    void SetupDashStatusEffect()
    {
        DisableDashIndicatorText();

        if (dashStatusParticles == null)
        {
            dashStatusParticles = GetComponentInChildren<ParticleSystem>();
        }

        if (dashStatusParticles != null || !createDashStatusParticles)
        {
            return;
        }

        GameObject particleObject = new GameObject("DashStatusParticles");
        particleObject.transform.SetParent(transform, false);
        particleObject.transform.localPosition = dashStatusParticlesOffset;

        dashStatusParticles = particleObject.AddComponent<ParticleSystem>();
        ConfigureDashStatusParticles();
    }

    void DisableDashIndicatorText()
    {
        if (dashIndicatorText == null)
        {
            GameObject dashIndicatorObject = GameObject.Find("DashIndicatorText");

            if (dashIndicatorObject != null)
            {
                dashIndicatorText = dashIndicatorObject.GetComponent<TMP_Text>();
            }
        }

        if (dashIndicatorText != null)
        {
            dashIndicatorText.gameObject.SetActive(false);
        }
    }

    void ConfigureDashStatusParticles()
    {
        ParticleSystem.MainModule main = dashStatusParticles.main;
        main.loop = true;
        main.playOnAwake = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.8f, 1.35f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.08f, 0.28f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.22f, 0.48f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 80;

        ParticleSystem.EmissionModule emission = dashStatusParticles.emission;
        emission.enabled = true;
        emission.rateOverTime = 14f;

        ParticleSystem.ShapeModule shape = dashStatusParticles.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.25f;
        shape.radiusThickness = 1f;

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = dashStatusParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;

        ParticleSystemRenderer renderer = dashStatusParticles.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sortingOrder = 10;
        ApplyDashStatusParticleMaterial(renderer);
    }

    void UpdateDashStatusEffect(bool forceColorUpdate = false)
    {
        if (dashStatusParticles == null)
        {
            return;
        }

        ApplyDashStatusParticleMaterial(dashStatusParticles.GetComponent<ParticleSystemRenderer>());

        bool dashReady = !isDashing && dashCooldownTimer <= 0f;

        if (forceColorUpdate || dashReady != dashStatusWasReady)
        {
            SetDashStatusParticleColor(dashReady ? dashReadyColor : dashCooldownColor);
            dashStatusWasReady = dashReady;
        }

        if (!dashStatusParticles.isPlaying)
        {
            dashStatusParticles.Play();
        }
    }

    void SetDashStatusParticleColor(Color color)
    {
        ParticleSystem.MainModule main = dashStatusParticles.main;
        main.startColor = color;

        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = dashStatusParticles.colorOverLifetime;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(color, 0f),
                new GradientColorKey(color, 1f)
            },
            new[]
            {
                new GradientAlphaKey(0.25f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverLifetime.color = gradient;
    }

    void ApplyDashStatusParticleMaterial(ParticleSystemRenderer renderer)
    {
        if (renderer != null && dashStatusParticleMaterial != null && renderer.sharedMaterial != dashStatusParticleMaterial)
        {
            renderer.sharedMaterial = dashStatusParticleMaterial;
        }
    }

    public void RespawnAtStart()
    {
        if (gameObject.scene.buildIndex == 2)
        {
            deathAudio.Play();
        }

        isDashing = false;
        dashTimer = 0f;
        dashCooldownTimer = 0f;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = startPosition;
        rb.rotation = startRotation;

        transform.SetPositionAndRotation(startPosition, startRotation);
        Physics.SyncTransforms();
        UpdateDashStatusEffect(true);
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

    void ApplyExtraGravity()
    {
        if (rb.linearVelocity.y < 0f)
        {
            rb.AddForce(Physics.gravity * (fallMultiplier - 1f), ForceMode.Acceleration);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (IsGround(collision))
        {
            isGrounded = true;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (IsGround(collision))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (IsGround(collision))
        {
            isGrounded = false;
        }
    }

    bool IsGround(Collision collision)
    {
        Transform current = collision.transform;

        while (current != null)
        {
            if (current.CompareTag("Ground"))
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsEnemy(other.transform))
        {
            TriggerGameOver();
        }
    }

    bool IsEnemy(Transform current)
    {
        while (current != null)
        {
            if (current.CompareTag("Enemy")||current.CompareTag("Chair"))
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }

    void TriggerGameOver()
    {
        if (isGameOver)
        {
            return;
        }

        if (gameObject.scene.buildIndex == 1||gameObject.scene.buildIndex == 3)
        {
            deathAudio.Play();
        }

        isGameOver = true;
        isDashing = false;
        verticalInput = 0f;
        horizontalInput = 0f;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (gameOverText != null)
        {
            gameOverText.SetActive(true);
        }

        StartCoroutine(ResetSceneAfterGameOverDelay());
    }

    IEnumerator ResetSceneAfterGameOverDelay()
    {
        yield return new WaitForSecondsRealtime(gameOverResetDelay);

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
