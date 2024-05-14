// Fill in the actual configs in this file.
using AO;
public static partial class SkillConfig
{
    // NOTE: The query dictionary (SkillKey : NodeConfig) is defined at the bottom of this file as 'STConfigQueryDict'
    // You MUST define node config and add it to the query dict.

    #region Node Configs

    /// <summary>
    /// Punch node
    /// </summary>
    public static readonly SkillTreeNodeConfig RootNodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Punch Forward and deals 1.0x damage",
        MaximumLevel = 1,
        NType = NodeType.SkillReplace,
        UpgradeCost = 0,
        UIPosition = new Vector2(420,30), // This will be set to the item's offset value
        SkillKey = "Punch",
        ParentNodeKeys = null,
        ChildrenNodeKeys = new [] {"ShoulderCrash", "HealthBoost", "AttackBoost", "FireFist"},
    };
    
    /// <summary>
    /// ShoulderCrash node
    /// </summary>
    public static readonly SkillTreeNodeConfig ShoulderCrashNodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Dash to a direction and deals 1.2x damage and knockback",
        MaximumLevel = 5,
        NType = NodeType.SkillUnlock,
        UpgradeCost = 0,
        UIPosition = new Vector2(120,120),
        SkillKey = "ShoulderCrash",
        ParentNodeKeys = new [] {"Punch"},
        ChildrenNodeKeys = {},
    };
    
    /// <summary>
    /// HealthBoost node
    /// </summary>
    public static readonly SkillTreeNodeConfig HealthBoostNodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Boost the player's health",
        MaximumLevel = 5,
        NType = NodeType.AttrBoost,
        UpgradeCost = 0,
        UIPosition = new Vector2(800,180),
        SkillKey = "HealthBoost",
        ParentNodeKeys = new [] {"Punch"},
        ChildrenNodeKeys = {},
    };
    
    /// <summary>
    /// AttackBoost node
    /// </summary>
    public static readonly SkillTreeNodeConfig AttackBoostNodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Boost the player's attack",
        MaximumLevel = 5,
        NType = NodeType.AttrBoost,
        UpgradeCost = 0,
        UIPosition = new Vector2(895,220),
        SkillKey = "AttackBoost",
        ParentNodeKeys = new [] {"Punch"},
        ChildrenNodeKeys = {},
    };
    
    /// <summary>
    /// RollOut node
    /// </summary>
    public static readonly SkillTreeNodeConfig RollOutNodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Boost your speed and enable you to crash into other players",
        MaximumLevel = 1,
        NType = NodeType.AttrBoost,
        UpgradeCost = 0,
        UIPosition = new Vector2(770,330),
        SkillKey = "RollOut",
        ParentNodeKeys = new [] {"Punch"},
        ChildrenNodeKeys = {},
    };

    #endregion
    
    public static readonly Dictionary<string, SkillTreeNodeConfig> STConfigQueryDict =
        new Dictionary<string, SkillTreeNodeConfig>()
        {
            {"Punch", RootNodeConfig},
            {"ShoulderCrash", ShoulderCrashNodeConfig},
            {"HealthBoost", HealthBoostNodeConfig},
            {"AttackBoost", AttackBoostNodeConfig},
            {"RollOut", RollOutNodeConfig},
        };
}