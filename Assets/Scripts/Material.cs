using System.Collections;
using DG.Tweening;
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
        float soundCounter = 0f;
        while (drillTime > 0)
        {
            drillTime -= Time.deltaTime;
            soundCounter += Time.deltaTime;
            if (soundCounter >= 0.5f)
            {
                SoundManager.instance.PlaySfx(SoundManager.ClipEnum.MaterailBreak, false);
                soundCounter = 0f;
                transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f, 10, 0.5f);
            }
            yield return null;
        }
        GameManager.instance.AddMaterial(materialName);
        GameManager.instance.drillableMaterials.Remove(this);
        transform.DOScale(Vector3.zero, 0.5f).OnComplete(DestroyMaterial);
    }

    private void DestroyMaterial()
    {
        Destroy(gameObject);
    }

    public float GetDrillTime()
    {
        return (originalDrillTime - drillTime) / originalDrillTime * 100f;
    }
}
