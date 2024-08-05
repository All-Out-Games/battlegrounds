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
        Psionic,
        Elemental
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
        public string DisplayName;
        public int BaseDamageKey;
        public string RangeDescriptionKey = "N/A";
        public string CooldownKey = "N/A";
        
        public int UpgradeCost;
        public int MaximumLevel; // Not used but put it here for redundancy. This number must be at least 1
        public NodeType NType; // This determines how the skill is going to be handled on gameplay side (unrelated to UI)
        public SkillTreeTabs NTab; // This determines which page in ability vendor and ability book this node belongs to
        
        public Vector2 UIPosition; // TODO: this field is being retired. 
        public int GridX = 1; // Max = 2
        public int GridY = 0; // Max = 5

        // Keys are unique for each node
        public string SkillKey;
        public string IconPath;
        public string AbilityIconPath; // These icons do not have a backplate, used in ability slots
        public string[] ChildrenNodeKeys;
        public string[] ParentNodeKeys;
        
        // Special Handler needed - If this is marked True, a special handler will be called when the skill is added
        public bool NeedSpecialHandler;
        /// <summary>
        /// Depending on NodeType, a remover will be called from server, in case this skill is removed.
        /// It should be true for stat boosts and punch upgrades, false (default) for others
        /// </summary>
        public bool NeedRemover; 
        
        // Stat Type. Fill this if this is a stat buff node. This can only buff one stat, if need multiple or other custom data, implement special handler
        public StatBuff Buff;

        public SkillTreeNodeConfig()
        {
            DescriptionTextKey = "Description Unfilled, to be updated";
            DisplayName = String.Empty;
            UpgradeCost = 0;
            MaximumLevel = 1;
            NType = NodeType.AttrBoost;
            NTab = SkillTreeTabs.Basic;
            UIPosition = default;
            SkillKey = "Empty";
            IconPath = FightAbility.DefaultIconPath;
            ChildrenNodeKeys = new string[] { };
            ParentNodeKeys = new string[] { };
            NeedSpecialHandler = false;
            NeedRemover = false;
            Buff = default;
        }


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
        
        public string GetDisplayName()
        {
            if (DisplayName == String.Empty)
            {
                return SkillKey;
            }
            else
            {
                return DisplayName;
            }
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

    public static string GetAbilityIconPath(string key)
    {
        if (key == "Empty")
        {
            return FightAbility.DefaultAbilityIcon;
        }
        string path = STConfigQueryDict[key].AbilityIconPath;
        
        if (path == String.Empty)
        {
            path = GetIconPath(key); // No btn texture -> fall back to skill tree icon
        }
        
        return path == String.Empty ? FightAbility.DefaultAbilityIcon : path;
    }

    public static string GetPunchAbilityIconPath(int punchLevel)
    {
        if (punchLevel == 1)
        {
            return GetAbilityIconPath("Punch");
        }
        return GetAbilityIconPath($"Punch{punchLevel}");
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