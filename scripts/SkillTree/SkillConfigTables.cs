// Fill in the actual configs in this file.
using AO;
using SC = SkillConfig;
using Assembly.scripts.Effects.ActiveSkills;

public static partial class SkillConfig
{
    // NOTE: The query dictionary (SkillKey : NodeConfig) is defined at the bottom of this file as 'STConfigQueryDict'
    // You MUST define node config and add it to the query dict.
    
    // [Add Skill] item 1: Config Entry
    #region Node Configs : Basic

    /// <summary>
    /// Punch node
    /// </summary>
    public static readonly SkillTreeNodeConfig PunchNodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Punch Forward and deals 1.0x damage",
        MaximumLevel = 1,
        NType = NodeType.SkillReplace,
        NTab = SkillTreeTabs.Basic,
        UpgradeCost = 0,
        UIPosition = new Vector2(450,30), // This will be set to the item's offset value
        SkillKey = "Punch",
        IconPath = "ability_icon_tmp/Punch_Tmp.png",
        ParentNodeKeys = new string[]{},
        ChildrenNodeKeys = new string[] {"HealthBoost", "AttackBoost"},
    };

    /// <summary>
    /// HealthBoost node
    /// </summary>
    public static readonly SkillTreeNodeConfig HealthBoostNodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Boost the player's health",
        MaximumLevel = 5,
        NType = NodeType.AttrBoost,
        NTab = SkillTreeTabs.Basic,
        UpgradeCost = 100,
        UIPosition = new Vector2(730,230),
        SkillKey = "HealthBoost",
        ParentNodeKeys = new string [] {"Punch"},
        ChildrenNodeKeys = new string[] {"Punch2"},
    };
    
    /// <summary>
    /// AttackBoost node
    /// </summary>
    public static readonly SkillTreeNodeConfig AttackBoostNodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Boost the player's attack",
        MaximumLevel = 5,
        NType = NodeType.AttrBoost,
        NTab = SkillTreeTabs.Basic,
        UpgradeCost = 100,
        UIPosition = new Vector2(110,230),
        SkillKey = "AttackBoost",
        ParentNodeKeys = new string[] {"Punch"},
        ChildrenNodeKeys = new string[] {"Punch2"},
    };
    
    /// <summary>
    /// RollOut node
    /// </summary>
    public static readonly SkillTreeNodeConfig RollOutNodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Boost your speed and enable you to crash into other players",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Defensive,
        UpgradeCost = 100,
        UIPosition = new Vector2(750,330),
        SkillKey = "RollOut",
        IconPath = "ability_icon_tmp/RollOut_Tmp.png",
        ParentNodeKeys =  new string[]{"Shield"},
        ChildrenNodeKeys = new string[]{},
    };
    
    /// <summary>
    /// Punch2 node
    /// </summary>
    public static readonly SkillTreeNodeConfig PunchTwoConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Boost your speed and enable you to crash into other players",
        MaximumLevel = 1,
        NType = NodeType.SkillReplace,
        NTab = SkillTreeTabs.Basic,
        UpgradeCost = 100,
        UIPosition = new Vector2(450,430),
        SkillKey = "Punch2",
        ParentNodeKeys = new string [] {"AttackBoost", "HealthBoost"},
        ChildrenNodeKeys = new string[] {},
    };
    
    /// <summary>
    /// Shield node
    /// </summary>
    public static readonly SkillTreeNodeConfig ShieldConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Dash forward and knock the enemy back",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Defensive,
        UpgradeCost = 100,
        UIPosition = new Vector2(450,30),
        SkillKey = "Shield",
        IconPath = "ability_icon_tmp/Shield_Tmp.png",
        ParentNodeKeys = new string [] {},
        ChildrenNodeKeys = new string[] {"RollOut"},
    };
    
    
    

    #endregion

    #region NodeConfig: Brawler

    /// <summary>
    /// ShoulderCrash node
    /// </summary>
    public static readonly SkillTreeNodeConfig ShoulderCrashNodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Dash to a direction and deals damage and knockback",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 100,
        UIPosition = new Vector2(450,30),
        SkillKey = "ShoulderCrash",
        IconPath = "ability_icon_tmp/ShoulderCrash_Tmp.png",
        ParentNodeKeys = new string[]{},
        ChildrenNodeKeys = new string[]{"GroundStomp", "Rage", "DoublePunch" },
    };
    
    public static readonly SkillTreeNodeConfig GroundStompConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Stomp The ground and damage nearby enemies",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 200,
        UIPosition = new Vector2(730, 230),
        SkillKey = "GroundStomp",
        IconPath = "ability_icon_tmp/GroundStomp_Tmp.png",
        ParentNodeKeys = new string[] {"ShoulderCrash"},
        ChildrenNodeKeys = new string[]{"LeapSlam"},
    };

    public static readonly SkillTreeNodeConfig RageConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Temporarily increase your attack power",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 200,
        UIPosition = new Vector2(450, 230),
        SkillKey = "Rage",
        IconPath = "ability_icon_tmp/GroundStomp_Tmp.png",
        ParentNodeKeys = new string[] { "ShoulderCrash" },
        ChildrenNodeKeys = new string[] { "BattleCry" },
    };

    public static readonly SkillTreeNodeConfig DoublePunchConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Punch to the front for two times in quick succession",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 200,
        UIPosition = new Vector2(170, 230),
        SkillKey = "DoublePunch",
        IconPath = "ability_icon_tmp/Punch_Tmp.png",
        ParentNodeKeys = new string[] { "ShoulderCrash" },
        ChildrenNodeKeys = new string[] { "SelfDestruct", "ClawSlash" },
    };

    public static readonly SkillTreeNodeConfig SelfDestructConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Release a big blast at the cost of damaging yourself",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 200,
        UIPosition = new Vector2(240, 430),
        SkillKey = "SelfDestruct",
        IconPath = "ability_icon_tmp/GroundStomp_Tmp.png",
        ParentNodeKeys = new string[] { "DoublePunch" },
        ChildrenNodeKeys = new string[] { },
    };

    public static readonly SkillTreeNodeConfig BattleCryConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Release a big blast at the cost of damaging yourself",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 200,
        UIPosition = new Vector2(450, 430),
        SkillKey = "BattleCry",
        IconPath = "ability_icon_tmp/RollOut_Tmp.png",
        ParentNodeKeys = new string[] { "Rage" },
        ChildrenNodeKeys = new string[] {  },
    };
    
    public static readonly SkillTreeNodeConfig ClawSlashConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Slash your enemies and make them bleed",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 200,
        UIPosition = new Vector2(10, 430),
        SkillKey = "ClawSlash",
        IconPath = "ability_icon_tmp/RollOut_Tmp.png",
        ParentNodeKeys = new string[] { "DoublePunch" },
        ChildrenNodeKeys = new string[] {  },
    };
    
    public static readonly SkillTreeNodeConfig LeapSlamConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Leap and slam the ground",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 999999,
        UIPosition = new Vector2(730, 430),
        SkillKey = "LeapSlam",
        IconPath = "ability_icon_tmp/RollOut_Tmp.png",
        ParentNodeKeys = new string[] { "GroundStomp" },
        ChildrenNodeKeys = new string[] {  },
    };

    #endregion

    #region NodeConfig: Psionic

    /// <summary>
    /// Spoon Throw
    /// </summary>
    public static readonly SkillTreeNodeConfig SpoonThrowConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Throw a spoon towards aiming position",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 100,
        UIPosition = new Vector2(450, 30),
        SkillKey = "SpoonThrow",
        IconPath = "ability_icon_tmp/SpoonThrow_Tmp.png",
        ParentNodeKeys = new string[] {},
        ChildrenNodeKeys = new string[]{"Befuddle", "Psybolt", "SelfHeal"},
    };
    
    /// <summary>
    /// Befuddle
    /// </summary>
    public static readonly SkillTreeNodeConfig BefuddleConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Throw a bird that confuses enemy movement on contact",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 100,
        UIPosition = new Vector2(450, 230),
        SkillKey = "Befuddle",
        IconPath = "ability_icon_tmp/Befuddle_Tmp.png",
        ParentNodeKeys = new string[] {"SpoonThrow"},
        ChildrenNodeKeys = new string[]{"Hypnotize"},
    };
    
    /// <summary>
    /// Psybolt
    /// </summary>
    public static readonly SkillTreeNodeConfig PsyboltConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Throw a bolt that knocks back enemy",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 100,
        UIPosition = new Vector2(170, 230),
        SkillKey = "Psybolt",
        IconPath = "ability_icon_tmp/Befuddle_Tmp.png",
        ParentNodeKeys = new string[] {"SpoonThrow"},
        ChildrenNodeKeys = new string[]{"PsionicBeam", "PsyThrow"},
    };
    
    /// <summary>
    /// SelfHeal
    /// </summary>
    public static readonly SkillTreeNodeConfig SelfHealConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Sit down and channel healing energy, restore a lot of health upon successful channel",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 100,
        UIPosition = new Vector2(730, 230),
        SkillKey = "SelfHeal",
        IconPath = "ability_icon_tmp/Shield_Tmp.png",
        ParentNodeKeys = new string[] {"SpoonThrow"},
        ChildrenNodeKeys = new string[]{"Regeneration"},
    };
    
    /// <summary>
    /// Regenerate
    /// </summary>
    public static readonly SkillTreeNodeConfig RegenerationConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Regenerate small amount of health over time",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 200,
        UIPosition = new Vector2(730, 430),
        SkillKey = "Regeneration",
        IconPath = "ability_icon_tmp/Shield_Tmp.png",
        ParentNodeKeys = new string[] {"SelfHeal"},
        ChildrenNodeKeys = new string[]{},
    };


    public static readonly SkillTreeNodeConfig HypnotizeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Regenerate small amount of health over time",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 200,
        UIPosition = new Vector2(450, 430),
        SkillKey = "Hypnotize",
        IconPath = String.Empty,
        ParentNodeKeys = new string[] {"Befuddle"},
        ChildrenNodeKeys = new string[]{},

    };
    
    public static readonly SkillTreeNodeConfig PsionicBeamConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Beam a small area in front of you with dark energy",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 200,
        UIPosition = new Vector2(10, 430),
        SkillKey = "PsionicBeam",
        IconPath = String.Empty,
        ParentNodeKeys = new string[] {"Psybolt"},
        ChildrenNodeKeys = new string[]{},

    };
    
    public static readonly SkillTreeNodeConfig PsyThrowConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Grab and throw your enemy",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 200,
        UIPosition = new Vector2(240, 430),
        SkillKey = "PsyThrow",
        IconPath = "ability_icon_tmp/PsyThrowGrab_Tmp.png",
        ParentNodeKeys = new string[] {"Psybolt"},
        ChildrenNodeKeys = new string[]{},

    };

    #endregion

    #region Stealth

    public static readonly SkillTreeNodeConfig InvisibilityConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Become Invisible for a short period",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 50,
        UIPosition = new Vector2(450, 30),
        SkillKey = "Invisibility",
        IconPath = "ability_icon_tmp/Invisibility_tmp.png",
        ParentNodeKeys = new string[] {},
        ChildrenNodeKeys = new string[]{"LightFeet"},

    };
    
    public static readonly SkillTreeNodeConfig LightFeetConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Increase movement speed temporarily",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 150,
        UIPosition = new Vector2(450, 230),
        SkillKey = "LightFeet",
        IconPath = "ability_icon_tmp/LightFeet_tmp.png",
        ParentNodeKeys = new string[] {"Invisibility"},
        ChildrenNodeKeys = new string[]{"Shuriken", "ShadowStep"},

    };

    public static readonly SkillTreeNodeConfig ShurikenConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Throw a shuriken which deals more damage if you struck the back of an enemy",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 250,
        UIPosition = new Vector2(450, 430),
        SkillKey = "Shuriken",
        IconPath = "ability_icon_tmp/Shuriken_tmp.png",
        ParentNodeKeys = new string[] { "LightFeet" },
        ChildrenNodeKeys = new string[] { "BearTrap", "Backstab" },
    };
    
    public static readonly SkillTreeNodeConfig BearTrapConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Setup a hidden trap to snare your enemies.",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 375,
        UIPosition = new Vector2(450, 630),
        SkillKey = "BearTrap",
        IconPath = "ability_icon_tmp/BearTrap_tmp.png",
        ParentNodeKeys = new string[] { "Shuriken" },
        ChildrenNodeKeys = new string[] { },
    };

    public static readonly SkillTreeNodeConfig ShadowStepConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Dash to your movement direction",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 250,
        UIPosition = new Vector2(230, 430),
        SkillKey = "ShadowStep",
        IconPath = "ability_icon_tmp/ShadowStep_tmp.png",
        ParentNodeKeys = new string[] { "LightFeet" },
        ChildrenNodeKeys = new string[] { },
    };
    
    public static readonly SkillTreeNodeConfig BackstabConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Throw A Kunai and backstab the victim if it hits.",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 375,
        UIPosition = new Vector2(230, 630),
        SkillKey = "Backstab",
        IconPath = "ability_icon_tmp/Backstab_tmp.png",
        ParentNodeKeys = new string[] { "Shuriken" },
        ChildrenNodeKeys = new string[] { },
    };

    #endregion
    
    // [Add Skill] item 2: Query Entry
    // MUST ADD for each new skill. This connects the unique skill key to their node config.
    // The standard format is {"[SkillKey]", "[SkillKey]Config"}
    public static readonly Dictionary<string, SkillTreeNodeConfig> STConfigQueryDict =
        new Dictionary<string, SkillTreeNodeConfig>()
        {
            // Basic
            {"Punch", PunchNodeConfig},
            {"HealthBoost", HealthBoostNodeConfig},
            {"AttackBoost", AttackBoostNodeConfig},
            {"Punch2", PunchTwoConfig},
            // Defensive
            {"RollOut", RollOutNodeConfig},
            {"Shield", ShieldConfig},
            // Brawler
            {"ShoulderCrash", ShoulderCrashNodeConfig},
            {"GroundStomp", GroundStompConfig},
            {"Rage", RageConfig},
            {"DoublePunch", DoublePunchConfig},
            {"SelfDestruct", SelfDestructConfig},
            {"BattleCry", BattleCryConfig},
            {"ClawSlash", ClawSlashConfig},
            {"LeapSlam", LeapSlamConfig},
            // Psionic
            {"SpoonThrow", SpoonThrowConfig},
            {"Befuddle", BefuddleConfig},
            {"Psybolt", PsyboltConfig},
            {"SelfHeal", SelfHealConfig},
            {"Regeneration", RegenerationConfig},
            {"Hypnotize", HypnotizeConfig},
            {"PsionicBeam", PsionicBeamConfig},
            {"PsyThrow", PsyThrowConfig},
            // Stealth
            {"Invisibility", InvisibilityConfig},
            {"LightFeet", LightFeetConfig},
            {"Shuriken", ShurikenConfig},
            {"BearTrap", BearTrapConfig},
            {"ShadowStep", ShadowStepConfig},
            {"Backstab", BackstabConfig}
        };

    // [Add Skill] Item 3: Put Classification Here
    public static readonly HashSet<string> AttrBoostSkills = new HashSet<string>() { "HealthBoost", "AttackBoost"};
    
    public static readonly HashSet<string> ActiveSkills = new HashSet<string>() {"Punch", "RollOut", "Shield", 
        // Brawler
        "ShoulderCrash", "GroundStomp", "Rage", "DoublePunch", "SelfDestruct", "BattleCry", "ClawSlash", "LeapSlam",
        // Psionic
        "SpoonThrow", "Befuddle", "Psybolt", "SelfHeal", "Regeneration", "Hypnotize","PsionicBeam", "PsyThrow",
        // Stealth
        "Invisibility","LightFeet", "Shuriken", "BearTrap", "ShadowStep", "Backstab"
    };

    public static readonly HashSet<string> ReplacementSkills = new HashSet<string>() {"Punch2"};
    
    public static readonly HashSet<string> SkillEnhanceSkills = new HashSet<string>() {  };
    
    
    // This affects how many pages appear on the ability book and ability vendor. Each page will have a tag that classifies the items.
    public static readonly List<SkillTreeTabs> STConfigTabsList = new List<SkillTreeTabs>()
    {
        SkillTreeTabs.Basic,
        SkillTreeTabs.Brawler,
        SkillTreeTabs.Defensive,
        SkillTreeTabs.Stealth,
        SkillTreeTabs.Psionic
    };

    public static readonly Dictionary<SkillTreeTabs, string> STTabsNameQueryDict =
        new Dictionary<SkillTreeTabs, string>()
        {
            {SkillTreeTabs.Basic, "Basic"},
            {SkillTreeTabs.Brawler, "Brawler"},
            {SkillTreeTabs.Defensive, "Defensive"},
            {SkillTreeTabs.Stealth, "Stealth"},
            {SkillTreeTabs.Psionic, "Psionic"}
        };
}

