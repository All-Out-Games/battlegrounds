// Definition of skills and related data structs

using AO;
using Assembly.scripts.Effects.ActiveSkills;

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

        /*string st = "";
        foreach (var key in allSkills)
        {
            st += $"{key} ";
        }
        Log.Warn(st);*/
        
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
        public string UpgradeTextKey;
        public string DisplayName; // This field is displayed on info screen as title. If unfilled, will display skillKey
        public int BaseDamageKey; // A fixed number for the base damage of the skill
        public string RangeDescriptionKey = "N/A";
        public string CooldownKey = "N/A";
        
        // SPECIAL VALUES:
        // DescriptionKey / BaseDamageKey / RangeDescriptionKey / CooldownKey are used in 
        // You can fill DescriptionKey / RangeDescriptionKey / CooldownKey with '%OVERRIDE%' to trigger a call of
        // an override function. Pass the skillkey and the local player in it to get the actual number based on their skill level
        // You can fill BaseDamageKey (int) as _overridevalue_ to achieve the same on this field.
        
        // Write the handler in this file. Add your case to the override function
        
        /// <summary>
        /// Coins needed when purchasing this skill
        /// </summary>
        public int UpgradeCost;
        /// <summary>
        /// Not used but put it here for redundancy. This number must be at least 1
        /// </summary>
        public int MaximumLevel; 
        /// <summary>
        /// Skill will be gated from players that are under this level.
        /// IMPORTANT NOTE: The DISPLAYED level on UI is (ACTUAL level + 1). So please fill this field as DISPLAY level - 1
        /// e.g. Punch II unlocks at level 5. You should fill in 4 (not 5)!
        /// </summary>
        public int UnlockLevel; 
        
        public NodeType NType; // The skill type. Active Skill - SkillUnlock, Passive Skill - SkillEnhancement / Others are reserved for special types
        public SkillTreeTabs NTab; // This determines which page in ability vendor and ability book this node belongs to
        
        public Vector2 UIPosition; // TODO: this field is being retired. 
        public int GridX = 1; // Max = 2
        public int GridY = 0; // Max = 5

        /// <summary>
        /// SkillKeys are unique IDs for skill.
        /// </summary>
        public string SkillKey;
        public string IconPath;
        public string AbilityIconPath; // These icons do not have a backplate, used in ability slots
        public string[] ChildrenNodeKeys;
        public string[] ParentNodeKeys;
        
        // Special Handler needed - If this is marked True, a special handler will be called (by Reflection) when the skill is added
        public bool NeedSpecialHandler;
        /// <summary>
        /// Depending on NodeType, a remover will be called from server, in case this skill is removed.
        /// It should be true for stat boosts and punch upgrades, false (default) for others
        /// </summary>
        public bool NeedRemover; 
        
        /// <summary>
        /// Stat Type. Fill this if this is a stat buff node. This can only buff one stat, if need multiple or other custom data, implement special handler
        /// </summary>
        public StatBuff Buff;

        public int[] UpgradeGemCost;

        public SkillTreeNodeConfig()
        {
            DescriptionTextKey = "Description Unfilled, to be updated";
            UpgradeTextKey = "Description Unfilled, to be updated";
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
            UpgradeGemCost = new int[] { };
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

    /// <summary>
    /// Get the icon (backplate version) of the skill
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string GetIconPath(string key)
    {
        if (key == "Empty")
        {
            return FightAbility.DefaultIconPath;
        }
        string path = STConfigQueryDict[key].IconPath;
        return path == String.Empty ? FightAbility.DefaultIconPath : path;
    }

    /// <summary>
    /// Get the icon (non-backplate version) of the skill
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
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

    public static string GetOverrideCooldown(string skillKey, FightPlayer fp)
    {
        SkillTreeNodeConfig cfg = GetConfig(skillKey);
        string res = cfg.CooldownKey; // default
        switch (skillKey)
        {
            case "ShoulderCrash":
                res = $"{AbilityShoulderCrash.GetCooldown(fp)}s";
                break;
            case "DoublePunch":
                res = $"{AbilityDoublePunch.GetCooldown(fp)}s";
                break;
            case "Rage":
                res = $"{AbilityRage.GetCooldown(fp)}s";
                break;
            case "GroundStomp":
                res = $"{AbilityGroundStomp.GetCooldown(fp)}s";
                break;
            case "ClawSlash":
                res = $"{AbilityClawSlash.GetCooldown(fp)}s";
                break;
            case "LeapSlam":
                res = $"{AbilityLeapSlam.GetCooldown(fp)}s";
                break;
            case "SelfDestruct":
                res = $"{AbilitySelfDestruct.GetCooldown(fp)}s";
                break;
            case "Invisibility":
                res = $"{AbilityInvisible.GetCooldown(fp)}s";
                break;
            case "Shuriken":
                res = $"{AbilityShuriken.GetCooldown(fp)}s";
                break;
            case "Backstab":
                res = $"{AbilityBackstab.GetCooldown(fp)}s";
                break;
            case "TotalDarkness":
                res = $"{AbilityTotalDarkness.GetCooldown(fp)}s";
                break;
            case "Shield":
                res = $"{AbilityShield.GetCooldown(fp)}s";
                break;
            case "IronSkin":
                res = $"{AbilityIronSkin.GetCooldown(fp)}s";
                break;
        }

        return res;
    }

    public static int GetOverrideBaseDamage(string skillKey, FightPlayer fp)
    {
        SkillTreeNodeConfig cfg = GetConfig(skillKey);
        int res = cfg.BaseDamageKey; // default
        switch (skillKey)
        {
            case "ShoulderCrash":
                res = EffectConfig.ShoulderCrashConfig.GetDefault(0, fp.GetSkillTree().GetSkillLevel("ShoulderCrash"))
                    .ContactDamage;
                break;
            case "DoublePunch":
                // There are two ways to get the overridden base damage. Calculate here or just get a config with atk=0
                res = EffectConfig.DoublePunchConfig.GetDefault(0, fp.GetSkillTree().GetSkillLevel("DoublePunch"))
                    .PunchDamage;
                break;
            case "Backstab":
                // Note: Do not fill in fp.CurrentAttack. We are looking for the base damage here!
                res = EffectConfig.BackStabConfig.GetDefault(0, fp.GetSkillTree().GetSkillLevel("Backstab")).Damage;
                break;
            case "GroundStomp":
                res = EffectConfig.GroundStompConfig.GetDefault(0, fp.GetSkillTree().GetSkillLevel("GroundStomp")).StompDamage;
                break;
            case "ClawSlash":
                res = EffectConfig.ClawSlashConfig.GetDefault(0, fp.GetSkillTree().GetSkillLevel("ClawSlash"))
                    .SlashDamage;
                break;
            case "BattleCry":
                res = EffectConfig.BattleCryConfig.GetDefault(0, fp.GetSkillTree().GetSkillLevel("BattleCry"))
                    .RoarDamage;
                break;
            case "LeapSlam":
                res = EffectConfig.LeapSlamConfig.GetDefault(0, fp.GetSkillTree().GetSkillLevel("LeapSlam")).SlamDamage;
                break;
            case "SelfDestruct":
                res = EffectConfig.SelfDestructConfig.GetDefault(0, fp.GetSkillTree().GetSkillLevel("SelfDestruct"))
                    .BlastDamage;
                break;
            case "Shuriken":
                res = EffectConfig.ProjectileConfig
                    .GetPlayerShurikenConfig(0, fp.GetSkillTree().GetSkillLevel("Shuriken")).Damage;
                break;
            case "TotalDarkness":
                res = EffectConfig.TotalDarknessConfig.GetConfig(0, fp.GetSkillTree().GetSkillLevel("TotalDarkness"))
                    .Damage;
                break;
        }

        return res;
    }

    public static string GetOverrideDescription(string skillKey, FightPlayer fp)
    {
        SkillTreeNodeConfig cfg = GetConfig(skillKey);
        string res = cfg.CooldownKey; // default
        switch (skillKey)
        {
            case "BearTrap":
                res =
                    $"Place a hidden bear trap that triggers on the next player to walk over it. The trap will last " +
                    $"{EffectConfig.BearTrapConfig.TrapLifeTime + EffectConfig.BearTrapConfig.LifeTimeGrowth * (fp.GetSkillTree().GetSkillLevel("BearTrap")-1)}s seconds on the ground.";
                break;
            case "LightFeet":
                int lflv = fp.GetSkillTree().GetSkillLevel("LightFeet");
                EffectConfig.LightFeetConfig lf =
                    EffectConfig.LightFeetConfig.GetDefault(lflv);
                res =
                    $"Kick it into second gear and temporarily increase your movement speed by {float.Round((lf.SpeedMtp - 1f) * 100, 0)}% for {lf.BuffTime} seconds.";
                if (lflv > 4)
                {
                    res += " Also provides a 10% dodge chance.";
                }
                break;
            case "Rage":
                res =
                    $"Channel your rage and temporarily increase your attack power by {EffectConfig.RageConfig.AtkBoostBase} for {EffectConfig.RageConfig.Duration + (fp.GetSkillTree().GetSkillLevel("Rage") > 4 ? 2 : 0)}s.";
                break;
            case "ClawSlash":
                res =
                    $"Slash with a razor sharp claw damaging and causing bleed. Bleed deals {EffectConfig.ClawSlashConfig.BleedDmgBase} damage per second for {EffectConfig.ClawSlashConfig.BleedTimeBase + (fp.GetSkillTree().GetSkillLevel("ClawSlash") > 4 ? 2 : 0)} seconds.";
                break;
            case "SelfDestruct":
                res = $"Unleash a powerful explosion that damages all nearby enemies, but also deals {EffectConfig.SelfDestructConfig.BaseSelfDmg - (fp.GetSkillTree().GetSkillLevel("SelfDestruct") > 4 ? 10 : 0)} damage to yourself.";
                break;
            case "ShadowStep":
                EffectConfig.ShadowStepConfig sscfg = EffectConfig.ShadowStepConfig.GetDefault(fp.GetSkillTree().GetSkillLevel("ShadowStep"));
                res = "Using your ninja way, teleport a short distance in the direction you're moving." + $" Also grants Shadow Armor for 2s, which blocks {sscfg.ShadowArmorAmount}damage for {sscfg.ShadowArmorEffectiveTime} time(s)";
                break;
            case "Shield":
                EffectConfig.ShieldConfig scfg =
                    EffectConfig.ShieldConfig.GetDefault(fp.GetSkillTree().GetSkillLevel("Shield"));
                res = $"Create a weak wooden shield that absorbs {scfg.ShieldAmt} damage.";
                break;
            case "IronSkin":
                EffectConfig.IronSkinConfig iscfg = EffectConfig.IronSkinConfig.GetDefault(fp.GetSkillTree().GetSkillLevel("IronSkin"));
                res =
                    $"Harden your skin and reduce {float.Round((1f - iscfg.DmgModifer) * 100, 0)}% damage for {iscfg.Duration} seconds.";
                break;
        }

        return res;
    }
}