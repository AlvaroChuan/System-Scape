using UnityEngine;

public class OxygenRangeController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag is "Spaceship" || other.gameObject.tag is "Spacereck") GameManager.instance.StartOxygenRegeneration();
        else if (other.gameObject.tag is "Material") GameManager.instance.drillableMaterials.Add(other.gameObject.GetComponent<Material>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag is "Spaceship" || other.gameObject.tag is "Spacereck") GameManager.instance.StopOxygenRegeneration();
        else if (other.gameObject.tag is "Material") GameManager.instance.drillableMaterials.Remove(other.gameObject.GetComponent<Material>());
    }
}
