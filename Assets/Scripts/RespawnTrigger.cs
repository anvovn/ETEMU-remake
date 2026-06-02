using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RespawnTrigger : MonoBehaviour
{
    void Reset()
    {
        SetColliderAsTrigger();
    }

    void OnValidate()
    {
        SetColliderAsTrigger();
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerMovement playerMovement = other.GetComponentInParent<PlayerMovement>();

        if (playerMovement != null)
        {
            playerMovement.RespawnAtStart();
        }
    }

    void SetColliderAsTrigger()
    {
        Collider triggerCollider = GetComponent<Collider>();

        if (triggerCollider != null)
        {
            triggerCollider.isTrigger = true;
        }
    }
}
