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
        Stealth,
        Psionic
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
        public int BoostValue;
    }
    
    // These are keys to skills, which associate the skill node to the logics in skill tree nodes.
    // Add keys here when new skills are added
    #region SkillKeys
    


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
        public string IconPath;
        public string[] ChildrenNodeKeys;
        public string[] ParentNodeKeys;
        
        // Special Handler needed - If this is marked True, a special handler will be called when the skill is added
        public bool NeedSpecialHandler;
        public bool NeedRemover; // Depending on NodeType, a remover will be called from server, in case this skill is removed.
        
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

    public static string GetIconPath(string key)
    {
        if (key == "Empty")
        {
            return FightAbility.DefaultIconPath;
        }
        string path = STConfigQueryDict[key].IconPath;
        return path == String.Empty ? FightAbility.DefaultIconPath : path;
    }

    public static SkillTreeNodeConfig GetConfig(string skillKey)
    {
        if (STConfigQueryDict.TryGetValue(skillKey, out var cfg))
        {
            return cfg;
        }
        else
        {
            Log.Error($"{skillKey} is not found in config!");
            throw new KeyNotFoundException($"{skillKey} is not found in config!");
        }
    }
}