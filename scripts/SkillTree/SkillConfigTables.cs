// Fill in the actual configs in this file.
using AO;
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
        UpgradeCost = 0,
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
        UpgradeCost = 0,
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
        UpgradeCost = 0,
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
        UpgradeCost = 0,
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
        UpgradeCost = 0,
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
        UpgradeCost = 10,
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
        UpgradeCost = 10,
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
        UpgradeCost = 10,
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
        UpgradeCost = 10,
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
        UpgradeCost = 10,
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
        UpgradeCost = 10,
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
        UpgradeCost = 10,
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
        UpgradeCost = 0,
        UIPosition = new Vector2(450, 30),
        SkillKey = "SpoonThrow",
        IconPath = "ability_icon_tmp/SpoonThrow_Tmp.png",
        ParentNodeKeys = new string[] {},
        ChildrenNodeKeys = new string[]{"Befuddle", "Psybolt"},
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
        UpgradeCost = 0,
        UIPosition = new Vector2(450, 230),
        SkillKey = "Befuddle",
        IconPath = "ability_icon_tmp/Befuddle_Tmp.png",
        ParentNodeKeys = new string[] {"SpoonThrow"},
        ChildrenNodeKeys = new string[]{},
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
        UpgradeCost = 0,
        UIPosition = new Vector2(110, 230),
        SkillKey = "Psybolt",
        IconPath = "ability_icon_tmp/Befuddle_Tmp.png",
        ParentNodeKeys = new string[] {"SpoonThrow"},
        ChildrenNodeKeys = new string[]{},
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
            {"RollOut", RollOutNodeConfig},
            {"Punch2", PunchTwoConfig},
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
            {"Psybolt", PsyboltConfig}
        };

    // [Add Skill] Item 3: Put Classification Here
    public static readonly HashSet<string> AttrBoostSkills = new HashSet<string>() { "HealthBoost", "AttackBoost"};
    
    public static readonly HashSet<string> ActiveSkills = new HashSet<string>() {"Punch", "RollOut", "Shield", 
        // Brawler
        "ShoulderCrash", "GroundStomp", "Rage", "DoublePunch", "SelfDestruct", "BattleCry", "ClawSlash", "LeapSlam",
        // Psionic
        "SpoonThrow", "Befuddle", "Psybolt"
    };

    public static readonly HashSet<string> ReplacementSkills = new HashSet<string>() {"Punch2"};
    
    public static readonly HashSet<string> SkillEnhanceSkills = new HashSet<string>() {  };
    
    // [Add Skill] Item 4: Go to FightAbility.cs, add the association between skillKey and type of ability.
    
    
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