using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeycardPickup : MonoBehaviour
{
    public string promptMessage = "Press E to pick up keycard";

    private PlayerInventory nearbyInventory;
    private InteractionPromptManager promptManager;

    void Start()
    {
        promptManager = FindObjectOfType<InteractionPromptManager>();
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
            
            // Notify manager that a keycard was collected
            KeycardManager manager = KeycardManager.instance;
            if (manager != null)
            {
                manager.OnKeycardCollected();
            }
            
            UnregisterPrompt();
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
            RegisterPrompt();
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
            UnregisterPrompt();
        }
    }

    void RegisterPrompt()
    {
        if (promptManager != null)
        {
            promptManager.Register(this, promptMessage);
        }
    }

    void UnregisterPrompt()
    {
        if (promptManager != null)
        {
            promptManager.Unregister(this);
        }
    }
}
