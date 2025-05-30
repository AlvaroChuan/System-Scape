using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Upgrade")]
public class Upgrade : ScriptableObject
{
    private enum UpgradeTree
    {
        EQUIPMENT,
        GADGETS,
        SPACESHIP
    }
    private enum UpgradeName
    {
        // Oxygen system
        BIGGER_TANKS,
        BIGGER_TANKS_2,
        BIGGER_TANKS_3,
        EFFICIENT_RELEASE,
        BIOLOGICAL_RECYCLING,
        BIOLOGICAL_RECYCLING_2,

        // Movement system
        LEG_EXOSKELETON,
        LEG_EXOSKELETON_2,
        LIGHTER_POCKETS,
        LIGHTER_POCKETS_2,

        // Spacesuit
        THICKER_FABRIC,
        THICKER_FABRIC_2,
        DEFLECTIVE_ARMOR,
        DEFLECTIVE_ARMOR_2,
        ENEMY_BUFFET,

        // Material capacity
        BIGGER_POCKETS,
        BIGGER_POCKETS_2,
        BIGGER_POCKETS_3,

        // Landing gear
        LANDING_GEAR,
        LANDING_GEAR_2,
        LANDING_GEAR_3,
        LANDING_GEAR_4,

        // Manouvering
        ENGINE_BURST,
        ENGINE_BURST_2,
        ENGINE_BURST_3,
        BREAKING_FLAPS,
        BREAKING_FLAPS_2,
        BREAKING_FLAPS_3,
        INTERESTELLAR_ENGINE,

        // Offensive
        SLICE_POWER,
        SLICE_POWER_2,
        SLICE_POWER_3,
        BULLET_POWER,
        BULLET_POWER_2,
        BULLET_POWER_3,

        // Gathering
        DRILL_POWER,
        DRILL_POWER_2,
        DRILL_POWER_3,
        DRILL_POWER_4,
        GATHERING_DISTANCE,
        GATHERING_DISTANCE_2,
        GATHERING_DISTANCE_3,
        GATHERING_DISTANCE_4,

        // Utility
        JETPACK,
        FLASHLIGHT,
        RADAR,
        ENEMY_DETECTION,
        FLIGHT_COMPANION
    }
    [Header("Upgrade Details")]
    [SerializeField] private UpgradeTree upgradeTree;
    [SerializeField] private UpgradeName upgradeName;
    [SerializeField] private Upgrade[] prerequisites;
    [Header("Upgrade Costs")]
    [SerializeField] private int ironCost;
    [SerializeField] private int copperCost;
    [SerializeField] private int magnetiteCost;
    [SerializeField] private int quartzCost;
    [SerializeField] private int phobositeCost;
    [SerializeField] private int radiumCost;
    [SerializeField] private int glaciateCost;
    [SerializeField] private int bismuthCost;
    [SerializeField] private int platinumCost;
    [SerializeField] private int petralactCost;
    private bool active = false;
    public bool IsActive => active;
    public void Activate() { active = true; }
}
