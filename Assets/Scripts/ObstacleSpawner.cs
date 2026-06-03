using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject keycardPrefab;
    public GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(SpawnObstacle), 1f, 3f);
        InvokeRepeating(nameof(SpawnKeycard), 2f, 6f);
    }

    void SpawnObstacle()
    {
        float xPos = Random.Range(-25f, 25f);
        float zPos = Random.Range(-25f, 25f);
        Vector3 spawnPosition = new Vector3(xPos, 2.5f, zPos);
        Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
    }
    void SpawnKeycard()
    {

        float xPos = Random.Range(-20f, 20f);
        float zPos = Random.Range(-20f, 20f);
        while(Vector3.Distance(new Vector3(xPos, 0f, zPos), player.transform.position) < 5f)
        {
            xPos = Random.Range(-20f, 20f);
            zPos = Random.Range(-20f, 20f);
        }
        Vector3 spawnPosition = new Vector3(xPos, 2.5f, zPos);
        Instantiate(keycardPrefab, spawnPosition, Quaternion.identity);
    }
}
