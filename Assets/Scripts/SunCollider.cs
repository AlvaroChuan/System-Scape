using UnityEngine;

public class SunCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) GameManager.instance.DamagePlayer(1000);
    }
}
