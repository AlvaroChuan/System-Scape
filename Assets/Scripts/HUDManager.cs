using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Playables;

public class HUDManager : MonoBehaviour
{
    // Singleton
    public static HUDManager instance;

    [Header("Main Menu HUD")]
    [SerializeField] private GameObject MainMenuHUD;
    [SerializeField] private Button playButton; // Reference to set as the selected button for the event system
    [SerializeField] private Button nextButton1;
    [SerializeField] private Button nextButton2;
    [SerializeField] private Button nextButton3;
    [SerializeField] private Button backButton1;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private Slider musicSlider2;
    [SerializeField] private Slider sfxSlider2;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject storyPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject storyPanel2;

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
    [SerializeField] private Button statsButton;
    [SerializeField] private Button equipmentButton;
    [SerializeField] private Button spaceshipButton;
    [SerializeField] private Button gadgetsButton;
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private GameObject equipmentPanel;
    [SerializeField] private GameObject spaceshipPanel;
    [SerializeField] private GameObject gadgetsPanel;

    [Header("Stats elements")]
    [SerializeField] private TMP_Text healthAmountText;
    [SerializeField] private Image healthFillImage;
    [SerializeField] private TMP_Text oxygenAmountText;
    [SerializeField] private Image oxygenFillImage;
    [SerializeField] private TMP_Text ironAmountText;
    [SerializeField] private Image ironFillImage;
    [SerializeField] private TMP_Text copperAmountText;
    [SerializeField] private Image copperFillImage;
    [SerializeField] private TMP_Text magnetiteAmountText;
    [SerializeField] private Image magnetiteFillImage;
    [SerializeField] private TMP_Text quartzAmountText;
    [SerializeField] private Image quartzFillImage;
    [SerializeField] private TMP_Text phobositeAmountText;
    [SerializeField] private Image phobositeFillImage;
    [SerializeField] private TMP_Text radiumAmountText;
    [SerializeField] private Image radiumFillImage;
    [SerializeField] private TMP_Text glaciateAmountText;
    [SerializeField] private Image glaciateFillImage;
    [SerializeField] private TMP_Text bismuthAmountText;
    [SerializeField] private Image bismuthFillImage;
    [SerializeField] private TMP_Text platinumAmountText;
    [SerializeField] private Image platinumFillImage;
    [SerializeField] private TMP_Text petralactAmountText;
    [SerializeField] private Image petralactFillImage;

    [Header("Ending HUD")]
    [SerializeField] private GameObject endingHUD;
    [SerializeField] private GameObject goodEndingPanel;
    [SerializeField] private GameObject badEndingPanel;
    [SerializeField] private Button goodEndingButton;
    [SerializeField] private Button badEndingButton;

    [Header("Selection Rect")]
    [SerializeField] private Image selectionRect;

    [Header("Fade In / Out")]
    [SerializeField] private Animator fadeAnimator;


    private GameObject currentPanel;
    private int currentSubPanelIndex = 0;
    private bool onUI = false;
    private string nextSceneName = "";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (SceneManager.GetActiveScene().name == "Main Menu")
            {
                ToggleHUD(false, "Pause");
                ToggleHUD(false, "Upgrades");
                ToggleHUD(true, "Main Menu");
            }

            if (PlayerPrefs.HasKey("musicVolume"))
            {
                musicSlider2.value = PlayerPrefs.GetFloat("musicVolume");
                musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
            }
            else
            {
                musicSlider2.value = 1f;
                musicSlider.value = 1f;
            }

