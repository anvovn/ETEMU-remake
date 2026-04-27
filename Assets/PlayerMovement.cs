using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float turnSpeed = 120f;
    public float jumpForce = 7f;

    private Rigidbody rb;
    private bool isGrounded;

    private float moveInput;
    private float turnInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        moveInput = 0f;
        turnInput = 0f;

        if (keyboard.wKey.isPressed)
        {
            moveInput += 1f;
        }

        if (keyboard.sKey.isPressed)
        {
            moveInput -= 1f;
        }

        if (keyboard.aKey.isPressed)
        {
            turnInput -= 1f;
        }

        if (keyboard.dKey.isPressed)
        {
            turnInput += 1f;
        }

        if (keyboard.spaceKey.wasPressedThisFrame && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        TurnPlayer();
        MovePlayer();
    }

    void TurnPlayer()
    {
        float turnAmount = turnInput * turnSpeed * Time.fixedDeltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);

        rb.MoveRotation(rb.rotation * turnRotation);
    }

    void MovePlayer()
    {
        Vector3 forwardMovement = transform.forward * moveInput * moveSpeed;

        Vector3 newVelocity = new Vector3(
            forwardMovement.x,
            rb.linearVelocity.y,
            forwardMovement.z
        );

        rb.linearVelocity = newVelocity;
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
}