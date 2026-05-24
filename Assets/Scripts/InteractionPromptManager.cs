using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class InteractionPromptManager : MonoBehaviour
{
    public TMP_Text interactionText;
    private HashSet<KeycardPickup> activePrompts = new HashSet<KeycardPickup>();

    void Start()
    {
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    public void Register(KeycardPickup keycard, string message)
    {
        if (!activePrompts.Contains(keycard))
        {
            activePrompts.Add(keycard);
        }

        if (activePrompts.Count > 0)
        {
            ShowPrompt(message);
        }
    }

    public void Unregister(KeycardPickup keycard)
    {
        activePrompts.Remove(keycard);

        if (activePrompts.Count == 0)
        {
            HidePrompt();
        }
    }

    void ShowPrompt(string message)
    {
        if (interactionText != null)
        {
            interactionText.text = message;
            interactionText.gameObject.SetActive(true);
        }
    }

    void HidePrompt()
    {
        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }
}
