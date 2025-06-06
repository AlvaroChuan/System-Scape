using UnityEngine;

public class OxygenRangeController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag is "Spaceship" || other.gameObject.tag is "Spacereck") GameManager.instance.StartOxygenRegeneration();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag is "Spaceship" || other.gameObject.tag is "Spacereck") GameManager.instance.StopOxygenRegeneration();
    }
}
