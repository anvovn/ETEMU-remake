using UnityEngine;
using TMPro;
using System.Collections;

public class KeycardManager : MonoBehaviour
{
    public static KeycardManager instance;
    public TMP_Text completionMessage;
    public float messageDisplayDuration = 3f; // How long to show the message
    
    private int totalKeycards;
    private int collectedKeycards;
    private GameObject exitDoor; // Cache the door reference

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Count total keycards in scene
        KeycardPickup[] keycards = FindObjectsOfType<KeycardPickup>();
        totalKeycards = keycards.Length;
        Debug.Log("Total keycards in scene: " + totalKeycards);
        
        if (completionMessage != null)
        {
            completionMessage.gameObject.SetActive(false);
        }
        
        // Find and cache the exit door (do this while it's still active!)
        exitDoor = GameObject.FindWithTag("ExitDoor");
        if (exitDoor != null)
        {
            exitDoor.SetActive(false);
        }
    }

    public void OnKeycardCollected()
    {
        collectedKeycards++;
        Debug.Log("Keycard collected! " + collectedKeycards + " / " + totalKeycards);
        
        if (collectedKeycards >= totalKeycards && totalKeycards > 0)
        {
            AllKeycardsCollected();
        }
    }
    
    public bool AreAllKeycardsCollected()
    {
        return collectedKeycards >= totalKeycards && totalKeycards > 0;
    }

    void AllKeycardsCollected()
    {
        Debug.Log("All keycards collected!");
        
        // Show completion message
        if (completionMessage != null)
        {
            completionMessage.text = "All keycards collected! Find the exit!";
            completionMessage.gameObject.SetActive(true);
            
            // Hide message after delay
            StartCoroutine(HideMessageAfterDelay());
        }
        
        // Show exit door (use cached reference)
        if (exitDoor != null)
        {
            exitDoor.SetActive(true);
        }
    }
    
    IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDisplayDuration);
        
        if (completionMessage != null)
        {
            completionMessage.gameObject.SetActive(false);
        }
    }
}