            if (PlayerPrefs.HasKey("sfxVolume"))
            {
                sfxSlider2.value = PlayerPrefs.GetFloat("sfxVolume");
                sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
            }
            else
            {
                sfxSlider2.value = 1f;
                sfxSlider.value = 1f;
            }

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (SceneManager.GetActiveScene().name == "Main Menu")
            {
                HUDManager.instance.ToggleHUD(false, "Pause");
                HUDManager.instance.ToggleHUD(false, "Upgrades");
                HUDManager.instance.ToggleHUD(true, "Main Menu");
            }
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Update stats
        healthAmountText.text = GameManager.instance.CurrentHP + " / " + GameManager.instance.MaxHP;
        healthFillImage.fillAmount = GameManager.instance.CurrentHP / GameManager.instance.MaxHP;
        oxygenAmountText.text = GameManager.instance.OxygenLevel + " / " + GameManager.instance.MaxOxygen;
        oxygenFillImage.fillAmount = GameManager.instance.OxygenLevel / GameManager.instance.MaxOxygen;
        ironAmountText.text = GameManager.instance.Iron + " / " + GameManager.instance.MaxAmmountPerMaterial;
        ironFillImage.fillAmount = GameManager.instance.Iron / GameManager.instance.MaxAmmountPerMaterial;
        copperAmountText.text = GameManager.instance.Copper + " / " + GameManager.instance.MaxAmmountPerMaterial;
        copperFillImage.fillAmount = GameManager.instance.Copper / GameManager.instance.MaxAmmountPerMaterial;
        magnetiteAmountText.text = GameManager.instance.Magnetite + " / " + GameManager.instance.MaxAmmountPerMaterial;
        magnetiteFillImage.fillAmount = GameManager.instance.Magnetite / GameManager.instance.MaxAmmountPerMaterial;
        quartzAmountText.text = GameManager.instance.Quartz + " / " + GameManager.instance.MaxAmmountPerMaterial;
        quartzFillImage.fillAmount = GameManager.instance.Quartz / GameManager.instance.MaxAmmountPerMaterial;
        phobositeAmountText.text = GameManager.instance.Phobosite + " / " + GameManager.instance.MaxAmmountPerMaterial;
        phobositeFillImage.fillAmount = GameManager.instance.Phobosite / GameManager.instance.MaxAmmountPerMaterial;
        radiumAmountText.text = GameManager.instance.Radium + " / " + GameManager.instance.MaxAmmountPerMaterial;
        radiumFillImage.fillAmount = GameManager.instance.Radium / GameManager.instance.MaxAmmountPerMaterial;
        glaciateAmountText.text = GameManager.instance.Glaciate + " / " + GameManager.instance.MaxAmmountPerMaterial;
        glaciateFillImage.fillAmount = GameManager.instance.Glaciate / GameManager.instance.MaxAmmountPerMaterial;
        bismuthAmountText.text = GameManager.instance.Bismuth + " / " + GameManager.instance.MaxAmmountPerMaterial;
        bismuthFillImage.fillAmount = GameManager.instance.Bismuth / GameManager.instance.MaxAmmountPerMaterial;
        platinumAmountText.text = GameManager.instance.Platinum + " / " + GameManager.instance.MaxAmmountPerMaterial;
        platinumFillImage.fillAmount = GameManager.instance.Platinum / GameManager.instance.MaxAmmountPerMaterial;
        petralactAmountText.text = GameManager.instance.Petralact + " / " + GameManager.instance.MaxAmmountPerMaterial;
        petralactFillImage.fillAmount = GameManager.instance.Petralact / GameManager.instance.MaxAmmountPerMaterial;

