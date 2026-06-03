using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float stoppingDistance = 1.5f;
    public Vector3 rotationOffset;
    
    public AudioSource chaseAudio;
    private float soundDelay = 3.0f;
    private float elapsedTime = 0f;

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

        elapsedTime += Time.deltaTime;
        if(distance <= 15.0f && elapsedTime >= soundDelay)
        {
            chaseAudio.Play();
            elapsedTime = 0f;
        }

        if (distance > stoppingDistance)
        {
            Vector3 moveDirection = direction.normalized;

            transform.position += moveDirection * moveSpeed * Time.deltaTime;

            transform.rotation = Quaternion.LookRotation(moveDirection) * Quaternion.Euler(rotationOffset);
        }
    }
}
