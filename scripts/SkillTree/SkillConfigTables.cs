// Fill in the actual configs in this file.
using AO;
using SC = SkillConfig;
using Assembly.scripts.Effects.ActiveSkills;

public static partial class SkillConfig
{
    public static readonly int _overrideValue_ = Int32.MinValue;
    public static readonly int[] _tierOneGemCost = new[] { 250, 625, 1500, 4000};
    public static readonly int[] _tierTwoGemCost = new[] { 400, 1000, 2500, 6250};
    
    // NOTE: The query dictionary (SkillKey : NodeConfig) is defined at the bottom of this file as 'STConfigQueryDict'
    // You MUST define node config and add it to the query dict.

    // [Add Skill] item 1: Config Entry

    
    // Template for filling multi-line upgrade text 
    // "Upgrade Effect: \n ★: \n ★★: \n ★★★:\n ★★★★:\n" // STAR U+2605 not supported
    // "*: Damage +1\n **: Cooldown -1\n ***: Damage +1\n ****: Dash Duration/Speed +20%"
    #region Node Configs : Basic

    /// <summary>
    /// Punch node
    /// </summary>
    public static readonly SkillTreeNodeConfig PunchNodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "A basic punch dealing a small amount of damage.",
        BaseDamageKey = EffectConfig.PunchConfig.PunchDmgBase,
        RangeDescriptionKey = "Melee",
        IconPath = "AbilityIcon_Merged/basic/punch.png",
        AbilityIconPath = "AbilityIcon_Separate/basic/punch_icon.png",
        AbilityPreviewPath = "Ability_Preview/basic/punch.gif",
        MaximumLevel = 1,
        NType = NodeType.SkillReplace,
        NTab = SkillTreeTabs.Basic,
        UnlockLevel = 0,
        UpgradeCost = 0,
        UIPosition = new Vector2(450, 30), // This will be set to the item's offset value
        GridX = 1,
        GridY = 0,
        SkillKey = "Punch",
        ParentNodeKeys = new string[] { },
        ChildrenNodeKeys = new string[] { "HealthBoost", "AttackBoost", "Punch2" },
    };

    /// <summary>
    /// HealthBoost node
    /// </summary>
    public static readonly SkillTreeNodeConfig HealthBoostNodeConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Health Boost", 
        DescriptionTextKey = "Increase your base health by 5.",
        MaximumLevel = 1, 
        IconPath = "AbilityIcon_Merged/basic/health_boost.png",
        NeedRemover = true,
        RangeDescriptionKey = "Passive",
        NType = NodeType.AttrBoost,
        NTab = SkillTreeTabs.Basic,
        UnlockLevel = 2,
        UpgradeCost = 300,
        UIPosition = new Vector2(730, 230),
        GridX = 0,
        GridY = 1,
        SkillKey = "HealthBoost",
        ParentNodeKeys = new string[] { "Punch" },
        ChildrenNodeKeys = new string[] { "HealthBoost2" },
        Buff = new StatBuff
        {
            BoostType = StatType.MaxHealth,
            BoostValue = 5
        }
    };

    /// <summary>
    /// AttackBoost node
    /// </summary>
    public static readonly SkillTreeNodeConfig AttackBoostNodeConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Attack Boost",
        DescriptionTextKey = "Increase your base attack power by 1.",
        IconPath = "AbilityIcon_Merged/basic/attack_boost.png",
        MaximumLevel = 1,
        NeedRemover = true,
        RangeDescriptionKey = "Passive",
        NType = NodeType.AttrBoost,
        NTab = SkillTreeTabs.Basic,
        UnlockLevel = 2,
        UpgradeCost = 300,
        UIPosition = new Vector2(110, 230),
        GridX = 2,
        GridY = 1,
        SkillKey = "AttackBoost",
        ParentNodeKeys = new string[] { "Punch" },
        ChildrenNodeKeys = new string[] { "AttackBoost2" },
        Buff = new StatBuff
        {
            BoostType = StatType.AttackPower,
            BoostValue = 1
        }
    };

    /// <summary>
    /// Punch2 node
    /// </summary>
    public static readonly SkillTreeNodeConfig PunchTwoConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Punch II",
        DescriptionTextKey = "A more powerful punch dealing more damage.",
        BaseDamageKey = EffectConfig.PunchConfig.PunchDmgBase + EffectConfig.PunchConfig.PunchDmgGrowth,
        RangeDescriptionKey = "Melee",
        IconPath = "AbilityIcon_Merged/basic/punch_2.png",
        AbilityIconPath = "AbilityIcon_Separate/basic/punch_2_icon.png",
        AbilityPreviewPath = "Ability_Preview/basic/punch2.gif",
        MaximumLevel = 1,
        NeedRemover = true,
        NType = NodeType.SkillReplace,
        NTab = SkillTreeTabs.Basic,
        UnlockLevel = 4,
        UpgradeCost = 800,
        UIPosition = new Vector2(450, 430),
        GridX = 1,
        GridY = 1,
        SkillKey = "Punch2",
        ParentNodeKeys = new string[] { "Punch" },
        ChildrenNodeKeys = new string[] { "Punch3" },
    };

    /// <summary>
    /// HealthBoost node
    /// </summary>
    public static readonly SkillTreeNodeConfig HealthBoost2NodeConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Health Boost II",
        DescriptionTextKey = "Increase your base health by 10",
        IconPath = "AbilityIcon_Merged/basic/health_boost_2.png",
        MaximumLevel = 1,
        NeedRemover = true,
        RangeDescriptionKey = "Passive",
        NType = NodeType.AttrBoost,
        NTab = SkillTreeTabs.Basic,
        UnlockLevel = 9,
        UpgradeCost = 900,
        UIPosition = new Vector2(730, 630),
        GridX = 0,
        GridY = 2,
        SkillKey = "HealthBoost2",
        ParentNodeKeys = new string[] { "HealthBoost" },
        ChildrenNodeKeys = new string[] { },
        Buff = new StatBuff
        {
            BoostType = StatType.MaxHealth,
            BoostValue = 10
        }
    };

    /// <summary>
    /// AttackBoost node
    /// </summary>
    public static readonly SkillTreeNodeConfig AttackBoost2NodeConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Attack Boost II",
        DescriptionTextKey = "Increase your base attack by 2.",
        IconPath = "AbilityIcon_Merged/basic/attack_boost_2.png",
        MaximumLevel = 1,
        NeedRemover = true,
        RangeDescriptionKey = "Passive",
        NType = NodeType.AttrBoost,
        NTab = SkillTreeTabs.Basic,
        UnlockLevel = 9,
        UpgradeCost = 900,
        UIPosition = new Vector2(110, 630),
        GridX = 2,
        GridY = 2,
        SkillKey = "AttackBoost2",
        ParentNodeKeys = new string[] { "AttackBoost" },
        ChildrenNodeKeys = new string[] { },
        Buff = new StatBuff
        {
            BoostType = StatType.AttackPower,
            BoostValue = 2
        }
    };

    /// <summary>
    /// Punch3 node
    /// </summary>
    public static readonly SkillTreeNodeConfig PunchThreeConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Punch III",
        DescriptionTextKey = "An elite punch dealing even more damage.",
        BaseDamageKey = EffectConfig.PunchConfig.PunchDmgBase + EffectConfig.PunchConfig.PunchDmgGrowth * 2,
        RangeDescriptionKey = "Melee",
        IconPath = "AbilityIcon_Merged/basic/punch_3.png",
        AbilityIconPath = "AbilityIcon_Separate/basic/punch_3_icon.png",
        AbilityPreviewPath = "Ability_Preview/basic/punch3.gif",
        UnlockLevel = 19,
        MaximumLevel = 1,
        NeedRemover = true,
        NType = NodeType.SkillReplace,
        NTab = SkillTreeTabs.Basic,
        UpgradeCost = 2400,
        UIPosition = new Vector2(450, 830),
        GridX = 1,
        GridY = 2,
        SkillKey = "Punch3",
        ParentNodeKeys = new string[] { "Punch2" },
        ChildrenNodeKeys = new string[] { },
    };
    
    #endregion

    #region Defensive

    /// <summary>
    /// RollOut node
    /// </summary>
    public static readonly SkillTreeNodeConfig RollOutNodeConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Rollout",
        DescriptionTextKey = "You curl into a ball, rolling at high speed and can crash into other players to deal damage.",
        UpgradeTextKey = "*: Duration +1\n **: Damage +1\n ***: Speed +5%\n ****: Get 10 Shield When Rolling",
        BaseDamageKey = _overrideValue_,
        RangeDescriptionKey = "Melee",
        CooldownKey = $"{EffectConfig.RollOutConfig.Cooldown}s",
        UnlockLevel = 8,
        MaximumLevel = 5,
        UpgradeGemCost = _tierOneGemCost,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Defensive,
        UpgradeCost = 1000,
        UIPosition = new Vector2(750, 330),
        GridX = 1,
        GridY = 1,
        SkillKey = "RollOut",
        IconPath = "AbilityIcon_Merged/defense/rollout.png",
        AbilityIconPath = "AbilityIcon_Separate/defense/rollout_icon.png",
        AbilityPreviewPath = "Ability_Preview/defense/rollout.gif",
        ParentNodeKeys = new string[] { "Shield" },
        ChildrenNodeKeys = new string[] { "GravityCrush" },
    };

    /// <summary>
    /// Shield node
    /// </summary>
    public static readonly SkillTreeNodeConfig ShieldConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "%OVERRIDE%",
        UpgradeTextKey = "*: Cooldown -1\n **: Duration +1\n ***: Duration +1\n ****: Shield +5",
        CooldownKey = "%OVERRIDE%",
        UnlockLevel = 0,
        MaximumLevel = 5,
        UpgradeGemCost = _tierOneGemCost,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Defensive,
        UpgradeCost = 100,
        UIPosition = new Vector2(450, 30),
        GridX = 1,
        GridY = 0,
        SkillKey = "Shield",
        IconPath = "AbilityIcon_Merged/defense/shield.png",
        AbilityIconPath = "AbilityIcon_Separate/defense/shield_icon.png",
        AbilityPreviewPath = "Ability_Preview/defense/shield.gif",
        ParentNodeKeys = new string[] { },
        ChildrenNodeKeys = new string[] { "RollOut", "IronSkin", "HealthBoostD" },
    };

    public static readonly SkillTreeNodeConfig HealthBoostDNodeConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Health Boost",
        DescriptionTextKey = "Increase your base health by 5.",
        UnlockLevel = 4,
        MaximumLevel = 1,
        IconPath = "AbilityIcon_Merged/basic/health_boost.png",
        NeedRemover = true,
        RangeDescriptionKey = "Passive",
        NType = NodeType.AttrBoost,
        NTab = SkillTreeTabs.Defensive,
        UpgradeCost = 750,
        UIPosition = new Vector2(730, 230),
        GridX = 0,
        GridY = 1,
        SkillKey = "HealthBoostD",
        ParentNodeKeys = new string[] { "Shield" },
        ChildrenNodeKeys = new string[] {  },
        Buff = new StatBuff
        {
            BoostType = StatType.MaxHealth,
            BoostValue = 5
        }
    };
    
    public static readonly SkillTreeNodeConfig IronSkinConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Iron Aura",
        DescriptionTextKey = "%OVERRIDE%",
        UpgradeTextKey = "*: Cooldown -1\n **: Duration +1\n ***: Duration +1\n ****: Resist +5%",
        CooldownKey = "%OVERRIDE%",
        UnlockLevel = 6,
        MaximumLevel = 5,
        UpgradeGemCost = _tierOneGemCost,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Defensive,
        UpgradeCost = 900,
        UIPosition = new Vector2(450, 30),
        GridX = 2,
        GridY = 1,
        SkillKey = "IronSkin",
        IconPath = "AbilityIcon_Merged/defense/iron_aura.png",
        AbilityIconPath = "AbilityIcon_Separate/defense/iron_aura_icon.png",
        AbilityPreviewPath = "Ability_Preview/defense/iron_aura.gif",
        ParentNodeKeys = new string[] { "Shield" },
        ChildrenNodeKeys = new string[] { "SpikeShield" },
    };

    public static readonly SkillTreeNodeConfig SpikeShieldConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Spike Guard",
        DescriptionTextKey = "%OVERRIDE%",
        UpgradeTextKey = "*: Cooldown -1\n **: Duration +1\n ***: Duration +1\n ****: Reflection +10%",
        CooldownKey = "%OVERRIDE%",
        UnlockLevel = 16,
        MaximumLevel = 5,
        UpgradeGemCost = _tierTwoGemCost,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Defensive,
        UpgradeCost = 2550,
        GridX = 2,
        GridY = 2,
        SkillKey = "SpikeShield",
        IconPath = "AbilityIcon_Merged/defense/spike_guard.png",
        AbilityIconPath = "AbilityIcon_Separate/defense/spike_guard.png",
        AbilityPreviewPath = "Ability_Preview/defense/spike_guard.gif",
        ParentNodeKeys = new string[] { "IronSkin" },
        ChildrenNodeKeys = new string[] { },
    };

    public static readonly SkillTreeNodeConfig GravityCrushConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Gravity Crush",
        DescriptionTextKey = "%OVERRIDE%",
        RangeDescriptionKey = "%OVERRIDE%",
        UpgradeTextKey = "*: Cooldown -1\n **: Duration +1\n ***: Duration +1\n ****: Field Size +20%",
        CooldownKey = "%OVERRIDE%",
        UnlockLevel = 20,
        MaximumLevel = 5,
        UpgradeGemCost = _tierTwoGemCost,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Defensive,
        UpgradeCost = 2900,
        GridX = 1,
        GridY = 2,
        SkillKey = "GravityCrush",
        IconPath = "AbilityIcon_Merged/defense/gravity_crush.png",
        AbilityIconPath = "AbilityIcon_Separate/defense/gravity_crush_icon.png",
        AbilityPreviewPath = "Ability_Preview/defense/gravity_crush.gif",
        ParentNodeKeys = new string[] { "RollOut" },
        ChildrenNodeKeys = new string[] { "Parry" },
    };

    public static readonly SkillTreeNodeConfig ParryConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Parry",
        DescriptionTextKey =
            "Parry your opponents' attack for a short amount of time, followed by a powerful counterattack that also reflects projectiles.",
        RangeDescriptionKey = "3m",
        BaseDamageKey = _overrideValue_,
        UpgradeTextKey = "*: Cooldown -1\n **: Damage +1\n ***: Damage +1\n ****: Parry Time +50%",
        CooldownKey = "%OVERRIDE%",
        UnlockLevel = 31,
        MaximumLevel = 5,
        UpgradeGemCost = _tierTwoGemCost,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Defensive,
        UpgradeCost = 7700,
        GridX = 1,
        GridY = 3,
        SkillKey = "Parry",
        IconPath = "AbilityIcon_Merged/defense/parry.png",
        AbilityIconPath = "AbilityIcon_Separate/defense/parry_icon.png",
        AbilityPreviewPath = "Ability_Preview/defense/parry.gif",
        ParentNodeKeys = new string[] { "GravityCrush" },
        ChildrenNodeKeys = new string[] { },
    };

    #endregion

    #region NodeConfig: Brawler

    /// <summary>
    /// ShoulderCrash node
    /// </summary>
    public static readonly SkillTreeNodeConfig ShoulderCrashNodeConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Shoulder Crash",
        DescriptionTextKey = "Charge with your shoulder dealing damage and knockback to other players.",
        UpgradeTextKey = "*: Damage +1\n **: Cooldown -1\n ***: Damage +1\n ****: Dash Distance +20%", 
        BaseDamageKey = _overrideValue_,
        RangeDescriptionKey = "5m",
        CooldownKey = "%OVERRIDE%",
        UnlockLevel = 0,
        MaximumLevel = 5,
        UpgradeGemCost = _tierOneGemCost,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 100,
        UIPosition = new Vector2(450, 30),
        GridX = 1,
        GridY = 0,
        SkillKey = "ShoulderCrash",
        IconPath = "AbilityIcon_Merged/brawler/shoulder_crash.png",
        AbilityIconPath = "AbilityIcon_Separate/brawler/shoulder_crash_icon.png",
        AbilityPreviewPath = "Ability_Preview/brawler/shoulder_crash.gif",
        ParentNodeKeys = new string[] { },
        ChildrenNodeKeys = new string[] { "GroundStomp", "Rage", "DoublePunch" },
    };

    public static readonly SkillTreeNodeConfig GroundStompConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Ground Stomp",
        DescriptionTextKey = "Stomp the ground in anger dealing damage to nearby players.",
        UpgradeTextKey = "*: Damage +1\n **: Cooldown -1\n ***: Damage +1\n ****: Crater Size +25%", 
        BaseDamageKey = _overrideValue_,
        RangeDescriptionKey = $"{EffectConfig.GroundStompConfig.StompRadius}m",
        CooldownKey = "%OVERRIDE%",
        IconPath = "AbilityIcon_Merged/brawler/ground_stomp.png",
        AbilityIconPath = "AbilityIcon_Separate/brawler/ground_stomp_icon.png",
        AbilityPreviewPath = "Ability_Preview/brawler/ground_stomp.gif",
        UnlockLevel = 8,
        MaximumLevel = 5,
        UpgradeGemCost = _tierOneGemCost,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 900,
        UIPosition = new Vector2(730, 230),
        GridX = 0,
        GridY = 1,
        SkillKey = "GroundStomp",
        ParentNodeKeys = new string[] { "ShoulderCrash" },
        ChildrenNodeKeys = new string[] { "LeapSlam" },
    };

    public static readonly SkillTreeNodeConfig RageConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "%OVERRIDE%",
        UpgradeTextKey = "*: Cooldown -1\n **: Cooldown -1\n ***: Cooldown -1\n ****: Duration +2",
        IconPath = "AbilityIcon_Merged/brawler/rage.png",
        AbilityIconPath = "AbilityIcon_Separate/brawler/rage_icon.png",
        AbilityPreviewPath = "Ability_Preview/brawler/rage.gif",
        CooldownKey = "%OVERRIDE%",
        UnlockLevel = 4,
        MaximumLevel = 5,
        UpgradeGemCost = _tierOneGemCost,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 750,
        UIPosition = new Vector2(450, 230),
        GridX = 1,
        GridY = 1,
        SkillKey = "Rage",
        ParentNodeKeys = new string[] { "ShoulderCrash" },
        ChildrenNodeKeys = new string[] { "BattleCry" },
    };

    public static readonly SkillTreeNodeConfig DoublePunchConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Double Punch",
        DescriptionTextKey = "The ol' one, two combo. Strike another player with a double punch.",
        UpgradeTextKey = "*: Damage +1\n **: Damage +1\n ***: Damage +1\n ****: Cooldown -1",
        BaseDamageKey = _overrideValue_,
        RangeDescriptionKey = "Melee",
        CooldownKey = "%OVERRIDE%",
        IconPath = "AbilityIcon_Merged/brawler/double_punch.png",
        AbilityIconPath = "AbilityIcon_Separate/brawler/double_punch_icon.png",
        AbilityPreviewPath = "Ability_Preview/brawler/double_punch.gif",
        UnlockLevel = 2,
        MaximumLevel = 5,
        UpgradeGemCost = _tierOneGemCost,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 600,
        UIPosition = new Vector2(170, 230),
        GridX = 2,
        GridY = 1,
        SkillKey = "DoublePunch",
        ParentNodeKeys = new string[] { "ShoulderCrash" },
        ChildrenNodeKeys = new string[] {"ClawSlash" },
    };

    public static readonly SkillTreeNodeConfig SelfDestructConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Self-Destruct",
        UpgradeTextKey = "*: Cooldown -1\n **: Damage +5\n ***: Cooldown -1\n ****: Self Damage -10",
        DescriptionTextKey = "%OVERRIDE%",
        BaseDamageKey = _overrideValue_,
        CooldownKey = "%OVERRIDE%",
        RangeDescriptionKey = $"{EffectConfig.SelfDestructConfig.BlastRange}m",
        IconPath = "AbilityIcon_Merged/brawler/self_destruct.png",
        AbilityIconPath = "AbilityIcon_Separate/brawler/self_destruct_icon.png",
        AbilityPreviewPath = "Ability_Preview/brawler/self_destruct.gif",
        UnlockLevel = 25,
        MaximumLevel = 5,
        UpgradeGemCost = _tierTwoGemCost,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 6750,
        UIPosition = new Vector2(240, 430),
        GridX = 1,
        GridY = 3,
        SkillKey = "SelfDestruct",
        ParentNodeKeys = new string[] { "BattleCry" },
        ChildrenNodeKeys = new string[] { },
    };

    public static readonly SkillTreeNodeConfig BattleCryConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Battle Cry",
        DescriptionTextKey = "Roar with all your might, creating a sound wave damaging and stunning all players nearby.",
        UpgradeTextKey = "*: Damage +1\n **: Damage +1\n ***: Damage +1\n ****: Sound Wave Size +25%",
        BaseDamageKey = _overrideValue_,
        RangeDescriptionKey = $"{EffectConfig.BattleCryConfig.RoarRadius}m",
        CooldownKey = $"{EffectConfig.BattleCryConfig.Cooldown}s",
        IconPath = "AbilityIcon_Merged/brawler/battle_cry.png",
        AbilityIconPath = "AbilityIcon_Separate/brawler/battle_cry_icon.png",
        AbilityPreviewPath = "Ability_Preview/brawler/battle_cry.gif",
        UnlockLevel = 15,
        MaximumLevel = 5,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 2250,
        UpgradeGemCost = _tierTwoGemCost,
        UIPosition = new Vector2(450, 430),
        GridX = 1,
        GridY = 2,
        SkillKey = "BattleCry",
        ParentNodeKeys = new string[] { "Rage" },
        ChildrenNodeKeys = new string[] { "SelfDestruct" },
    };

    public static readonly SkillTreeNodeConfig ClawSlashConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Claw Slash",
        DescriptionTextKey = "%OVERRIDE%",
        UpgradeTextKey = "*: Damage +1\n **: Cooldown -1\n ***: Damage +1\n ****: Bleed Time +2", 
        BaseDamageKey = _overrideValue_,
        RangeDescriptionKey = "Melee",
        CooldownKey = "%OVERRIDE%",
        IconPath = "AbilityIcon_Merged/brawler/claw_slash.png",
        AbilityIconPath = "AbilityIcon_Separate/brawler/claw_slash_icon.png",
        AbilityPreviewPath = "Ability_Preview/brawler/claw_slash.gif",
        UnlockLevel = 12,
        MaximumLevel = 5,
        UpgradeGemCost = _tierTwoGemCost,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 1800,
        UIPosition = new Vector2(10, 430),
        GridX = 2,
        GridY = 2,
        SkillKey = "ClawSlash",
        ParentNodeKeys = new string[] { "DoublePunch" },
        ChildrenNodeKeys = new string[] { "ClawSlash" },
    };
    
    public static readonly SkillTreeNodeConfig DualClawConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Dual Claw",
        DescriptionTextKey = $"Upgrade your slash so that you can use Claw Slash again for {EffectConfig.ClawSlashConfig.DualClawTime} seconds after slash the first time.",
        RangeDescriptionKey = "Passive",
        IconPath = "AbilityIcon_Merged/brawler/dual_claw.png",
        AbilityPreviewPath = "Ability_Preview/brawler/dual_claw.gif",
        UnlockLevel = 22,
        MaximumLevel = 1,
        NType = NodeType.SkillEnhance,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 5400,
        UIPosition = new Vector2(10, 430),
        GridX = 2,
        GridY = 3,
        SkillKey = "DualClaw",
        ParentNodeKeys = new string[] { "ClawSlash" },
        ChildrenNodeKeys = new string[] { },
    };

    public static readonly SkillTreeNodeConfig LeapSlamConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Leap Slam",
        DescriptionTextKey = "Leap forward and slam the ground with your fists, damaging and knocking away other players.",
        UpgradeTextKey= "*: Damage +1\n **: Cooldown -1\n ***: Cooldown -1\n ****: Slam Area +25%",
        BaseDamageKey = _overrideValue_,
        RangeDescriptionKey = "5m",
        CooldownKey = $"{EffectConfig.LeapSlamConfig.Cooldown}s",
        IconPath = "AbilityIcon_Merged/brawler/leaping_fist_slam.png",
        AbilityIconPath = "AbilityIcon_Separate/brawler/leaping_fist_slam_icon.png",
        AbilityPreviewPath = "Ability_Preview/brawler/leaping_fist_slam.gif",
        UnlockLevel = 17,
        MaximumLevel = 5,
        UpgradeGemCost = _tierTwoGemCost,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 2700,
        UIPosition = new Vector2(730, 430),
        GridX = 0,
        GridY = 2,
        SkillKey = "LeapSlam",
        ParentNodeKeys = new string[] { "GroundStomp" },
        ChildrenNodeKeys = new string[] { },
    };

    #endregion

    #region NodeConfig: Psionic

    /// <summary>
    /// Spoon Throw
    /// </summary>
    public static readonly SkillTreeNodeConfig SpoonThrowConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Spoon Throw",
        DescriptionTextKey = "Fire an energy spoon in a direction that deals damage.",
        UpgradeTextKey = "*: Cooldown -1\n **: Damage +1\n ***: Damage +1\n ****: Range +2",
        BaseDamageKey = _overrideValue_,
        RangeDescriptionKey = "%OVERRIDE%",
        CooldownKey = "%OVERRIDE%",
        IconPath = "AbilityIcon_Merged/psionic/spoon_throw.png",
        AbilityIconPath = "AbilityIcon_Separate/psionic/spoon_throw_icon.png",
        AbilityPreviewPath = "Ability_Preview/psionic/spoon_throw.gif",
        UnlockLevel = 0,
        MaximumLevel = 5,
        UpgradeGemCost = _tierOneGemCost,
        GridX = 1,
        GridY = 0,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 100,
        UIPosition = new Vector2(450, 30),
        SkillKey = "SpoonThrow",
        ParentNodeKeys = new string[] { },
        ChildrenNodeKeys = new string[] { "Befuddle", "Psybolt", "SelfHeal" },
    };

    /// <summary>
    /// Befuddle
    /// </summary>
    public static readonly SkillTreeNodeConfig BefuddleConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Befuddle",
        DescriptionTextKey = $"Project a cloud of energy that confuses enemy for {EffectConfig.ProjectileConfig.BefuddleConfusionTime}s.",
        UpgradeTextKey = "*: Cooldown -1\n **: Cooldown -1\n ***: Cooldown -1\n ****: Range +2",
        BaseDamageKey = EffectConfig.ProjectileConfig.BefuddleDamageBase,
        RangeDescriptionKey = "%OVERRIDE%",
        CooldownKey = "%OVERRIDE%",
        IconPath = "AbilityIcon_Merged/psionic/befuddle.png",
        AbilityIconPath = "AbilityIcon_Separate/psionic/befuddle_icon.png",
        AbilityPreviewPath = "Ability_Preview/psionic/befuddle.gif",
        UnlockLevel = 5,
        MaximumLevel = 5,
        UpgradeGemCost = _tierOneGemCost,
        GridX = 0,
        GridY = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 850,
        UIPosition = new Vector2(450, 230),
        SkillKey = "Befuddle",
        ParentNodeKeys = new string[] { "SpoonThrow" },
        ChildrenNodeKeys = new string[] { "Hypnotize" },
    };

    /// <summary>
    /// Psybolt
    /// </summary>
    public static readonly SkillTreeNodeConfig PsyboltConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Fire a bolt of energy that deals damage.",
        UpgradeTextKey = "*: Damage +1 \n **: Cooldown -1\n ***: Damage +1\n ****: Knockback +20%",
        BaseDamageKey = _overrideValue_,
        RangeDescriptionKey = $"{EffectConfig.ProjectileConfig.PsyboltRange}m",
        CooldownKey = "%OVERRIDE%",
        IconPath = "AbilityIcon_Merged/psionic/psybolt.png",
        AbilityIconPath = "AbilityIcon_Separate/psionic/psybolt_icon.png",
        AbilityPreviewPath = "Ability_Preview/psionic/psybolt.gif",
        UnlockLevel = 6,
        MaximumLevel = 5,
        UpgradeGemCost = _tierOneGemCost,
        GridX = 1,
        GridY = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 900,
        UIPosition = new Vector2(170, 230),
        SkillKey = "Psybolt",
        ParentNodeKeys = new string[] { "SpoonThrow" },
        ChildrenNodeKeys = new string[] { "PsionicBeam" },
    };

    /// <summary>
    /// SelfHeal
    /// </summary>
    public static readonly SkillTreeNodeConfig SelfHealConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Self-Heal",
        DescriptionTextKey = "%OVERRIDE%",
        UpgradeTextKey = "*: Cooldown -1\n **: Cooldown -1\n ***: Cooldown -1\n ****: Heal +5",
        CooldownKey = "%OVERRIDE%",
        IconPath = "AbilityIcon_Merged/psionic/self_heal.png",
        AbilityIconPath = "AbilityIcon_Separate/psionic/self_heal_icon.png",
        AbilityPreviewPath = "Ability_Preview/psionic/self_heal.gif",
        UnlockLevel = 2,
        MaximumLevel = 5,
        UpgradeGemCost = _tierOneGemCost,
        GridX = 2,
        GridY = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 650,
        UIPosition = new Vector2(730, 230),
        SkillKey = "SelfHeal",
        ParentNodeKeys = new string[] { "SpoonThrow" },
        ChildrenNodeKeys = new string[] { "Regeneration" },
    };
    
    public static readonly SkillTreeNodeConfig ConcentrateConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Concentration",
        DescriptionTextKey = $"Gain more health from Self-Heal and Regeneration. If Self-Heal is interrupted, recover some health and gain Rage for {EffectConfig.SelfHealConfig.ConcentrateRageTime}s.",
        IconPath = "AbilityIcon_Merged/psionic/concentration.png",
        RangeDescriptionKey = "Passive",
        UnlockLevel = 21,
        MaximumLevel = 1,
        GridX = 2,
        GridY = 3,
        NType = NodeType.SkillEnhance,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 5850,
        UIPosition = new Vector2(730, 230),
        SkillKey = "Concentrate",
        ParentNodeKeys = new string[] { "Regeneration" },
        ChildrenNodeKeys = new string[] {  },
    };

    /// <summary>
    /// Regenerate
    /// </summary>
    public static readonly SkillTreeNodeConfig RegenerationConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "%OVERRIDE%",
        UpgradeTextKey = "*: Cooldown -1\n **: Cooldown -1\n ***: Duration +1\n ****: 3% Speed Boost During Healing",
        CooldownKey = "%OVERRIDE%",
        IconPath = "AbilityIcon_Merged/psionic/regeneration.png",
        AbilityIconPath = "AbilityIcon_Separate/psionic/regeneration_icon.png",
        AbilityPreviewPath = "Ability_Preview/psionic/regeneration.gif",
        UnlockLevel = 11,
        MaximumLevel = 5,
        UpgradeGemCost = _tierTwoGemCost,
        GridX = 2,
        GridY = 2,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 1950,
        UIPosition = new Vector2(730, 430),
        SkillKey = "Regeneration",
        ParentNodeKeys = new string[] { "SelfHeal" },
        ChildrenNodeKeys = new string[] { "Concentrate" },
    };


    public static readonly SkillTreeNodeConfig HypnotizeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "%OVERRIDE%",
        UpgradeTextKey = "*: Cooldown -1\n **: Cooldown -1\n ***: Cooldown -1\n ****: Sleep Time +1",
        RangeDescriptionKey = "8m",
        CooldownKey = "%OVERRIDE%",
        IconPath = "AbilityIcon_Merged/psionic/hypnotize.png",
        AbilityIconPath = "AbilityIcon_Separate/psionic/hypnotize_icon.png",
        AbilityPreviewPath = "Ability_Preview/psionic/hypnotize.gif",
        UnlockLevel = 14,
        MaximumLevel = 5,
        UpgradeGemCost = _tierTwoGemCost,
        GridX = 0,
        GridY = 2,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 2550,
        UIPosition = new Vector2(450, 430),
        SkillKey = "Hypnotize",
        ParentNodeKeys = new string[] { "Befuddle" },
        ChildrenNodeKeys = new string[] { "PsyThrow" },

    };

    public static readonly SkillTreeNodeConfig PsionicBeamConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Psionic Beam",
        DescriptionTextKey = "Unleash a devastating energy beam that sweeps a small area dealing damage and causing an explosion.",
        UpgradeTextKey = "*: Damage +1\n **: Cooldown -1\n ***: Damage +1\n ****: Targets will explode for 10% extra damage in 2 seconds",
        BaseDamageKey = _overrideValue_,
        CooldownKey = "%OVERRIDE%",
        RangeDescriptionKey = "3-7m",
        IconPath = "AbilityIcon_Merged/psionic/psionic_beam.png",
        AbilityIconPath = "AbilityIcon_Separate/psionic/psionic_beam_icon.png",
        AbilityPreviewPath = "Ability_Preview/psionic/psionic_beam.gif",
        UnlockLevel = 19,
        MaximumLevel = 5,
        UpgradeGemCost = _tierTwoGemCost,
        GridX = 1,
        GridY = 2,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 2700,
        UIPosition = new Vector2(10, 430),
        SkillKey = "PsionicBeam",
        ParentNodeKeys = new string[] { "Psybolt" },
        ChildrenNodeKeys = new string[] { "Psychic" },

    };

    public static readonly SkillTreeNodeConfig PsyThrowConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Psythrow",
        DescriptionTextKey = "Grab a target with your telekinetic power. Reactivate to throw them in a direction of your choice.",
        UpgradeTextKey = "*: Cooldown -1\n **: Damage +1\n ***: Damage +1\n ****: Your target explodes for 20% extra damage",
        BaseDamageKey = _overrideValue_,
        RangeDescriptionKey = $"{EffectConfig.PsyThrowConfig.Range}m",
        CooldownKey = "%OVERRIDE%",
        IconPath = "AbilityIcon_Merged/psionic/psythrow.png",
        AbilityIconPath = "AbilityIcon_Separate/psionic/psythrow_icon.png",
        AbilityPreviewPath = "Ability_Preview/psionic/psythrow.gif",
        UnlockLevel = 24,
        MaximumLevel = 5,
        UpgradeGemCost = _tierTwoGemCost,
        GridX = 0,
        GridY = 3,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 7650,
        UIPosition = new Vector2(240, 430),
        SkillKey = "PsyThrow",
        ParentNodeKeys = new string[] { "Hypnotize" },
        ChildrenNodeKeys = new string[] { },

    };

    public static readonly SkillTreeNodeConfig PsychicConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Psychic Mastery",
        DescriptionTextKey =
            "Become one with your powers! \n Psybolt: Enhance size and knockback; \n Psionic Beam: Absorbs health.",
        RangeDescriptionKey = "Passive",
        IconPath = "AbilityIcon_Merged/psionic/psychic_mastery.png",
        AbilityPreviewPath = "Ability_Preview/psionic/psychic_mastery.gif",
        UnlockLevel = 29,
        MaximumLevel = 1,
        GridX = 1,
        GridY = 3,
        NType = NodeType.SkillEnhance,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 8100,
        UIPosition = new Vector2(10, 430),
        SkillKey = "Psychic",
        ParentNodeKeys = new string[] { "PsionicBeam" },
        ChildrenNodeKeys = new string[] { },
    };

    #endregion

    #region NodeConfig: Stealth

    public static readonly SkillTreeNodeConfig InvisibilityConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = $"Turn invisible for {EffectConfig.InvisibilityConfig.InvisTime} seconds. Being hit by a projectile will end your invisibility early!",
        UpgradeTextKey = "*: Cooldown -1\n **: Cooldown -1\n ***: Duration +1\n ****: Boost Speed +10% During Effect",
        CooldownKey = "%OVERRIDE%",
        IconPath = "AbilityIcon_Merged/stealth/invisibility.png",
        AbilityIconPath = "AbilityIcon_Separate/stealth/invisibility_icon.png",
        AbilityPreviewPath = "Ability_Preview/stealth/invisibility.gif",
        UnlockLevel = 0,
        MaximumLevel = 5,
        UpgradeGemCost = _tierOneGemCost,
        GridX = 1,
        GridY = 0,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 100,
        UIPosition = new Vector2(450, 30),
        SkillKey = "Invisibility",
        ParentNodeKeys = new string[] { },
        ChildrenNodeKeys = new string[] { "LightFeet", "SpeedBoost", "Shuriken" },

    };
    
    public static readonly SkillTreeNodeConfig SpeedBoostConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Speed Boost",
        DescriptionTextKey = "Increase your base movement speed by 5%.",
        IconPath = "AbilityIcon_Merged/stealth/speed_boost.png",
        UnlockLevel = 1,
        MaximumLevel = 1,
        GridX = 0,
        GridY = 1,
        NType = NodeType.AttrBoost,
        NTab = SkillTreeTabs.Stealth,
        RangeDescriptionKey = "Passive",
        UpgradeCost = 550,
        UIPosition = new Vector2(450, 30),
        SkillKey = "SpeedBoost",
        ParentNodeKeys = new string[] { "Invisibility" },
        ChildrenNodeKeys = new string[] { "BearTrap" },
        Buff = new StatBuff()
        {
            BoostType = StatType.BaseSpeed,
            BoostValue = 4 // Your base speed is 80
        }

    };

    public static readonly SkillTreeNodeConfig LightFeetConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Light Feet",
        DescriptionTextKey = "%OVERRIDE%",
        UpgradeTextKey = "*: Speed +3% Duration +0.5\n **: Speed +3% Duration +0.5\n ***: Speed +3% Duration +0.5\n ****: 10% Dodge Chance During Effect",
        IconPath = "AbilityIcon_Merged/stealth/light_feet.png",
        AbilityIconPath = "AbilityIcon_Separate/stealth/light_feet_icon.png",
        AbilityPreviewPath = "Ability_Preview/stealth/light_feet.gif",
        CooldownKey = $"{EffectConfig.LightFeetConfig.Cooldown}s",
        UnlockLevel = 3,
        MaximumLevel = 5,
        UpgradeGemCost = new [] {250,625,1500,4000},
        GridX = 2,
        GridY = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 650,
        UIPosition = new Vector2(450, 230),
        SkillKey = "LightFeet",
        ParentNodeKeys = new string[] { "Invisibility" },
        ChildrenNodeKeys = new string[] { "ShadowStep" },

    };

    public static readonly SkillTreeNodeConfig ShurikenConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Throw a shuriken dealing damage to other players, critically striking if you hit the target from behind.",
        UpgradeTextKey = "*: Damage +1 \n **: Cooldown -1\n ***: Damage +1\n ****: 20% More Crit Damage",
        BaseDamageKey = _overrideValue_,
        RangeDescriptionKey = $"{EffectConfig.ProjectileConfig.ShurikenRange}m",
        CooldownKey = "%OVERRIDE%",
        IconPath = "AbilityIcon_Merged/stealth/shuriken.png",
        AbilityIconPath = "AbilityIcon_Separate/stealth/shuriken_icon.png",
        AbilityPreviewPath = "Ability_Preview/stealth/shuriken.gif",
        UnlockLevel = 7,
        MaximumLevel = 5,
        UpgradeGemCost = _tierOneGemCost,
        GridX = 1,
        GridY = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 900,
        UIPosition = new Vector2(450, 430),
        SkillKey = "Shuriken",
        ParentNodeKeys = new string[] { "Invisibility" },
        ChildrenNodeKeys = new string[] { "Backstab" },
    };

    public static readonly SkillTreeNodeConfig BearTrapConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Bear Trap",
        DescriptionTextKey = "%OVERRIDE%",
        UpgradeTextKey = $"*: Trap Lifetime +1.5 \n **: Trap Lifetime +1.5\n ***: Trap Lifetime +1.5\n ****: 15% More Trap Size",
        BaseDamageKey = EffectConfig.BearTrapConfig.TrapBaseDamage,
        RangeDescriptionKey = $"{EffectConfig.BearTrapConfig.MaxSetupDistance}m",
        CooldownKey = $"{EffectConfig.BearTrapConfig.Cooldown}s",
        UnlockLevel = 10,
        MaximumLevel = 5,
        UpgradeGemCost = new [] {250,625,1500,4000},
        GridX = 0,
        GridY = 2,
        IconPath = "AbilityIcon_Merged/stealth/bear_trap.png",
        AbilityIconPath = "AbilityIcon_Separate/stealth/bear_trap_icon.png",
        AbilityPreviewPath = "Ability_Preview/stealth/bear_trap.gif",
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 1650,
        UIPosition = new Vector2(450, 630),
        SkillKey = "BearTrap",
        ParentNodeKeys = new string[] { "SpeedBoost" },
        ChildrenNodeKeys = new string[] { },
    };

    public static readonly SkillTreeNodeConfig ShadowStepConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Shadow Step",
        UpgradeTextKey = "*: Shadow Armor +2 \n **: Shadow Armor +2\n ***: Shadow Armor +2\n ****: Shadow Armor blocks damage twice",
        DescriptionTextKey = "%OVERRIDE%",
        RangeDescriptionKey = $"{EffectConfig.ShadowStepConfig.MovementDistance}m",
        CooldownKey = $"{EffectConfig.ShadowStepConfig.Cooldown}s",
        IconPath = "AbilityIcon_Merged/stealth/shadow_step.png",
        AbilityIconPath = "AbilityIcon_Separate/stealth/shadow_step_icon.png",
        AbilityPreviewPath = "Ability_Preview/stealth/shadow_step.gif",
        UnlockLevel = 13,
        MaximumLevel = 5,
        UpgradeGemCost = _tierTwoGemCost,
        GridX = 2,
        GridY = 2,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 1950,
        UIPosition = new Vector2(230, 430),
        SkillKey = "ShadowStep",
        ParentNodeKeys = new string[] { "LightFeet" },
        ChildrenNodeKeys = new string[] { "TotalDarkness" },
    };

    public static readonly SkillTreeNodeConfig BackstabConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Throw A Kunai. If it hits another player, you teleport behind them and stab the target.",
        UpgradeTextKey = "*: Damage +1 \n **: Damage +1\n ***: Cooldown -1\n ****: 50% Life Steal",
        BaseDamageKey = _overrideValue_,
        RangeDescriptionKey = $"{EffectConfig.BackStabConfig.KunaiRange}m",
        CooldownKey = "%OVERRIDE%",
        IconPath = "AbilityIcon_Merged/stealth/backstab.png",
        AbilityIconPath = "AbilityIcon_Separate/stealth/backstab_icon.png",
        AbilityPreviewPath = "Ability_Preview/stealth/backstab.gif",
        UnlockLevel = 18,
        MaximumLevel = 5,
        UpgradeGemCost = _tierTwoGemCost,
        GridX = 1,
        GridY = 2,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 2700,
        UIPosition = new Vector2(230, 630),
        SkillKey = "Backstab",
        ParentNodeKeys = new string[] { "Shuriken" },
        ChildrenNodeKeys = new string[] { "NinjaMastery" },
    };
    
    public static readonly SkillTreeNodeConfig NinjaMasteryConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Ninja Mastery",
        DescriptionTextKey = "Become a master ninja! \n Backstab Kunai: Throw it further; \n Shuriken: Bounce towards a second target.",
        RangeDescriptionKey = "Passive",
        IconPath = "AbilityIcon_Merged/stealth/ninja_mastery.png",
        AbilityPreviewPath = "Ability_Preview/stealth/ninja_mastery.gif",
        UnlockLevel = 28,
        MaximumLevel = 1,
        GridX = 1,
        GridY = 3,
        NType = NodeType.SkillEnhance,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 8100,
        UIPosition = new Vector2(230, 630),
        SkillKey = "NinjaMastery",
        ParentNodeKeys = new string[] { "Backstab" },
        ChildrenNodeKeys = new string[] { },
    };

    public static readonly SkillTreeNodeConfig TotalDarknessConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Total Darkness",
        DescriptionTextKey = $"Release a wave of darkness that blinds all nearby players for {EffectConfig.TotalDarknessConfig.BlindTime} seconds.",
        UpgradeTextKey = "*: Damage +1 \n **: Damage +1\n ***: Cooldown -1\n ****: 25% Life Steal",
        RangeDescriptionKey = $"{EffectConfig.TotalDarknessConfig.Range}m",
        CooldownKey = "%OVERRIDE%",
        BaseDamageKey = _overrideValue_,
        IconPath = "AbilityIcon_Merged/stealth/total_darkness.png",
        AbilityIconPath = "AbilityIcon_Separate/stealth/total_darkness_icon.png",
        AbilityPreviewPath = "Ability_Preview/stealth/total_darkness.gif",
        UnlockLevel = 23,
        MaximumLevel = 5,
        UpgradeGemCost = _tierTwoGemCost,
        GridX = 2,
        GridY = 3,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 5850,
        UIPosition = new Vector2(10, 630),
        SkillKey = "TotalDarkness",
        ParentNodeKeys = new string[] { "ShadowStep" },
        ChildrenNodeKeys = new string[] { },
    };

    #endregion

    #region NodeConfig: Element

    /// <summary>
    /// Punch node
    /// </summary>
    public static readonly SkillTreeNodeConfig IceFistNodeConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Ice Fist",
        DescriptionTextKey = "Punch with an ice covered fist.",
        UpgradeTextKey = "*: Damage +1 \n **: Damage +1\n ***: Damage +1\n ****: 10% Slowdown On Target (1.5s)",
        BaseDamageKey = _overrideValue_,
        RangeDescriptionKey = "Melee",
        CooldownKey = $"{EffectConfig.IceFistConfig.Cooldown}s",
        IconPath = "AbilityIcon_Merged/elemental/ice_fist.png",
        AbilityIconPath = "AbilityIcon_Separate/elemental/ice_fist_icon.png",
        AbilityPreviewPath = "Ability_Preview/elemental/ice_fist.gif",
        UnlockLevel = 0,
        MaximumLevel = 5,
        UpgradeGemCost = _tierOneGemCost,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Elemental,
        UpgradeCost = 100,
        UIPosition = new Vector2(450, 30), // This will be set to the item's offset value
        SkillKey = "IceFist",
        ParentNodeKeys = new string[] { },
        ChildrenNodeKeys = new string[] { "WindPunch", "Thunderbolt", "Fireball" },
    };
    
    public static readonly SkillTreeNodeConfig WindPunchNodeConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Wind Punch",
        DescriptionTextKey = "Channel power of the wind to boost your speed and and unleash a powerful punch to knockdown enemies. ",
        UpgradeTextKey = "*: Damage +1 \n **: Damage +1\n ***: Cooldown -1\n ****: 4x Boost",
        BaseDamageKey = _overrideValue_,
        RangeDescriptionKey = "Melee",
        CooldownKey = "%OVERRIDE%",
        IconPath = "AbilityIcon_Merged/elemental/wind_punch.png",
        AbilityIconPath = "AbilityIcon_Separate/elemental/wind_punch_icon.png",
        AbilityPreviewPath = "Ability_Preview/elemental/wind_punch.gif",
        UnlockLevel = 27,
        MaximumLevel = 5,
        UpgradeGemCost = _tierOneGemCost,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Elemental,
        UpgradeCost = 4400,
        SkillKey = "WindPunch",
        GridX = 0,
        GridY = 1,
        ParentNodeKeys = new string[] { "IceFist" },
        ChildrenNodeKeys = new string[] { "IceStorm"},
    };
    
    public static readonly SkillTreeNodeConfig FireballNodeConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Fireball",
        DescriptionTextKey = "Toss a fireball that explodes and burns your enemy. ",
        UpgradeTextKey = "*: Damage +1 \n **: Coo1down -1\n ***: Burn +1s\n ****: Fireball split to 3",
        BaseDamageKey = _overrideValue_,
        RangeDescriptionKey = "12m",
        CooldownKey = "%OVERRIDE%",
        IconPath = "AbilityIcon_Merged/elemental/fireball.png",
        AbilityIconPath = "AbilityIcon_Separate/elemental/fireball_icon.png",
        AbilityPreviewPath = "Ability_Preview/elemental/fireball.gif",
        UnlockLevel = 30,
        MaximumLevel = 5,
        UpgradeGemCost = _tierTwoGemCost,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Elemental,
        UpgradeCost = 5100,
        SkillKey = "Fireball",
        GridX = 1,
        GridY = 1,
        ParentNodeKeys = new string[] { "IceFist" },
        ChildrenNodeKeys = new string[] { },
    };
    
    public static readonly SkillTreeNodeConfig ThunderboltNodeConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Thunderbolt",
        DescriptionTextKey = "Summon a thunderbolt that paralyzes your enemies. ",
        UpgradeTextKey = "*: Damage +1 \n **: Damage +1\n ***: Range +2m\n ****: Gain immunity during cast",
        BaseDamageKey = _overrideValue_,
        RangeDescriptionKey = "%OVERRIDE%",
        CooldownKey = $"{EffectConfig.ThunderboltConfig.Cooldown}s",
        IconPath = "AbilityIcon_Merged/elemental/lightning_bolt.png",
        AbilityIconPath = "AbilityIcon_Separate/elemental/lightning_bolt_icon.png",
        AbilityPreviewPath = "Ability_Preview/elemental/thunderbolt.gif",
        UnlockLevel = 26,
        MaximumLevel = 5,
        UpgradeGemCost = _tierOneGemCost,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Elemental,
        UpgradeCost = 4050,
        SkillKey = "Thunderbolt",
        GridX = 2,
        GridY = 1,
        ParentNodeKeys = new string[] { "IceFist" },
        ChildrenNodeKeys = new string[] { },
    };
    
    public static readonly SkillTreeNodeConfig IceStormNodeConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Ice Storm",
        DescriptionTextKey = "Surround yourself with an ice storm. Slow enemies caught inside and shoot out ice chunks.",
        UpgradeTextKey = "*: Damage +1 \n **: Chunk Damage +1\n ***: Cooldown -2s\n ****: +1 Ice Chunk",
        BaseDamageKey = _overrideValue_,
        RangeDescriptionKey = "3m",
        CooldownKey = "%OVERRIDE%",
        IconPath = "AbilityIcon_Merged/elemental/ice_storm.png",
        AbilityIconPath = "AbilityIcon_Separate/elemental/ice_storm_icon.png",
        AbilityPreviewPath = "Ability_Preview/elemental/icestorm.gif",
        UnlockLevel = 34,
        MaximumLevel = 5,
        UpgradeGemCost = _tierTwoGemCost,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Elemental,
        UpgradeCost = 10500,
        SkillKey = "IceStorm",
        GridX = 0,
        GridY = 2,
        ParentNodeKeys = new string[] { "WindPunch" },
        ChildrenNodeKeys = new string[] { },
    };
    
    public static readonly SkillTreeNodeConfig ChargingStationNodeConfig = new SkillTreeNodeConfig()
    {
        DisplayName = "Charging Station",
        DescriptionTextKey = "Place a charging station that charges nearby players for an overshield. You can also detonate it.",
        UpgradeTextKey = "*: Damage +2 \n **: Shield +5\n ***: Cooldown -2s\n ****: Detonation shocks enemies",
        BaseDamageKey = _overrideValue_,
        RangeDescriptionKey = "6m",
        CooldownKey = "%OVERRIDE%",
        IconPath = "AbilityIcon_Merged/elemental/lightning_snare.png",
        AbilityIconPath = "AbilityIcon_Separate/elemental/lightning_snare_icon.png",
        AbilityPreviewPath = "Ability_Preview/elemental/lightning_snare.gif",
        UnlockLevel = 32,
        MaximumLevel = 5,
        UpgradeGemCost = _tierTwoGemCost,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Elemental,
        UpgradeCost = 8800,
        SkillKey = "ChargingStation",
        GridX = 2,
        GridY = 2,
        ParentNodeKeys = new string[] { "Thunderbolt" },
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
            { "Punch", PunchNodeConfig },
            { "HealthBoost", HealthBoostNodeConfig },
            { "AttackBoost", AttackBoostNodeConfig },
            { "Punch2", PunchTwoConfig },
            { "HealthBoost2", HealthBoost2NodeConfig },
            { "AttackBoost2", AttackBoost2NodeConfig },
            { "Punch3", PunchThreeConfig },
            // Defensive
            { "RollOut", RollOutNodeConfig },
            { "Shield", ShieldConfig },
            { "HealthBoostD", HealthBoostDNodeConfig},
            { "IronSkin", IronSkinConfig},
            {"SpikeShield", SpikeShieldConfig},
            {"GravityCrush", GravityCrushConfig},
            {"Parry", ParryConfig},
            // Brawler
            { "ShoulderCrash", ShoulderCrashNodeConfig },
            { "GroundStomp", GroundStompConfig },
            { "Rage", RageConfig },
            { "DoublePunch", DoublePunchConfig },
            { "SelfDestruct", SelfDestructConfig },
            { "BattleCry", BattleCryConfig },
            { "ClawSlash", ClawSlashConfig },
            { "LeapSlam", LeapSlamConfig },
            { "DualClaw", DualClawConfig},
            // Psionic
            { "SpoonThrow", SpoonThrowConfig },
            { "Befuddle", BefuddleConfig },
            { "Psybolt", PsyboltConfig },
            { "SelfHeal", SelfHealConfig },
            { "Regeneration", RegenerationConfig },
            { "Hypnotize", HypnotizeConfig },
            { "PsionicBeam", PsionicBeamConfig },
            { "PsyThrow", PsyThrowConfig },
            { "Concentrate", ConcentrateConfig},
            { "Psychic", PsychicConfig},
            // Stealth
            { "Invisibility", InvisibilityConfig },
            { "LightFeet", LightFeetConfig },
            {"SpeedBoost", SpeedBoostConfig},
            { "Shuriken", ShurikenConfig },
            { "BearTrap", BearTrapConfig },
            { "ShadowStep", ShadowStepConfig },
            { "Backstab", BackstabConfig },
            { "TotalDarkness", TotalDarknessConfig },
            { "NinjaMastery", NinjaMasteryConfig},
            // Elemental
            { "IceFist", IceFistNodeConfig},
            {"WindPunch", WindPunchNodeConfig},
            {"Fireball", FireballNodeConfig},
            {"Thunderbolt", ThunderboltNodeConfig},
            {"IceStorm", IceStormNodeConfig},
            {"ChargingStation", ChargingStationNodeConfig}
        };

    // [Add Skill] Item 3: Put Classification Here
    public static readonly HashSet<string> AttrBoostSkills = new HashSet<string>()
        { "HealthBoost", "AttackBoost", "HealthBoost2", "AttackBoost2", "HealthBoostD" };

    public static readonly HashSet<string> ActiveSkills = new HashSet<string>()
    {
        // Defensive
        "Punch", "RollOut", "Shield","IronSkin","SpikeShield","GravityCrush", "Parry",
        // Brawler
        "ShoulderCrash", "GroundStomp", "Rage", "DoublePunch", "SelfDestruct", "BattleCry", "ClawSlash", "LeapSlam", 
        // Psionic
        "SpoonThrow", "Befuddle", "Psybolt", "SelfHeal", "Regeneration", "Hypnotize", "PsionicBeam", "PsyThrow",
        // Stealth
        "Invisibility", "LightFeet", "SpeedBoost","Shuriken", "BearTrap", "ShadowStep", "Backstab", "TotalDarkness",
        // Elemental
        "IceFist", "WindPunch", "Fireball", "Thunderbolt", "IceStorm", "ChargingStation"
    };

    public static readonly HashSet<string> ReplacementSkills = new HashSet<string>() { "Punch2", "Punch3" };

    public static readonly HashSet<string> SkillEnhanceSkills = new HashSet<string>() { "DualClaw",
        "Concentrate", "Psychic", "NinjaMastery", };
}

