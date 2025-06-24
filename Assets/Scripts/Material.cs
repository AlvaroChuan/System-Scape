using System.Collections;
using UnityEngine;

public class Material : MonoBehaviour
{
    [SerializeField] private int materialTier;
    public int MaterialTier => materialTier;
    [SerializeField] private string materialName;
    public string MaterialName => materialName;
    [SerializeField] private float drillTime = 3f;
    private Coroutine drillCoroutine;
    private float originalDrillTime = 3f;

    public void DrillMaterial()
    {
        originalDrillTime = drillTime; // Store the original drill time
        if (GameManager.instance.DrillGadgetTier < materialTier) return;
        if (drillCoroutine != null) StopCoroutine(drillCoroutine);
        drillCoroutine = StartCoroutine(DrillCoroutine());
    }

    public void StopDrilling()
    {
        if (drillCoroutine != null)
        {
            StopCoroutine(drillCoroutine);
            drillCoroutine = null;
        }
        drillTime = 3f; // Reset drill time
    }

    private IEnumerator DrillCoroutine()
    {
        while (drillTime > 0)
        {
            drillTime -= Time.deltaTime;
            yield return null;
        }
        GameManager.instance.AddMaterial(materialName);
        GameManager.instance.drillableMaterials.Remove(this);
        Destroy(gameObject);
    }

    public float GetDrillTime()
    {
        return (originalDrillTime - drillTime) / originalDrillTime * 100f;
    }
}
