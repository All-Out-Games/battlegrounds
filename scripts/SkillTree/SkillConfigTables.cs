// Fill in the actual configs in this file.
using AO;
public static partial class SkillConfig
{
    // NOTE: The query dictionary (SkillKey : NodeConfig) is defined at the bottom of this file as 'STConfigQueryDict'
    // You MUST define node config and add it to the query dict.
    
    // [Add Skill] item 1: Config Entry
    #region Node Configs

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
        UIPosition = new Vector2(420,30), // This will be set to the item's offset value
        SkillKey = "Punch",
        ParentNodeKeys = new string[]{},
        ChildrenNodeKeys = new string[] {"HealthBoost", "AttackBoost"},
    };
    
    /// <summary>
    /// ShoulderCrash node
    /// </summary>
    public static readonly SkillTreeNodeConfig ShoulderCrashNodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Dash to a direction and deals 1.2x damage and knockback",
        MaximumLevel = 5,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 0,
        UIPosition = new Vector2(120,120),
        SkillKey = "ShoulderCrash",
        ParentNodeKeys = new string[]{},
        ChildrenNodeKeys = new string[]{},
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
        UIPosition = new Vector2(800,180),
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
        UIPosition = new Vector2(895,220),
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
        NType = NodeType.AttrBoost,
        NTab = SkillTreeTabs.Defensive,
        UpgradeCost = 0,
        UIPosition = new Vector2(770,330),
        SkillKey = "RollOut",
        ParentNodeKeys =  new string[]{},
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
        UIPosition = new Vector2(120,450),
        SkillKey = "Punch2",
        ParentNodeKeys = new string [] {"AttackBoost", "HealthBoost"},
        ChildrenNodeKeys = new string[] {},
    };
    
    /// <summary>
    /// Fire Fist node
    /// </summary>
    public static readonly SkillTreeNodeConfig FireFistConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Boost your speed and enable you to crash into other players",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Elemental,
        UpgradeCost = 0,
        UIPosition = new Vector2(120,450),
        SkillKey = "FireFist",
        ParentNodeKeys = new string[]{},
        ChildrenNodeKeys = new string[]{},
    };

    #endregion
    
    // [Add Skill] item 2: Query Entry
    // MUST ADD for each new skill. This connects the unique skill key to their node config.
    public static readonly Dictionary<string, SkillTreeNodeConfig> STConfigQueryDict =
        new Dictionary<string, SkillTreeNodeConfig>()
        {
            {"Punch", PunchNodeConfig},
            {"ShoulderCrash", ShoulderCrashNodeConfig},
            {"HealthBoost", HealthBoostNodeConfig},
            {"AttackBoost", AttackBoostNodeConfig},
            {"RollOut", RollOutNodeConfig},
            {"Punch2", PunchTwoConfig},
            {"FireFist", FireFistConfig}
        };

    
    // This affects how many pages appear on the ability book and ability vendor. Each page will have a tag that classifies the items.
    public static readonly List<SkillTreeTabs> STConfigTabsList = new List<SkillTreeTabs>()
    {
        SkillTreeTabs.Basic,
        SkillTreeTabs.Brawler,
        SkillTreeTabs.Defensive,
        SkillTreeTabs.Elemental
    };
}