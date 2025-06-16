using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class HUDManager : MonoBehaviour
{
    // Singleton
    public static HUDManager instance;
    [SerializeField] private Text oxygenText;
    [SerializeField] private Text healthText;

    [Header("Pause HUD")]
    [SerializeField] private GameObject PauseHUD;
    [SerializeField] private Button resumeButton; // Reference to set as the selected button for the event system
    [SerializeField] private Button backButton; // Reference to set as the selected button for the event sysstem
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Text tabText;

    [Header("Upgrades HUD")]
    [SerializeField] private GameObject UpgradesHUD;
    [SerializeField] private Color panelColor;
    [SerializeField] private Color headerColor;
    [SerializeField] private Button equipmentButton;
    [SerializeField] private Button spaceshipButton;
    [SerializeField] private Button gadgetsButton;
    [SerializeField] private GameObject equipmentPanel;
    [SerializeField] private GameObject spaceshipPanel;
    [SerializeField] private GameObject gadgetsPanel;

    [Header("Selection Rect")]
    [SerializeField] private Image selectionRect;


    private GameObject currentPanel;
    private int currentSubPanelIndex = 0;
    private bool onUI = false;

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
        if (onUI && EventSystem.current.currentSelectedGameObject != null)
        {
            if (EventSystem.current.currentSelectedGameObject.transform.parent != null && EventSystem.current.currentSelectedGameObject.name == "Button")
            {
                selectionRect.rectTransform.anchorMin = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<RectTransform>().anchorMin;
                selectionRect.rectTransform.anchorMax = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<RectTransform>().anchorMax;
                selectionRect.rectTransform.pivot = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<RectTransform>().pivot;
                selectionRect.rectTransform.position = Vector3.Lerp(selectionRect.rectTransform.position, EventSystem.current.currentSelectedGameObject.transform.parent.position, Time.fixedDeltaTime * 10f);
                selectionRect.rectTransform.sizeDelta = Vector3.Lerp(selectionRect.rectTransform.sizeDelta, EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<RectTransform>().sizeDelta, Time.fixedDeltaTime * 10f);
            }
            else
            {
                selectionRect.rectTransform.anchorMin = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().anchorMin;
                selectionRect.rectTransform.anchorMax = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().anchorMax;
                selectionRect.rectTransform.pivot = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().pivot;
                selectionRect.rectTransform.position = Vector3.Lerp(selectionRect.rectTransform.position, EventSystem.current.currentSelectedGameObject.transform.position, Time.fixedDeltaTime * 10f);
                selectionRect.rectTransform.sizeDelta = Vector3.Lerp(selectionRect.rectTransform.sizeDelta, EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().sizeDelta, Time.fixedDeltaTime * 10f);
            }
        }
    }

    public void ToggleHUD(bool state, string type)
    {
        UpgradesHUD.SetActive(false);
        PauseHUD.SetActive(false);
        switch (type)
        {
            case "Upgrades":
                UpgradesHUD.SetActive(state);
                if (state) EventSystem.current.SetSelectedGameObject(equipmentButton.gameObject);
                ToggleUpgradesPanel(equipmentPanel);
                break;
            case "Pause":
                PauseHUD.SetActive(state);
                musicSlider.value = SoundManager.instance.GetMusicVolume();
                sfxSlider.value = SoundManager.instance.GetSFXVolume();
                if (state) EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
                break;
        }
        onUI = state;
        selectionRect.gameObject.SetActive(state);
    }

    public void ToggleUpgradesPanel(GameObject panel)
    {
        GameObject[] panelList = new GameObject[] { equipmentPanel, spaceshipPanel, gadgetsPanel };
        Button[] buttonList = new Button[] { equipmentButton, spaceshipButton, gadgetsButton };
        foreach (GameObject go in panelList) go.SetActive(go == panel);
        foreach (Button btn in buttonList)
        {
            btn.image.color = headerColor;
            btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = panelColor;
        }
        switch (panel)
        {
            case GameObject go when go == equipmentPanel:
                equipmentButton.image.color = panelColor;
                equipmentButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
                break;
            case GameObject go when go == spaceshipPanel:
                spaceshipButton.image.color = panelColor;
                spaceshipButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
                break;
            case GameObject go when go == gadgetsPanel:
                gadgetsButton.image.color = panelColor;
                gadgetsButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
                break;
        }
        currentPanel = panel;
    }

    public void NextUpgradesSubPanel()
    {
        if (currentPanel == null) return;
        List<GameObject> panelList = new List<GameObject>();
        currentSubPanelIndex++;
        for (int i = 0; i < currentPanel.transform.childCount; i++) panelList.Add(currentPanel.transform.GetChild(i).gameObject);
        foreach (GameObject go in panelList) go.SetActive(false);
        if (currentSubPanelIndex >= panelList.Count) currentSubPanelIndex = 0;
        panelList[currentSubPanelIndex].SetActive(true);
    }

    public void PreviousUpgradesSubPanel()
    {
        if (currentPanel == null) return;
        List<GameObject> panelList = new List<GameObject>();
        currentSubPanelIndex--;
        for (int i = 0; i < currentPanel.transform.childCount; i++) panelList.Add(currentPanel.transform.GetChild(i).gameObject);
        foreach (GameObject go in panelList) go.SetActive(false);
        if (currentSubPanelIndex < 0) currentSubPanelIndex = panelList.Count - 1;
        panelList[currentSubPanelIndex].SetActive(true);
    }

    public void BuyUpgrade(Upgrade upgrade)
    {
        GameObject mask = EventSystem.current.currentSelectedGameObject.transform.parent.GetChild(1).gameObject;
        int result = GameManager.instance.ApplyUpgrade(upgrade);
        if (result == 0) mask.SetActive(false);
        else if (result == 1) Debug.Log("Upgrade already purchased: " + upgrade.upgradeName);
        else if (result == 2) Debug.Log("Prerequisites not met");
        else if (result == 3) Debug.Log("Not enough resources to purchase upgrade: " + upgrade.upgradeName);
    }

    public void OpenOptionsPanel()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
        tabText.text = "Options";
        EventSystem.current.SetSelectedGameObject(backButton.gameObject);
    }

    public void CloseOptionsPanel()
    {
        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);
        tabText.text = "Pause Menu";
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }

    public void ClosePad()
    {
        ToggleHUD(false, "Pause");
        if (SceneManager.GetActiveScene().name != "Space") PlayerController.instance.ClosePad("Pause");
        else SpaceshipController.instance.ClosePad("Pause");
    }

    public void SetMusicVolume(float volume)
    {
        SoundManager.instance.SetMusicVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        SoundManager.instance.SetSfxVolume(volume);
    }

    public void Quit()
    {
        GameManager.instance.QuitGame();
    }
}
