using UnityEngine;

public class FinalBossBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private BossBehaviour bossBehaviour;
    [SerializeField] private AudioSource audioSource;
    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BossDamager"))
        {
            audioSource.Play();
            bossBehaviour.takeDamage(11f);
            Destroy(other.gameObject);
        }
    }
}
