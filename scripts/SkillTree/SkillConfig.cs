// Definition of skills and related data structs

using AO;

public static partial class SkillConfig
{
    
    public enum NodeType
    {
        AttrBoost,
        SkillUnlock,
        SkillReplace,
        SkillEnhance // Unused for current design
    }
    
    public enum SkillTreeTabs
    {
        Basic,
        Defensive,
        Brawler,
        Elemental,
    }

    public enum StatType
    {
        None,
        MaxHealth,
        BaseSpeed,
        AttackPower
    }

    public struct StatBuff
    {
        public StatType BoostType;
        public float BoostValue;
    }
    
    // These are keys to skills, which associate the skill node to the logics in skill tree nodes.
    // Add keys here when new skills are added
    #region SkillKeys
    
    // [Add Skill] Item 3: Put Classification Here
    public static readonly HashSet<string> AttrBoostSkills = new HashSet<string>() { "HealthBoost", "AttackBoost"};
    
    public static readonly HashSet<string> ActiveSkills = new HashSet<string>() {"Punch", "RollOut", "ShoulderCrash", "FireFist"};

    public static readonly HashSet<string> ReplacementSkills = new HashSet<string>() {"Punch2"};
    
    public static readonly HashSet<string> SkillEnhanceSkills = new HashSet<string>() {  };

    public static HashSet<string> GetAllSkillKeys()
    {
        HashSet<string> allSkills = new HashSet<string>(ActiveSkills);
        allSkills.UnionWith(ReplacementSkills);
        allSkills.UnionWith(AttrBoostSkills);
        allSkills.UnionWith(SkillEnhanceSkills);
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
        public int MaximumLevel; // Not used but put it here for redundancy. This number must be at least 1
        public NodeType NType; // This determines how the skill is going to be handled on gameplay side (unrelated to UI)
        public SkillTreeTabs NTab; // This determines which page in ability vendor and ability book this node belongs to
        public Vector2 UIPosition; // (x,y). Current page center is x = 420

        // Keys are unique for each node
        public string SkillKey;
        public string[] ChildrenNodeKeys;
        public string[] ParentNodeKeys;
        
        // Special Handler needed - If this is marked True, a special handler will be called when the skill is added
        public bool NeedSpecialHandler; // TODO: Implement in SkillHandler
        
        // Stat Type. Fill this if this is a stat buff node. This can only buff one stat, if need multiple or other custom data, implement special handler
        public StatBuff Buff;

        
        // PROGRAMMING NOTICE:
        // In C# structs, empty array (e.g. string[] ChildrenNodeKeys) will be initialized to null.
        // You must do a null check if you want to iterate over it.
        public string[] GetChildrenNodeKeys()
        {
            return ChildrenNodeKeys ?? Array.Empty<string>();
        }

        public string[] GetParentNodeKeys()
        {
            return ParentNodeKeys ?? Array.Empty<string>();
        }
    }
    
}