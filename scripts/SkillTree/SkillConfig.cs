// Definition of skills and related data structs

using AO;

public static partial class SkillConfig
{
    
    public enum NodeType
    {
        AttrBoost,
        SkillUnlock,
        SkillReplace,
        SkillEnhance
    }
    
    #region SkillKeys
    // These are keys to skills, which associate the skill node to the logics in skill tree nodes.
    
    public static readonly HashSet<string> ActiveSkills = new HashSet<string>() {"Punch", "ShoulderCrash", "RollOut", "GroundSlam", 
        "Fireball"};

    public static readonly HashSet<string> ReplacementSkills = new HashSet<string>() { "FireFist" };

    public static readonly HashSet<string> AttrBoostSkills = new HashSet<string>() { "HealthBoost", "AttackBoost"};

    public static readonly HashSet<string> SkillBoostSkills = new HashSet<string>() { };

    public static HashSet<string> GetAllSkillKeys()
    {
        HashSet<string> allSkills = new HashSet<string>(ActiveSkills);
        allSkills.UnionWith(ReplacementSkills);
        allSkills.UnionWith(AttrBoostSkills);
        allSkills.UnionWith(SkillBoostSkills);
        return allSkills;
    }

    #endregion

    
    /// <summary>
    /// The static data for root node. Skill Tree create a structure from these structs and fill in the data from player save.
    /// Fill in configurations in SkillConfigTables.cs
    /// </summary>
    public struct SkillTreeNodeConfig
    {
        public string DescriptionTextKey;
        
        public int UpgradeCost;
        public int MaximumLevel;
        public NodeType NType;
        public Vector2 UIPosition;

        // Keys are unique for each node
        public string SkillKey;
        public string[] ChildrenNodeKeys;
        public string ParentNodeKey;

    }
    
}