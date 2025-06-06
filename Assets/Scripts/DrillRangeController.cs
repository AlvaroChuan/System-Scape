using UnityEngine;

public class DrillRangeController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag is "Material") GameManager.instance.drillableMaterials.Add(other.gameObject.GetComponent<Material>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag is "Material") GameManager.instance.drillableMaterials.Remove(other.gameObject.GetComponent<Material>());
    }
}
