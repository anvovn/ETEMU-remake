using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class BossBehaviour : MonoBehaviour
{

    [SerializeField] private GameObject crosshairPrefab;
    [SerializeField] private float baseHeight;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text healthText;
    private float health = 100f;
    private bool passed50 = false;
    private Vector3 currentPosition;
    public string level3Achievement = "Down Goes Fredrick: Complete Level 3";


    private float xPos;
    private float yPos;

    void Start()
    {    
        currentPosition = transform.position;
        InvokeRepeating("spawnLandPoint", 2f, 8f);
    }

    //X range = -30 to 30
    //Y range = -30 to 30
    // Update is called once per frame
    public void takeDamage(float damage)
    {
        health -= damage;
        slider.value = 1-(health/100f);
        healthText.text = health.ToString("0") + "/100";
        if(health < 50 && !passed50)
        {
            passed50 = true;
            GameObject[] chairs = GameObject.FindGameObjectsWithTag("Chair");
            foreach (GameObject chair in chairs)
            {
                Destroy(chair);
            }
        }
        if (health <= 0)
        {
            triggerDeath();
        }
    }

    void triggerDeath()
    {
        // Play death animation or effects here
            PersistentTimer.Instance.UnlockAchievement(level3Achievement);
            PersistentTimer.Instance.CheckSpeedrunAchievement();
            Debug.Log("Boss defeated!");
            Destroy(gameObject); // Remove the boss from the scene
            SceneManager.LoadScene("WinScreen");
    }
    void spawnLandPoint()
    {
        Debug.Log("Spawning crosshair");    
        xPos = Random.Range(-30f, 30f);
        yPos = Random.Range(-30f, 30f);
        Vector3 spawnPos = new Vector3(xPos, 0.81f, yPos);
        GameObject crosshair = Instantiate(crosshairPrefab, spawnPos, Quaternion.identity);
        StartCoroutine(MoveToTarget(2f, spawnPos));
        Destroy(crosshair, 5f);
    }

    IEnumerator MoveToTarget(float duration, Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Debug.Log("Moving to target XZ");
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(currentPosition, targetPosition, elapsedTime / duration);
            transform.position = new Vector3(transform.position.x, baseHeight, transform.position.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Moving to target Y");
        elapsedTime = 0f;
        duration /= 2f;
        duration +=0.5f;

        while (elapsedTime < duration)
        {
            float ypos = Mathf.SmoothStep(baseHeight, -5f, elapsedTime / duration);
            transform.position = new Vector3(targetPosition.x, ypos, targetPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float ypos = Mathf.SmoothStep(-5f, baseHeight, elapsedTime / duration);
            transform.position = new Vector3(targetPosition.x, ypos, targetPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        targetPosition = new Vector3(targetPosition.x, baseHeight, targetPosition.z);
        currentPosition = targetPosition;
        
    }
}
