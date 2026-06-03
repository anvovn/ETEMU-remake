using UnityEngine;
using TMPro;
using System.Collections;

public class PersistentTimer : MonoBehaviour
{
    public static PersistentTimer Instance { get; private set; }

    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI achievementText;

    [Header("Speedrun Settings")]
    [Tooltip("Time threshold in seconds (e.g., 300 seconds = 5 minutes)")]
    [SerializeField] private float speedrunThreshold = 300f;
    [SerializeField] private string speedrunAchievementKey = "Fastest Duck in the North West";

    private float totalPlayTime = 0f;
    private bool isTimerRunning = false;

    private Coroutine achievementCoroutine;

    void Awake()
    {
        // Singleton Pattern: Keeps this exact UI and script alive across all scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // This makes this object (and its children, like the UI text) survive scene loads!
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        PlayerPrefs.DeleteAll();
        StartTimer();
        if (achievementText != null)
        {
            achievementText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isTimerRunning)
        {
            totalPlayTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    public void StartTimer()
    {
        totalPlayTime = 0f;
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(totalPlayTime / 60f);
        int seconds = Mathf.FloorToInt(totalPlayTime % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UnlockAchievement(string achievementKey)
    {
        if (PlayerPrefs.GetInt(achievementKey, 0) == 0)
        {
            PlayerPrefs.SetInt(achievementKey, 1);
            PlayerPrefs.Save();
            Debug.Log($"{achievementKey}!");
            string formattedName = achievementKey.Replace("_", " ");
            TriggerNotification(formattedName);
        }
    }

    public void CheckSpeedrunAchievement()
    {
        StopTimer();
        Debug.Log($"Game Completed in: {totalPlayTime:F2} seconds.");

        if (totalPlayTime <= speedrunThreshold)
        {
            UnlockAchievement(speedrunAchievementKey);
        }
    }

    private void TriggerNotification(string achievementName)
    {
        if (achievementText == null) return;
        if (achievementCoroutine != null)
        {
            StopCoroutine(achievementCoroutine);
        }
        achievementCoroutine = StartCoroutine(DisplayNotificationRoutine(achievementName));
    }
    // Change "iEnumeratior" to "IEnumerator"
    private IEnumerator DisplayNotificationRoutine(string achievementName)
    {
        // Set the text and turn the object on
        achievementText.text = $"ACHIEVEMENT UNLOCKED!\n<color=#FFD700>{achievementName}</color>";
        achievementText.gameObject.SetActive(true);

        // Wait completely in the background for 5 seconds
        yield return new WaitForSeconds(5f);

        // Turn the object back off
        achievementText.gameObject.SetActive(false);
    }
}