// [Add Skill] Item 4: Add the association between skillKey and type of ability.

public partial class FightAbility
{
    public static readonly Dictionary<string, Type> AbilityQueryDict = new Dictionary<string, Type>()
    {
        {"Empty", typeof(FightAbility)},
        {SC.PunchNodeConfig.SkillKey, typeof(AbilityPunch)},
        {SC.RollOutNodeConfig.SkillKey, typeof(AbilityRollOut)},
        {SC.ShieldConfig.SkillKey, typeof(AbilityShield)},
        {SC.ShoulderCrashNodeConfig.SkillKey, typeof(AbilityShoulderCrash)},
        {SC.SpoonThrowConfig.SkillKey, typeof(AbilitySpoonThrow)},
        {SC.GroundStompConfig.SkillKey, typeof(AbilityGroundStomp)},
        {SC.RageConfig.SkillKey, typeof(AbilityRage)},
        {SC.DoublePunchConfig.SkillKey, typeof(AbilityDoublePunch)},
        {SC.SelfDestructConfig.SkillKey, typeof(AbilitySelfDestruct)},
        {SC.BattleCryConfig.SkillKey, typeof(AbilityBattleCry)},
        {SC.ClawSlashConfig.SkillKey, typeof(AbilityClawSlash)},
        {SC.LeapSlamConfig.SkillKey, typeof(AbilityLeapSlam)},
        {SC.BefuddleConfig.SkillKey, typeof(AbilityBefuddle)},
        {SC.PsyboltConfig.SkillKey, typeof(AbilityPsybolt)},
        {SC.SelfHealConfig.SkillKey, typeof(AbilitySelfHeal)},
        {SC.RegenerationConfig.SkillKey, typeof(AbilityRegeneration)},
        {SC.HypnotizeConfig.SkillKey, typeof(AbilityHypnotize)},
        {SC.PsionicBeamConfig.SkillKey, typeof(AbilityPsionicBeam)},
        {SC.PsyThrowConfig.SkillKey, typeof(AbilityPsyThrow)},
        {SC.InvisibilityConfig.SkillKey, typeof(AbilityInvisible)},
        {SC.LightFeetConfig.SkillKey, typeof(AbilityLightFeet)},
        {SC.ShurikenConfig.SkillKey, typeof(AbilityShuriken)},
        {SC.BearTrapConfig.SkillKey, typeof(AbilityBearTrap)},
        {SC.ShadowStepConfig.SkillKey, typeof(AbilityShadowStep)},
        {SC.BackstabConfig.SkillKey, typeof(AbilityBackstab)}
    };
}