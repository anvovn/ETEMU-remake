using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ControlGrantSign : MonoBehaviour
{
    public bool grantOnlyOnce = true;
    public bool enablePlayerCamera = true;
    public bool deactivateAfterGrant;
    public bool showControlsMessage = true;
    public TMP_Text controlsText;
    public string controlsMessage = "WASD: Move\nMouse: Look\nSpace: Jump\nLeft Shift: Dash";
    public float messageDuration = 4f;

    private bool hasGrantedControls;
    private Coroutine hideMessageCoroutine;

    void Start()
    {
        if (controlsText == null)
        {
            InteractionPromptManager promptManager = FindFirstObjectByType<InteractionPromptManager>();

            if (promptManager != null)
            {
                controlsText = promptManager.interactionText;
            }
        }
    }

    void Reset()
    {
        SetColliderAsTrigger();
    }

    void OnValidate()
    {
        SetColliderAsTrigger();
    }

    void OnTriggerEnter(Collider other)
    {
        if (grantOnlyOnce && hasGrantedControls)
        {
            return;
        }

        PlayerMovement playerMovement = other.GetComponentInParent<PlayerMovement>();

        if (playerMovement == null)
        {
            return;
        }

        playerMovement.enabled = true;

        if (enablePlayerCamera)
        {
            EnableCameraControl(playerMovement.transform);
        }

        if (showControlsMessage)
        {
            ShowControlsMessage();
        }

        hasGrantedControls = true;

        if (deactivateAfterGrant)
        {
            gameObject.SetActive(false);
        }
    }

    void EnableCameraControl(Transform playerTransform)
    {
        ThirdPersonCamera cameraControl = playerTransform.GetComponentInChildren<ThirdPersonCamera>(true);

        if (cameraControl == null && Camera.main != null)
        {
            cameraControl = Camera.main.GetComponent<ThirdPersonCamera>();
        }

        if (cameraControl == null)
        {
            return;
        }

        if (cameraControl.target == null)
        {
            cameraControl.target = playerTransform;
        }

        cameraControl.enabled = true;
    }

    void ShowControlsMessage()
    {
        if (controlsText == null)
        {
            return;
        }

        controlsText.text = controlsMessage;
        controlsText.gameObject.SetActive(true);

        if (hideMessageCoroutine != null)
        {
            StopCoroutine(hideMessageCoroutine);
        }

        if (messageDuration > 0f)
        {
            hideMessageCoroutine = StartCoroutine(HideControlsMessageAfterDelay());
        }
    }

    IEnumerator HideControlsMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDuration);

        if (controlsText != null)
        {
            controlsText.gameObject.SetActive(false);
        }

        hideMessageCoroutine = null;
    }

    void SetColliderAsTrigger()
    {
        Collider triggerCollider = GetComponent<Collider>();

        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
    }
}
