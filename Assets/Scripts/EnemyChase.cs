using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float stoppingDistance = 1.5f;
    public Vector3 rotationOffset;

    private Transform player;

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        Vector3 direction = player.position - transform.position;

        // Keep the enemy moving only on the ground plane
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance > stoppingDistance)
        {
            Vector3 moveDirection = direction.normalized;

            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            transform.rotation = Quaternion.LookRotation(moveDirection) * Quaternion.Euler(rotationOffset);
        }
    }
}
