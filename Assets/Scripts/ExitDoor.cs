using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ExitDoor : MonoBehaviour
{
    public TMP_Text interactionText;
    public GameObject winText;
    public string promptMessage = "Press E to exit";
    public string lockedMessage = "You need a keycard";
    public string nextSceneName = "level2";
    public string level1Achivement = "From Humble Beginnings: Clear Level 1";

    private PlayerInventory nearbyInventory;

    void Start()
    {
        SetPromptVisible(false);

        if (winText != null)
        {
            winText.SetActive(false);
        }
    }

    void OnEnable()
    {
        // Ensure win text is disabled when door becomes active
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
        KeycardManager keycardManager = KeycardManager.instance;
        
        if (keycardManager == null || !keycardManager.AreAllKeycardsCollected())
        {
            SetPromptText(lockedMessage);
            return;
        }

        SetPromptVisible(false);
        Time.timeScale = 1f;
        if (PersistentTimer.Instance != null)
        {
            PersistentTimer.Instance.UnlockAchievement(level1Achivement);
        }
        SceneManager.LoadScene(nextSceneName);
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
