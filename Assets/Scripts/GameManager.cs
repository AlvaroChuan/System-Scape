using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager instance;

    // Player movement
    [Header("Movement Parameters")]
    [SerializeField] private float speed;
    public float PlayerSpeed => speed;
    [SerializeField] private float jumpForce;
    public float JumpForce => jumpForce;
    [SerializeField] private float jetpackForce;
    public float JetpackForce => jetpackForce;
    [SerializeField] private float rotationSpeed;
    public float RotationSpeed => rotationSpeed;

    // Health management
    private int maxHP = 100;
    public int MaxHP => maxHP;
    private int currentHP = 100;
    public int CurrentHP => currentHP;
    private float healthRegenTimeout = 5f;
    private int healthRegenAmmount = 1;
    private float healthRegenInterval = 0.5f;
    private int healthPerKill = 0;

    // Oxygen management
    private float maxOxygen = 60f;
    public float MaxOxygen => maxOxygen;
    private float currentOxygen = 60f;
    public float OxygenLevel => currentOxygen;
    private float oxygenUseRate = 0.5f;
    private float oxygenUseInterval = 1f;
    private float oxygenRegenAmmount = 15f;
    private float oxygenPerKill = 0f;
    private bool regenerateOxygen = false;

    // Spacesuit
    private float damageReduction = 0f;
    private float reflectionChance = 0f;

    // Materials
    private int maxAmmountPerMaterial = 30;
    public int MaxAmmountPerMaterial => maxAmmountPerMaterial;
    public List<Material> drillableMaterials = new List<Material>();
    private int iron = 0;
    public int Iron => iron;
    private int copper = 0;
    public int Copper => copper;
    private int magnetite = 0;
    public int Magnetite => magnetite;
    private int quartz = 0;
    public int Quartz => quartz;
    private int phobosite = 0;
    public int Phobosite => phobosite;
    private int radium = 0;
    public int Radium => radium;
    private int glaciate = 0;
    public int Glaciate => glaciate;
    private int bismuth = 0;
    public int Bismuth => bismuth;
    private int platinum = 0;
    public int Platinum => platinum;
    private int petralact = 0;
    public int Petralact => petralact;

    // Spaceship
    [Header("Spaceship Parameters")]
    private float maxSpaceshipSpeed = 10f;
    public float MaxSpaceshipSpeed => maxSpaceshipSpeed;
    private float spaceShipAcceleration = 5f;
    public float SpaceShipAcceleration => spaceShipAcceleration;
    private float spaceShipDeceleration = 5f;
    public float SpaceShipDeceleration => spaceShipDeceleration;
    private int landingGearTier = 0;
    public int LandingGearTier => landingGearTier;

    // Gadgets value
    private int selectedGadget = 0;
    public int SelectedGadget => selectedGadget; // 0 = Drill, 1 = Sword, 2 = Gun
    private float swordDamage = 10f;
    public float SwordDamage => swordDamage;
    private float bulletDamage = 2.5f;
    public float BulletDamage => bulletDamage;
    private int bulletsPerBurst = 3;
    public int BulletsPerBurst => bulletsPerBurst;
    private float aimRangeRifle = 10f;
    public float AimRangeRifle => aimRangeRifle;
    public float aimSwordRange = 2f;
    public float AimSwordRange => aimSwordRange;
    private int drillGadgetTier = 1;
    public int DrillGadgetTier => drillGadgetTier;
    private float drillDistance = 2.5f;
    public float DrillDistance => drillDistance;
    private bool jetpackEnabled = false;
    public bool JetpackEnabled => jetpackEnabled;
    private bool FlashlightEnabled = false;
    public bool Flashlight => FlashlightEnabled;
    private bool radarEnabled = false;
    public bool RadarEnabled => radarEnabled;
    private bool enemyDetectionEnabled = false;
    public bool EnemyDetectionEnabled => enemyDetectionEnabled;
    private bool flightCompanionEnabled = false;
    public bool FlightCompanionEnabled => flightCompanionEnabled;

    //Enemies
    public List<Enemy> enemiesInMaxRange = new List<Enemy>();

    // Upgrades
    public List<Upgrade> upgrades; // List of all available upgrades

    // Coroutines
    private Coroutine healthRegenCoroutine;

    // Space
    public float solarSystemRotation;
    public float solarSystemStarSize = 100f;
    public string lastPlanet = "Mercum";
    public bool goodEnding = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void PrepareRun()
    {
        ResetVariables();
        StartCoroutine(OxygenManagement());
        StartCoroutine(SolarSystemManagement());
        LoadScene("Mercum");
    }

    private void ResetVariables()
    {
        // Reset player stats for a new run
        speed = 5f;
        jumpForce = 5f;
        jetpackForce = 15f;
        rotationSpeed = 10f;

        maxHP = 100;
        currentHP = maxHP;
        healthRegenTimeout = 5f;
        healthRegenAmmount = 1;
        healthRegenInterval = 0.5f;
        healthPerKill = 0;

        maxOxygen = 60f;
        currentOxygen = maxOxygen;
        oxygenUseRate = 0.5f;
        oxygenUseInterval = 1f;
        oxygenRegenAmmount = 15f;
        oxygenPerKill = 0f;

        damageReduction = 0f;
        reflectionChance = 0f;

        iron = 0;
        copper = 0;
        magnetite = 0;
        quartz = 0;
        phobosite = 0;
        radium = 0;
        glaciate = 0;
        bismuth = 0;
        platinum = 0;
        petralact = 0;
        maxAmmountPerMaterial = 110;

        maxSpaceshipSpeed = 10f;
        spaceShipAcceleration = 5f;
        spaceShipDeceleration = 5f;
        landingGearTier = 0;

        selectedGadget = 0;

        swordDamage = 10f;
        bulletDamage = 2.5f;
        bulletsPerBurst = 3;
        drillGadgetTier = 1;
        drillDistance = 2.5f;

        jetpackEnabled = false;
        FlashlightEnabled = false;
        radarEnabled = false;
        enemyDetectionEnabled = false;
        flightCompanionEnabled = false;
        enemiesInMaxRange.Clear();
        drillableMaterials.Clear();
        solarSystemRotation = 0f;
        solarSystemStarSize = 100f;
        lastPlanet = "Mercum";
        goodEnding = false;

        // Reset upgrades
        foreach (Upgrade upgrade in upgrades) upgrade.Deactivate();
    }

    public void EndRun(int reason)
    {
        Time.timeScale = 1f;
        StopAllCoroutines();
        switch (reason)
        {
            case 0: // Player died due to the sun
                goodEnding = false;
                HUDManager.instance.OpenPad();
                break;
            case 1: // Player died due to enemies
                goodEnding = false;
                HUDManager.instance.OpenPad();
                break;
            case 2: // Oxygen ran out
                goodEnding = false;
                HUDManager.instance.OpenPad();
                break;
            case 3: // Player quit
                goodEnding = false;
                HUDManager.instance.FadeOut("Main Menu");
                SoundManager.instance.PlayMusic(SoundManager.ClipEnum.MainTheme);
                break;
            case 4: // Player completed the game
                goodEnding = true;
                HUDManager.instance.OpenPad();
                break;
        }
    }

    public void SwitchGadget(bool forward)
    {
        selectedGadget += forward ? 1 : -1;
        if (selectedGadget < 0) selectedGadget = 2;
        else if (selectedGadget > 2) selectedGadget = 0;
        PlayerController.instance.SetAnimTree(selectedGadget);
    }

    public int ApplyUpgrade(Upgrade upgrade) // Returns 0 if successful, 1 if already active, 2 if prerequisites not met, 3 if not enough resources
    {
        if (upgrade.IsActive) return 1;

        // Check prerequisites
        foreach (var prerequisite in upgrade.prerequisites)
        {
            if (!prerequisite.IsActive) return 2;
        }

        if (iron < upgrade.ironCost || copper < upgrade.copperCost || magnetite < upgrade.magnetiteCost ||
            quartz < upgrade.quartzCost || phobosite < upgrade.phobositeCost || radium < upgrade.radiumCost ||
            glaciate < upgrade.glaciateCost || bismuth < upgrade.bismuthCost || platinum < upgrade.platinumCost ||
            petralact < upgrade.petralactCost)
        {
            return 3;
        }

        // Deduct costs
        iron -= upgrade.ironCost;
        copper -= upgrade.copperCost;
        magnetite -= upgrade.magnetiteCost;
        quartz -= upgrade.quartzCost;
        phobosite -= upgrade.phobositeCost;
        radium -= upgrade.radiumCost;
        glaciate -= upgrade.glaciateCost;
        bismuth -= upgrade.bismuthCost;
        platinum -= upgrade.platinumCost;
        petralact -= upgrade.petralactCost;

        // Apply upgrade and deduct costs
        switch (upgrade.upgradeName)
        {
            case Upgrade.UpgradeName.BIGGER_TANKS:
                maxOxygen += 10f;
                break;
            case Upgrade.UpgradeName.BIGGER_TANKS_2:
                maxOxygen += 20f;
                break;
            case Upgrade.UpgradeName.BIGGER_TANKS_3:
                maxOxygen += 30f;
                break;
            case Upgrade.UpgradeName.EFFICIENT_RELEASE:
                oxygenUseRate -= 0.25f;
                break;
            case Upgrade.UpgradeName.BIOLOGICAL_RECYCLING:
                oxygenPerKill += 1f;
                break;
            case Upgrade.UpgradeName.BIOLOGICAL_RECYCLING_2:
                oxygenPerKill += 2f;
                break;
            case Upgrade.UpgradeName.LEG_EXOSKELETON:
                speed += 1f;
                break;
            case Upgrade.UpgradeName.LEG_EXOSKELETON_2:
                speed += 2f;
                break;
            case Upgrade.UpgradeName.LIGHTER_POCKETS:
                jumpForce += 1f;
                break;
            case Upgrade.UpgradeName.LIGHTER_POCKETS_2:
                jumpForce += 2f;
                break;
            case Upgrade.UpgradeName.THICKER_FABRIC:
                damageReduction += 0.05f;
                break;
            case Upgrade.UpgradeName.THICKER_FABRIC_2:
                damageReduction += 0.1f;
                break;
            case Upgrade.UpgradeName.DEFLECTIVE_ARMOR:
                reflectionChance += 0.05f;
                break;
            case Upgrade.UpgradeName.DEFLECTIVE_ARMOR_2:
                reflectionChance += 0.05f;
                break;
            case Upgrade.UpgradeName.ENEMY_BUFFET:
                healthPerKill += 1;
                break;
            case Upgrade.UpgradeName.BIGGER_POCKETS:
                maxAmmountPerMaterial += 10;
                break;
            case Upgrade.UpgradeName.BIGGER_POCKETS_2:
                maxAmmountPerMaterial += 10;
                break;
            case Upgrade.UpgradeName.BIGGER_POCKETS_3:
                maxAmmountPerMaterial += 20;
                break;
            case Upgrade.UpgradeName.LANDING_GEAR:
                landingGearTier = 1;
                break;
            case Upgrade.UpgradeName.LANDING_GEAR_2:
                landingGearTier = 2;
                break;
            case Upgrade.UpgradeName.LANDING_GEAR_3:
                landingGearTier = 3;
                break;
            case Upgrade.UpgradeName.LANDING_GEAR_4:
                landingGearTier = 4;
                break;
            case Upgrade.UpgradeName.ENGINE_BURST:
                maxSpaceshipSpeed += 10f;
                break;
            case Upgrade.UpgradeName.ENGINE_BURST_2:
                spaceShipAcceleration += 1f;
                break;
            case Upgrade.UpgradeName.ENGINE_BURST_3:
                spaceShipDeceleration += 1f;
                break;
            case Upgrade.UpgradeName.BREAKING_FLAPS:
                spaceShipDeceleration += 0.5f;
                break;
            case Upgrade.UpgradeName.BREAKING_FLAPS_2:
                spaceShipDeceleration += 0.5f;
                break;
            case Upgrade.UpgradeName.BREAKING_FLAPS_3:
                spaceShipDeceleration += 1f;
                break;
            case Upgrade.UpgradeName.INTERESTELLAR_ENGINE:
                landingGearTier = 5;
                break;
            case Upgrade.UpgradeName.SLICE_POWER:
                swordDamage += 5f;
                break;
            case Upgrade.UpgradeName.SLICE_POWER_2:
                swordDamage += 10f;
                break;
            case Upgrade.UpgradeName.SLICE_POWER_3:
                swordDamage += 15f;
                break;
            case Upgrade.UpgradeName.BULLET_POWER:
                bulletDamage += 2.5f;
                break;
            case Upgrade.UpgradeName.BULLET_POWER_2:
                bulletDamage += 2.5f;
                break;
            case Upgrade.UpgradeName.BULLET_POWER_3:
                bulletsPerBurst += 1;
                break;
            case Upgrade.UpgradeName.DRILL_POWER:
                drillGadgetTier = 2;
                break;
            case Upgrade.UpgradeName.DRILL_POWER_2:
                drillGadgetTier = 3;
                break;
            case Upgrade.UpgradeName.DRILL_POWER_3:
                drillGadgetTier = 4;
                break;
            case Upgrade.UpgradeName.DRILL_POWER_4:
                drillGadgetTier = 5;
                break;
            case Upgrade.UpgradeName.GATHERING_DISTANCE:
                drillDistance += 0.5f;
                break;
            case Upgrade.UpgradeName.GATHERING_DISTANCE_2:
                drillDistance += 0.5f;
                break;
            case Upgrade.UpgradeName.GATHERING_DISTANCE_3:
                drillDistance += 0.5f;
                break;
            case Upgrade.UpgradeName.GATHERING_DISTANCE_4:
                drillDistance += 0.5f;
                break;
            case Upgrade.UpgradeName.JETPACK:
                jetpackEnabled = true;
                break;
            case Upgrade.UpgradeName.FLASHLIGHT:
                FlashlightEnabled = true;
                break;
            case Upgrade.UpgradeName.RADAR:
                radarEnabled = true;
                break;
            case Upgrade.UpgradeName.ENEMY_DETECTION:
                enemyDetectionEnabled = true;
                break;
            case Upgrade.UpgradeName.FLIGHT_COMPANION:
                flightCompanionEnabled = true;
                break;
        }
        upgrade.Activate();
        return 0;
    }

    public void StartOxygenRegeneration()
    {
        regenerateOxygen = true;
    }

    public void StopOxygenRegeneration()
    {
        regenerateOxygen = false;
    }

    public void DamagePlayer(int damage)
    {
        Debug.Log($"Player took {damage} damage. Current HP: {currentHP}");
        if (SceneManager.GetActiveScene().name != "Space") PlayerController.instance.Damage();
        if (Random.Range(0, 1f) < reflectionChance) return; // Reflect damage if chance is met
        currentHP -= Mathf.Max(0, damage - (int)(damage * damageReduction)); // Apply damage reduction
        if (currentHP <= 0) EndRun(1);
        else
        {
            if (healthRegenCoroutine != null) StopCoroutine(healthRegenCoroutine);
            healthRegenCoroutine = StartCoroutine(HealthRegeneration());
        }
    }

    public void EnemyDeath()
    {
        currentHP = Mathf.Min(currentHP + healthPerKill, maxHP);
        currentOxygen = Mathf.Min(currentOxygen + oxygenPerKill, maxOxygen);
    }

    public void AddMaterial(string materialName)
    {
        switch (materialName)
        {
            case "Iron":
                if (iron < maxAmmountPerMaterial) iron += 10;
                break;
            case "Copper":
                if (copper < maxAmmountPerMaterial) copper += 10;
                break;
            case "Magnetite":
                if (magnetite < maxAmmountPerMaterial) magnetite += 10;
                break;
            case "Quartz":
                if (quartz < maxAmmountPerMaterial) quartz += 10;
                break;
            case "Phobosite":
                if (phobosite < maxAmmountPerMaterial) phobosite += 10;
                break;
            case "Radium":
                if (radium < maxAmmountPerMaterial) radium += 10;
                break;
            case "Glaciate":
                if (glaciate < maxAmmountPerMaterial) glaciate += 10;
                break;
            case "Bismuth":
                if (bismuth < maxAmmountPerMaterial) bismuth += 10;
                break;
            case "Platinum":
                if (platinum < maxAmmountPerMaterial) platinum += 10;
                break;
            case "Petralact":
                if (petralact < maxAmmountPerMaterial) petralact += 10;
                break;
        }
    }

    public void LoadScene(string sceneName)
    {
        switch (sceneName)
        {
            case "Mercum":
                SoundManager.instance.PlayMusic(SoundManager.ClipEnum.MercumTheme, true, 0.5f);
                break;
            case "Colis":
                SoundManager.instance.PlayMusic(SoundManager.ClipEnum.ColisTheme, true, 0.75f);
                break;
            case "Phobos":
                SoundManager.instance.PlayMusic(SoundManager.ClipEnum.PhobosTheme, true, 0.5f);
                break;
            case "Regio":
                SoundManager.instance.PlayMusic(SoundManager.ClipEnum.RegioTheme, true, 1f);
                break;
            case "Platum":
                SoundManager.instance.PlayMusic(SoundManager.ClipEnum.PlatumTheme, true, 0.5f);
                break;
            case "Space":
                SoundManager.instance.PlayMusic(SoundManager.ClipEnum.SpaceTheme, true, 1f);
                break;
        }
        SceneManager.LoadScene(sceneName);
        HUDManager.instance.FadeIn();
        if (sceneName == "Main Menu") HUDManager.instance.SetMainMenu();
        HUDManager.instance.ToggleHUD(false, "Ending");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void AttemptToScape()
    {
        if (landingGearTier < 5)
        {
            SoundManager.instance.PlaySfx(SoundManager.ClipEnum.Prohibited, false, 2f);
        }
        else
        {
            EndRun(4); // Player completed the game
        }
    }

    private IEnumerator OxygenManagement()
    {
        while (true)
        {
            if (SceneManager.GetActiveScene().name != "Space") currentOxygen = regenerateOxygen ? Mathf.Min(currentOxygen + oxygenRegenAmmount, maxOxygen) : Mathf.Max(currentOxygen - oxygenUseRate, 0f);
            yield return new WaitForSeconds(oxygenUseInterval);
            if (currentOxygen <= 0f) EndRun(2);
        }
    }

    private IEnumerator HealthRegeneration()
    {
        yield return new WaitForSeconds(healthRegenTimeout);
        while (currentHP < maxHP)
        {
            currentHP = Mathf.Min(currentHP + healthRegenAmmount, maxHP);
            yield return new WaitForSeconds(healthRegenInterval);
        }
    }

    private IEnumerator SolarSystemManagement()
    {
        while (true)
        {
            solarSystemRotation += Time.deltaTime * 0.25f;
            if (solarSystemRotation >= 360f) solarSystemRotation -= 360f;
            solarSystemStarSize += Time.deltaTime * 0.25f;
            if (SceneManager.GetActiveScene().name == "Mercum" && solarSystemStarSize > 450) EndRun(0);
            else if (SceneManager.GetActiveScene().name == "Colis" && solarSystemStarSize > 850) EndRun(0);
            else if (SceneManager.GetActiveScene().name == "Phobos" && solarSystemStarSize > 1250) EndRun(0);
            else if (SceneManager.GetActiveScene().name == "Regio" && solarSystemStarSize > 1650) EndRun(0);
            else if (SceneManager.GetActiveScene().name == "Platum" && solarSystemStarSize > 2050) EndRun(0);
            yield return null;
        }
    }

}
