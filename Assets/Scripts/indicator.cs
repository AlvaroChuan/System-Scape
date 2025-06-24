using UnityEngine;

public class Indicator : MonoBehaviour
{
    private Transform cameraTransform;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Vector2 originalSize = new Vector2(1f, 1f);
    private float fillAmount = 0;
    private Transform childTransform;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        childTransform = transform.GetChild(0);
        originalSize = spriteRenderer.size;
    }

    private void LateUpdate()
    {
        childTransform.LookAt(childTransform.position + cameraTransform.forward);
    }

    public void SetFillAmount(float amount)
    {
        fillAmount = Mathf.Clamp(amount, 0, 100);
        float fillRatio = fillAmount / 100f;
        spriteRenderer.size = new Vector2(originalSize.x * fillRatio, originalSize.y);
    }

}
