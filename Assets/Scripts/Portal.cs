using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string nextSceneName;
    public TMP_Text messageText;
    public string message = "Entering portal...";

    private bool isLoading;

    void OnTriggerEnter(Collider other)
    {
        if (isLoading || !IsPlayer(other.transform))
        {
            return;
        }

        isLoading = true;

        if (messageText != null)
        {
            messageText.text = message;
            messageText.gameObject.SetActive(true);
        }

        LoadNextScene();
    }

    void LoadNextScene()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrWhiteSpace(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("Portal has no next scene to load.");
            isLoading = false;
        }
    }

    bool IsPlayer(Transform current)
    {
        while (current != null)
        {
            if (current.CompareTag("Player"))
            {
                return true;
            }

            current = current.parent;
        }

        return false;
    }
}
