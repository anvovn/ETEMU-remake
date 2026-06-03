
using UnityEngine;

public class ObstacleBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //on start, set timer for 3 seconds. then spwan obstacle
        Invoke(nameof(SpawnObstacle), 3f);

    }

    void SpawnObstacle()
    {
        Vector3 spawnPosition = new Vector3(transform.position.x, 1f, transform.position.z);
        GameObject chair = Instantiate(obstaclePrefab, spawnPosition, Quaternion.Euler(270, 0, 0));
        chair.tag = "Chair";
        BoxCollider collider = chair.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        
        Destroy(gameObject);
    }
}
