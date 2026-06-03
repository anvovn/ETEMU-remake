using UnityEngine;

public class FinalBossBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private BossBehaviour bossBehaviour;
    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BossDamager"))
        {
            bossBehaviour.takeDamage(7f);
            Destroy(other.gameObject);
        }
    }
}
