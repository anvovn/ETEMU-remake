using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(SpawnObstacle), 1f, 3f);
    }

    void SpawnObstacle()
    {
        float xPos = Random.Range(-25f, 25f);
        float zPos = Random.Range(-25f, 25f);
        Vector3 spawnPosition = new Vector3(xPos, 2.5f, zPos);
        Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
    }
}
