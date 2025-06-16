using UnityEngine;

public class OxygenRangeController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag is "Spaceship" || other.gameObject.tag is "Spacereck") GameManager.instance.StartOxygenRegeneration();
        if (other.gameObject.tag is "Spaceship") PlayerController.instance.AllowSpaceshipMount(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag is "Spaceship" || other.gameObject.tag is "Spacereck") GameManager.instance.StopOxygenRegeneration();
        if (other.gameObject.tag is "Spaceship") PlayerController.instance.AllowSpaceshipMount(false);
    }
}
