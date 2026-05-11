using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeycardPickup : MonoBehaviour
{
    public TMP_Text interactionText;
    public string promptMessage = "Press E to pick up keycard";

    private PlayerInventory nearbyInventory;

    void Start()
    {
        SetPromptVisible(false);
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
            nearbyInventory.AddKeycard();
            SetPromptVisible(false);
            gameObject.SetActive(false);
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

    void SetPromptVisible(bool visible)
    {
        if (interactionText != null)
        {
            interactionText.text = promptMessage;
            interactionText.gameObject.SetActive(visible);
        }
    }
}