// [Add Skill] Item 4: Add the association between skillKey and type of ability.
// Only active skills (skillkeys associated with an ability) need this. 

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
        {SC.BackstabConfig.SkillKey, typeof(AbilityBackstab)},
        {SC.TotalDarknessConfig.SkillKey, typeof(AbilityTotalDarkness)},
        {SC.IceFistNodeConfig.SkillKey, typeof(AbilityIceFist)},
        {SC.IronSkinConfig.SkillKey, typeof(AbilityIronSkin)},
        {SC.SpikeShieldConfig.SkillKey, typeof(AbilitySpikeShield)},
        {SC.GravityCrushConfig.SkillKey, typeof(AbilityGravityCrush)},
        {SC.ParryConfig.SkillKey, typeof(AbilityParry)},
        {SC.WindPunchNodeConfig.SkillKey, typeof(AbilityWindPunch)},
        {SC.FireballNodeConfig.SkillKey, typeof(AbilityFireball)},
        {SC.ThunderboltNodeConfig.SkillKey, typeof(AbilityThunderbolt)},
        {SC.IceStormNodeConfig.SkillKey, typeof(AbilityIceStorm)},
        {SC.ChargingStationNodeConfig.SkillKey, typeof(AbilityChargingStation)}
    };
}

// [Add Skill] Item 5: If you have override items, handle them in SkillConfig.cs
// by adding the entry in respective switch cases