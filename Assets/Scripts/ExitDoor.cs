using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExitDoor : MonoBehaviour
{
    public TMP_Text interactionText;
    public GameObject winText;
    public string promptMessage = "Press E to exit";
    public string lockedMessage = "You need a keycard";
    public bool pauseGameOnWin = true;

    private PlayerInventory nearbyInventory;

    void Start()
    {
        SetPromptVisible(false);

        if (winText != null)
        {
            winText.SetActive(false);
        }
    }

    void Update()
    {
        if (nearbyInventory == null)
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;

        if (keyboard != null && keyboard.eKey.wasPressedThisFrame)
        {
            TryExit();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        nearbyInventory = other.GetComponent<PlayerInventory>();

        if (nearbyInventory != null)
        {
            SetPromptText(promptMessage);
            SetPromptVisible(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerInventory exitingInventory = other.GetComponent<PlayerInventory>();

        if (exitingInventory == nearbyInventory)
        {
            nearbyInventory = null;
            SetPromptVisible(false);
        }
    }

    void TryExit()
    {
        if (!nearbyInventory.HasKeycard())
        {
            SetPromptText(lockedMessage);
            return;
        }

        SetPromptVisible(false);

        if (winText != null)
        {
            winText.SetActive(true);
        }

        PlayerMovement playerMovement = nearbyInventory.GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (pauseGameOnWin)
        {
            Time.timeScale = 0f;
        }
    }

    void SetPromptVisible(bool visible)
    {
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(visible);
        }
    }

    void SetPromptText(string message)
    {
        if (interactionText != null)
        {
            interactionText.text = message;
        }
    }
}
