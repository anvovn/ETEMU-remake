
using UnityEngine;

public class ObstacleBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //on start, set timer for 3 seconds. then spwan obstacle
        Invoke(nameof(makeEnemy), 2.5f);
        Invoke(nameof(SpawnObstacle), 3f);

    }
    void makeEnemy()
    {

        gameObject.tag = "Enemy";
        
    }
    void SpawnObstacle()
    {
        Vector3 spawnPosition = new Vector3(transform.position.x, 1f, transform.position.z);
        Instantiate(obstaclePrefab, spawnPosition, Quaternion.Euler(270, 0, 0));
        Destroy(gameObject);
    }


}
