using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 targetOffset = new Vector3(0f, 1.6f, 0f);
    public float distance = 5f;
    public float mouseSensitivity = 0.12f;
    public float followSpeed = 12f;
    public float minPitch = -25f;
    public float maxPitch = 65f;

    private float yaw;
    private float pitch = 20f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Mouse mouse = Mouse.current;

        if (mouse != null)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();
            yaw += mouseDelta.x * mouseSensitivity;
            pitch -= mouseDelta.y * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        Quaternion cameraRotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 targetPosition = target.position + targetOffset;
        Vector3 wantedPosition = targetPosition - cameraRotation * Vector3.forward * distance;
        float followAmount = 1f - Mathf.Exp(-followSpeed * Time.deltaTime);

        transform.position = Vector3.Lerp(
            transform.position,
            wantedPosition,
            followAmount
        );

        transform.LookAt(targetPosition);
    }
}