        // Seletor
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
        MainMenuHUD.SetActive(false);
        switch (type)
        {
            case "Upgrades":
                UpgradesHUD.SetActive(state);
                if (state) EventSystem.current.SetSelectedGameObject(statsButton.gameObject);
                ToggleUpgradesPanel(statsPanel);
                break;
            case "Pause":
                PauseHUD.SetActive(state);
                if (state)
                {
                    EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
                    if (PlayerPrefs.HasKey("musicVolume"))
                    {
                        musicSlider2.value = PlayerPrefs.GetFloat("musicVolume");
                        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
                    }
                    else
                    {
                        musicSlider2.value = 1f;
                        musicSlider.value = 1f;
                    }

                    if (PlayerPrefs.HasKey("sfxVolume"))
                    {
                        sfxSlider2.value = PlayerPrefs.GetFloat("sfxVolume");
                        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
                    }
                    else
                    {
                        sfxSlider2.value = 1f;
                        sfxSlider.value = 1f;
                    }
                }
                break;
            case "Main Menu":
                MainMenuHUD.SetActive(state);
                if (state)
                {
                    EventSystem.current.SetSelectedGameObject(playButton.gameObject);
                    if (PlayerPrefs.HasKey("musicVolume"))
                    {
                        musicSlider2.value = PlayerPrefs.GetFloat("musicVolume");
                        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
                    }
                    else
                    {
                        musicSlider2.value = 1f;
                        musicSlider.value = 1f;
                    }

                    if (PlayerPrefs.HasKey("sfxVolume"))
                    {
                        sfxSlider2.value = PlayerPrefs.GetFloat("sfxVolume");
                        sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
                    }
                    else
                    {
                        sfxSlider2.value = 1f;
                        sfxSlider.value = 1f;
                    }
                }
                break;
            case "Ending":
                endingHUD.SetActive(state);
                if (state)
                {
                    if (GameManager.instance.goodEnding)
                    {
                        goodEndingPanel.SetActive(true);
                        badEndingPanel.SetActive(false);
                        EventSystem.current.SetSelectedGameObject(goodEndingButton.gameObject);
                    }
                    else
                    {
                        goodEndingPanel.SetActive(false);
                        badEndingPanel.SetActive(true);
                        EventSystem.current.SetSelectedGameObject(badEndingButton.gameObject);
                    }
                }
                break;
        }
        onUI = state;
        selectionRect.gameObject.SetActive(state);
    }

    public void ToggleUpgradesPanel(GameObject panel)
    {
        if(SoundManager.instance != null) SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Interface);
        GameObject[] panelList = new GameObject[] { statsPanel, equipmentPanel, spaceshipPanel, gadgetsPanel };
        Button[] buttonList = new Button[] { statsButton, equipmentButton, spaceshipButton, gadgetsButton };
        foreach (GameObject go in panelList) go.SetActive(go == panel);
        foreach (Button btn in buttonList)
        {
            btn.image.color = headerColor;
            btn.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = panelColor;
        }
        switch (panel)
        {
            case GameObject go when go == statsPanel:
                statsButton.image.color = panelColor;
                statsButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
                break;
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
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Interface);
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
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Interface);
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
        if(result == 0) SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Upgrade, false, 2f);
        else SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Prohibited, false, 2f);
    }

    public void BackToMainMenu()
    {
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Interface);
        GameManager.instance.EndRun(3);
    }

    public void SetMainMenu()
    {
        ToggleHUD(false, "Pause");
        ToggleHUD(false, "Upgrades");
        ToggleHUD(true, "Main Menu");
    }

    public void OpenOptionsPanel()
    {
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Interface);
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
        tabText.text = "Options";
        EventSystem.current.SetSelectedGameObject(backButton.gameObject);
    }

    public void OpenOptionsPanelFromMainMenu()
    {
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Interface);
        optionsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(backButton1.gameObject);
    }

    public void CloseOptionsPanel()
    {
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Interface);
        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);
        tabText.text = "Pause Menu";
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }

    public void CloseOptionsPanelFromMainMenu()
    {
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Interface);
        optionsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(playButton.gameObject);
    }

    public void OpenPad()
    {
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.OpenPad);
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.CRT, true);
        if (SceneManager.GetActiveScene().name != "Space") PlayerController.instance.OpenPad("Ending");
        else SpaceshipController.instance.OpenPad("Ending");
        ToggleHUD(true, "Ending");
        EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
    }

    public void ClosePad()
    {
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.ClosePad);
        SoundManager.instance.StopSfx(SoundManager.ClipEnum.CRT);
        ToggleHUD(false, "Pause");
        if (SceneManager.GetActiveScene().name != "Space") PlayerController.instance.ClosePad("Pause");
        else SpaceshipController.instance.ClosePad("Pause");
    }

    public void SetMusicVolume1()
    {
        if(SoundManager.instance != null) SoundManager.instance.SetMusicVolume(musicSlider.value);
    }

    public void SetSFXVolume1()
    {
        if(SoundManager.instance != null) SoundManager.instance.SetSfxVolume(sfxSlider.value);
    }

    public void SetMusicVolume2()
    {
        if(SoundManager.instance != null) SoundManager.instance.SetMusicVolume(musicSlider2.value);
    }

    public void SetSFXVolume2()
    {
        if(SoundManager.instance != null) SoundManager.instance.SetSfxVolume(sfxSlider2.value);
    }

    public void Quit()
    {
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Interface);
        GameManager.instance.QuitGame();
    }

    public void NextStoryPanel(GameObject nextPanel)
    {
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Interface);
        GameObject[] panels = new GameObject[] { storyPanel, controlsPanel, storyPanel2, mainMenuPanel };
        Button[] nextButtons = new Button[] { nextButton1, nextButton2, nextButton3 };
        foreach (GameObject panel in panels) panel.SetActive(false);
        if (nextPanel == mainMenuPanel)
        {
            PlayableDirector director = GameObject.Find("Director").GetComponent<PlayableDirector>();
            director.Play();
            mainMenuPanel.SetActive(true);
            ToggleHUD(false, "Main Menu");
            return;
        }
        nextPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(nextButtons[Array.IndexOf(panels, nextPanel)].gameObject);
    }

    public void SetTargetCamera(Camera cam)
    {
        gameObject.GetComponent<Canvas>().worldCamera = cam;
    }

    public void FadeOut(string sceneName)
    {
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.FadeOut);
        nextSceneName = sceneName;
        fadeAnimator.SetTrigger("FadeOut");
    }

    public void FadeIn()
    {
        SoundManager.instance.PlaySfx(SoundManager.ClipEnum.FadeIn);
        fadeAnimator.SetTrigger("FadeIn");
    }

    public void OnFadeOutFinished()
    {
        if (SceneManager.GetActiveScene().name != "Main Menu") GameManager.instance.LoadScene(nextSceneName);
        else GameManager.instance.PrepareRun();
    }
}
