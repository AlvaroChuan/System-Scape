using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    // Singleton
    public static HUDManager instance;
    [SerializeField] private Text oxygenText;
    [SerializeField] private Text healthText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Update()
    {
        oxygenText.text = $"Oxygen: {GameManager.instance.OxygenLevel:F1} / {GameManager.instance.MaxOxygen:F1}";
        healthText.text = $"Health: {GameManager.instance.CurrentHP:F1} / {GameManager.instance.MaxHP:F1}";
    }
}
