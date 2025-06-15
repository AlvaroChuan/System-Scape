using UnityEngine;

public class AimRangeController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag is "Enemy") GameManager.instance.enemiesInMaxRange.Add(other.gameObject.GetComponent<Enemy>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag is "Enemy") GameManager.instance.enemiesInMaxRange.Remove(other.gameObject.GetComponent<Enemy>());
    }
}
