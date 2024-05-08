// Fill in the actual configs in this file.
using AO;
public static partial class SkillConfig
{
    // NOTE: The query dictionary (SkillKey : NodeConfig) is defined at the bottom of this file as 'STConfigQueryDict'
    // You MUST define node config and add it to the query dict for the whole thing to work.

    #region Node Configs

    /// <summary>
    /// Root node
    /// </summary>
    public static readonly SkillTreeNodeConfig RootNodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Punch Forward and deals 1.0x damage",
        MaximumLevel = 1,
        NType = SkillTreeNode.NodeType.SkillUnlock,
        UpgradeCost = 0,
        UIPosition = new Vector2(900,100),
        SkillKey = "Punch",
        ParentNodeKey = null,
        ChildrenNodeKeys = new [] {"ShoulderCrash", "HealthBoost", "AttackBoost", "FireFist"},
    };
    
    /// <summary>
    /// Shoulder Crash node
    /// </summary>
    public static readonly SkillTreeNodeConfig ShoulderCrashNodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Dash to a direction and deals 1.2x damage and knockback",
        MaximumLevel = 5,
        NType = SkillTreeNode.NodeType.SkillUnlock,
        UpgradeCost = 0,
        UIPosition = new Vector2(450,120),
        SkillKey = "ShoulderCrash",
        ParentNodeKey = "Punch",
        ChildrenNodeKeys = {},
    };
    
    /// <summary>
    /// Health Boost node
    /// </summary>
    public static readonly SkillTreeNodeConfig HealthBoostNodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Boost the player's health",
        MaximumLevel = 5,
        NType = SkillTreeNode.NodeType.AttrBoost,
        UpgradeCost = 0,
        UIPosition = new Vector2(800,180),
        SkillKey = "HealthBoost",
        ParentNodeKey = "Punch",
        ChildrenNodeKeys = {},
    };
    
    /// <summary>
    /// Health Boost node
    /// </summary>
    public static readonly SkillTreeNodeConfig AttackBoostNodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Boost the player's attack",
        MaximumLevel = 5,
        NType = SkillTreeNode.NodeType.AttrBoost,
        UpgradeCost = 0,
        UIPosition = new Vector2(1000,180),
        SkillKey = "AttackBoost",
        ParentNodeKey = "Punch",
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
        };
}