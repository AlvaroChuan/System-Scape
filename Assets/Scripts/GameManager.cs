using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager instance;

    // Health management
    private int maxHP = 100;
    private int currentHP = 100;
    private float healthRegenTimeout = 5f;
    private int healthRegenAmmount = 1;
    private float healthRegenInterval = 0.5f;

    // Oxygen management
    private float maxOxygen = 60f;
    private float currentOxygen = 60f;
    private float oxygenUseRate = 0.5f;
    private float oxygenUseInterval = 1f;
    private float oxygenRegenAmmount = 15f;

    private void Awake()
    {
        if (GameManager.instance == null)
        {
            GameManager.instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
}
