// Fill in the actual configs in this file.
using AO;
using SC = SkillConfig;
using Assembly.scripts.Effects.ActiveSkills;

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
        DescriptionTextKey = "Punch and deals damage.",
        BaseDamageKey = EffectConfig.PunchConfig.PunchDmgBase,
        RangeDescriptionKey = "Melee",
        IconPath = "AbilityIcon_Merged/basic/punch.png",
        MaximumLevel = 1,
        NType = NodeType.SkillReplace,
        NTab = SkillTreeTabs.Basic,
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
        DescriptionTextKey = "Boost the player's health by 10.",
        MaximumLevel = 1,
        IconPath = "AbilityIcon_Merged/basic/health_boost.png",
        NeedRemover = true,
        NType = NodeType.AttrBoost,
        NTab = SkillTreeTabs.Basic,
        UpgradeCost = 250,
        UIPosition = new Vector2(730, 230),
        GridX = 0,
        GridY = 1,
        SkillKey = "HealthBoost",
        ParentNodeKeys = new string[] { "Punch" },
        ChildrenNodeKeys = new string[] { "HealthBoost2" },
        Buff = new StatBuff
        {
            BoostType = StatType.MaxHealth,
            BoostValue = 10
        }
    };

    /// <summary>
    /// AttackBoost node
    /// </summary>
    public static readonly SkillTreeNodeConfig AttackBoostNodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Boost the player's attack by 2.",
        IconPath = "AbilityIcon_Merged/basic/attack_boost.png",
        MaximumLevel = 1,
        NeedRemover = true,
        NType = NodeType.AttrBoost,
        NTab = SkillTreeTabs.Basic,
        UpgradeCost = 250,
        UIPosition = new Vector2(110, 230),
        GridX = 2,
        GridY = 1,
        SkillKey = "AttackBoost",
        ParentNodeKeys = new string[] { "Punch" },
        ChildrenNodeKeys = new string[] { "AttackBoost2" },
        Buff = new StatBuff
        {
            BoostType = StatType.AttackPower,
            BoostValue = 2
        }
    };

    /// <summary>
    /// Punch2 node
    /// </summary>
    public static readonly SkillTreeNodeConfig PunchTwoConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Upgrade your punch to be more powerful.",
        BaseDamageKey = EffectConfig.PunchConfig.PunchDmgBase + EffectConfig.PunchConfig.PunchDmgGrowth,
        RangeDescriptionKey = "Melee",
        IconPath = "AbilityIcon_Merged/basic/punch_2.png",
        MaximumLevel = 1,
        NeedRemover = true,
        NType = NodeType.SkillReplace,
        NTab = SkillTreeTabs.Basic,
        UpgradeCost = 500,
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
        DescriptionTextKey = "Boost the player's health by 15",
        IconPath = "AbilityIcon_Merged/basic/health_boost_2.png",
        MaximumLevel = 1,
        NeedRemover = true,
        NType = NodeType.AttrBoost,
        NTab = SkillTreeTabs.Basic,
        UpgradeCost = 750,
        UIPosition = new Vector2(730, 630),
        GridX = 0,
        GridY = 2,
        SkillKey = "HealthBoost2",
        ParentNodeKeys = new string[] { "HealthBoost" },
        ChildrenNodeKeys = new string[] { },
        Buff = new StatBuff
        {
            BoostType = StatType.MaxHealth,
            BoostValue = 15
        }
    };

    /// <summary>
    /// AttackBoost node
    /// </summary>
    public static readonly SkillTreeNodeConfig AttackBoost2NodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Boost the player's attack by 3.",
        IconPath = "AbilityIcon_Merged/basic/attack_boost_2.png",
        MaximumLevel = 1,
        NeedRemover = true,
        NType = NodeType.AttrBoost,
        NTab = SkillTreeTabs.Basic,
        UpgradeCost = 750,
        UIPosition = new Vector2(110, 630),
        GridX = 2,
        GridY = 2,
        SkillKey = "AttackBoost2",
        ParentNodeKeys = new string[] { "AttackBoost" },
        ChildrenNodeKeys = new string[] { },
        Buff = new StatBuff
        {
            BoostType = StatType.AttackPower,
            BoostValue = 3
        }
    };

    /// <summary>
    /// Punch3 node
    /// </summary>
    public static readonly SkillTreeNodeConfig PunchThreeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Upgrade your punch to be more powerful.",
        BaseDamageKey = EffectConfig.PunchConfig.PunchDmgBase + EffectConfig.PunchConfig.PunchDmgGrowth * 2,
        RangeDescriptionKey = "Melee",
        IconPath = "AbilityIcon_Merged/basic/punch_3.png",
        MaximumLevel = 1,
        NeedRemover = true,
        NType = NodeType.SkillReplace,
        NTab = SkillTreeTabs.Basic,
        UpgradeCost = 1500,
        UIPosition = new Vector2(450, 830),
        GridX = 1,
        GridY = 2,
        SkillKey = "Punch3",
        ParentNodeKeys = new string[] { "Punch2" },
        ChildrenNodeKeys = new string[] { },
    };

    /// <summary>
    /// RollOut node
    /// </summary>
    public static readonly SkillTreeNodeConfig RollOutNodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "You roll around with higher speed and crash into other players",
        BaseDamageKey = EffectConfig.RollOutConfig.BumpDmgBase,
        RangeDescriptionKey = "Melee",
        CooldownKey = $"{EffectConfig.RollOutConfig.Cooldown}s",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Defensive,
        UpgradeCost = 1000,
        UIPosition = new Vector2(750, 330),
        SkillKey = "RollOut",
        IconPath = "AbilityIcon_Merged/defense/rollout.png",
        ParentNodeKeys = new string[] { "Shield" },
        ChildrenNodeKeys = new string[] { },
    };

    /// <summary>
    /// Shield node
    /// </summary>
    public static readonly SkillTreeNodeConfig ShieldConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = $"Create a shield that absorbs {EffectConfig.ShieldConfig.ShieldAmtBase} damage.",
        CooldownKey = $"{EffectConfig.ShieldConfig.Cooldown}s",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Defensive,
        UpgradeCost = 100,
        UIPosition = new Vector2(450, 30),
        SkillKey = "Shield",
        IconPath = "AbilityIcon_Merged/defense/shield.png",
        ParentNodeKeys = new string[] { },
        ChildrenNodeKeys = new string[] { "RollOut" },
    };




    #endregion

    #region NodeConfig: Brawler

    /// <summary>
    /// ShoulderCrash node
    /// </summary>
    public static readonly SkillTreeNodeConfig ShoulderCrashNodeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Dash to a direction and deals damage and knockback.",
        BaseDamageKey = EffectConfig.ShoulderCrashConfig.BumpDmgBase,
        RangeDescriptionKey = "5m",
        CooldownKey = $"{ EffectConfig.ShoulderCrashConfig.Cooldown }s",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 100,
        UIPosition = new Vector2(450, 30),
        SkillKey = "ShoulderCrash",
        IconPath = "AbilityIcon_Merged/brawler/shoulder_crash.png",
        ParentNodeKeys = new string[] { },
        ChildrenNodeKeys = new string[] { "GroundStomp", "Rage", "DoublePunch" },
    };

    public static readonly SkillTreeNodeConfig GroundStompConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Stomp The ground and damage nearby enemies.",
        BaseDamageKey = EffectConfig.GroundStompConfig.StompDamageBase,
        RangeDescriptionKey = $"{EffectConfig.GroundStompConfig.StompRadius}m",
        CooldownKey = $"{EffectConfig.GroundStompConfig.Cooldown}s",
        IconPath = "AbilityIcon_Merged/brawler/ground_stomp.png",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 750,
        UIPosition = new Vector2(730, 230),
        SkillKey = "GroundStomp",
        ParentNodeKeys = new string[] { "ShoulderCrash" },
        ChildrenNodeKeys = new string[] { "LeapSlam" },
    };

    public static readonly SkillTreeNodeConfig RageConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = $"Temporarily increase your attack power by {EffectConfig.RageConfig.AtkBoostBase}. Lasts {EffectConfig.RageConfig.Duration}s.",
        IconPath = "AbilityIcon_Merged/brawler/rage.png",
        CooldownKey = $"{EffectConfig.RageConfig.Cooldown}s",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 500,
        UIPosition = new Vector2(450, 230),
        SkillKey = "Rage",
        ParentNodeKeys = new string[] { "ShoulderCrash" },
        ChildrenNodeKeys = new string[] { "BattleCry" },
    };

    public static readonly SkillTreeNodeConfig DoublePunchConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Punch to the front for two times in quick succession. 2x Damage.",
        BaseDamageKey = EffectConfig.DoublePunchConfig.BaseDmg,
        RangeDescriptionKey = "Melee",
        CooldownKey = $"{EffectConfig.DoublePunchConfig.Cooldown}s",
        IconPath = "AbilityIcon_Merged/brawler/double_punch.png",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 350,
        UIPosition = new Vector2(170, 230),
        SkillKey = "DoublePunch",
        ParentNodeKeys = new string[] { "ShoulderCrash" },
        ChildrenNodeKeys = new string[] { "SelfDestruct", "ClawSlash" },
    };

    public static readonly SkillTreeNodeConfig SelfDestructConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = $"Channel and release a big blast at the cost of dealing {EffectConfig.SelfDestructConfig.BaseSelfDmg} damage to yourself.",
        BaseDamageKey = EffectConfig.SelfDestructConfig.BaseDmg,
        RangeDescriptionKey = $"{EffectConfig.SelfDestructConfig.BlastRange}m",
        IconPath = "AbilityIcon_Merged/brawler/self_destruct.png",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 3000,
        UIPosition = new Vector2(240, 430),
        SkillKey = "SelfDestruct",
        ParentNodeKeys = new string[] { "DoublePunch" },
        ChildrenNodeKeys = new string[] { },
    };

    public static readonly SkillTreeNodeConfig BattleCryConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Release a sound wave which shock and stun the enemy around you briefly.",
        BaseDamageKey = EffectConfig.BattleCryConfig.RoarDmgBase,
        RangeDescriptionKey = $"{EffectConfig.BattleCryConfig.RoarRadius}m",
        CooldownKey = $"{EffectConfig.BattleCryConfig.Cooldown}s",
        IconPath = "AbilityIcon_Merged/brawler/battle_cry.png",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 1500,
        UIPosition = new Vector2(450, 430),
        SkillKey = "BattleCry",
        ParentNodeKeys = new string[] { "Rage" },
        ChildrenNodeKeys = new string[] { },
    };

    public static readonly SkillTreeNodeConfig ClawSlashConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = $"Slash your enemies and make them bleed, losing {EffectConfig.ClawSlashConfig.BleedDmgBase} Health/s for {EffectConfig.ClawSlashConfig.BleedTimeBase}.",
        BaseDamageKey = EffectConfig.ClawSlashConfig.SlashDmgBase,
        RangeDescriptionKey = "Melee",
        CooldownKey = $"{EffectConfig.ClawSlashConfig.Cooldown}s",
        IconPath = "AbilityIcon_Merged/brawler/claw_slash.png",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 1150,
        UIPosition = new Vector2(10, 430),
        SkillKey = "ClawSlash",
        ParentNodeKeys = new string[] { "DoublePunch" },
        ChildrenNodeKeys = new string[] { },
    };

    public static readonly SkillTreeNodeConfig LeapSlamConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Leap fiercely to a direction and slam the ground. Enemies you hit got knocked away.",
        BaseDamageKey = EffectConfig.LeapSlamConfig.SlamDamageBase,
        RangeDescriptionKey = "5m",
        CooldownKey = $"{EffectConfig.LeapSlamConfig.Cooldown}s",
        IconPath = "AbilityIcon_Merged/brawler/leaping_fist_slam.png",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Brawler,
        UpgradeCost = 2250,
        UIPosition = new Vector2(730, 430),
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
        DescriptionTextKey = "Throw a spoon towards aiming position.",
        BaseDamageKey = EffectConfig.ProjectileConfig.SpoonDamageBase,
        RangeDescriptionKey = $"{EffectConfig.ProjectileConfig.SpoonRange}m",
        CooldownKey = $"{EffectConfig.ProjectileConfig.SpoonThrowCooldown}s",
        IconPath = "AbilityIcon_Merged/psionic/spoon_throw.png",
        MaximumLevel = 1,
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
        DescriptionTextKey = $"Throw a cloud of psychic energy that confuses enemy for {EffectConfig.ProjectileConfig.BefuddleConfusionTime}s.",
        BaseDamageKey = EffectConfig.ProjectileConfig.BefuddleDamageBase,
        RangeDescriptionKey = $"{EffectConfig.ProjectileConfig.BefuddleRange}m",
        CooldownKey = $"{EffectConfig.ProjectileConfig.BefuddleCooldown}s",
        IconPath = "AbilityIcon_Merged/psionic/befuddle.png",
        MaximumLevel = 1,
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
        DescriptionTextKey = "Manifest a bolt of energy that knocks back enemy.",
        BaseDamageKey = EffectConfig.ProjectileConfig.PsyboltDamageBase,
        RangeDescriptionKey = $"{EffectConfig.ProjectileConfig.PsyboltRange}m",
        CooldownKey = $"{EffectConfig.ProjectileConfig.PsyboltCooldown}s",
        IconPath = "AbilityIcon_Merged/psionic/psybolt.png",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 900,
        UIPosition = new Vector2(170, 230),
        SkillKey = "Psybolt",
        ParentNodeKeys = new string[] { "SpoonThrow" },
        ChildrenNodeKeys = new string[] { "PsionicBeam", "PsyThrow" },
    };

    /// <summary>
    /// SelfHeal
    /// </summary>
    public static readonly SkillTreeNodeConfig SelfHealConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = $"Sit down and channel healing energy for {EffectConfig.SelfHealConfig.ChannelTime}s, restore {EffectConfig.SelfHealConfig.HealAmtBase} health upon successful channel",
        CooldownKey = $"{EffectConfig.SelfHealConfig.Cooldown}s",
        IconPath = "AbilityIcon_Merged/psionic/self_heal.png",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 450,
        UIPosition = new Vector2(730, 230),
        SkillKey = "SelfHeal",
        ParentNodeKeys = new string[] { "SpoonThrow" },
        ChildrenNodeKeys = new string[] { "Regeneration" },
    };

    /// <summary>
    /// Regenerate
    /// </summary>
    public static readonly SkillTreeNodeConfig RegenerationConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = $"Regenerate {EffectConfig.RegenerateConfig.PerSecondHeal} health over {EffectConfig.RegenerateConfig.HealTime}s",
        CooldownKey = $"{EffectConfig.RegenerateConfig.Cooldown}s",
        IconPath = "AbilityIcon_Merged/psionic/regeneration.png",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 1350,
        UIPosition = new Vector2(730, 430),
        SkillKey = "Regeneration",
        ParentNodeKeys = new string[] { "SelfHeal" },
        ChildrenNodeKeys = new string[] { },
    };


    public static readonly SkillTreeNodeConfig HypnotizeConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = $"Make your enemies sleep for {EffectConfig.HypnotizeConfig.HypnotizeTime}s. They wake up if get damaged.",
        RangeDescriptionKey = "3m",
        CooldownKey = $"{EffectConfig.HypnotizeConfig.Cooldown}s",
        IconPath = "AbilityIcon_Merged/psionic/hypnotize.png",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 1900,
        UIPosition = new Vector2(450, 430),
        SkillKey = "Hypnotize",
        ParentNodeKeys = new string[] { "Befuddle" },
        ChildrenNodeKeys = new string[] { },

    };

    public static readonly SkillTreeNodeConfig PsionicBeamConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Using a beam, sweep a small area in front of you with dark energy.",
        BaseDamageKey = EffectConfig.PsionicBeamConfig.PsionicBeamDmgBase,
        CooldownKey = $"{EffectConfig.PsionicBeamConfig.Cooldown}s",
        RangeDescriptionKey = "3-7m",
        IconPath = "AbilityIcon_Merged/psionic/psionic_beam.png",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 2700,
        UIPosition = new Vector2(10, 430),
        SkillKey = "PsionicBeam",
        ParentNodeKeys = new string[] { "Psybolt" },
        ChildrenNodeKeys = new string[] { },

    };

    public static readonly SkillTreeNodeConfig PsyThrowConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Grab your enemy with your psionic force. Activate again to throw them to a direction of your choice.",
        BaseDamageKey = EffectConfig.PsyThrowConfig.DmgBase,
        RangeDescriptionKey = $"{EffectConfig.PsyThrowConfig.Range}m",
        CooldownKey = $"{EffectConfig.PsyThrowConfig.Cooldown}s",
        IconPath = "AbilityIcon_Merged/psionic/psythrow.png",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Psionic,
        UpgradeCost = 3400,
        UIPosition = new Vector2(240, 430),
        SkillKey = "PsyThrow",
        ParentNodeKeys = new string[] { "Psybolt" },
        ChildrenNodeKeys = new string[] { },

    };

    #endregion

    #region NodeConfig: Stealth

    public static readonly SkillTreeNodeConfig InvisibilityConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = $"Become Invisible for {EffectConfig.InvisibilityConfig.InvisTime} seconds. Watch out for stray projectiles!",
        CooldownKey = $"{EffectConfig.InvisibilityConfig.Cooldown}s",
        IconPath = "AbilityIcon_Merged/stealth/invisibility.png",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 100,
        UIPosition = new Vector2(450, 30),
        SkillKey = "Invisibility",
        ParentNodeKeys = new string[] { },
        ChildrenNodeKeys = new string[] { "LightFeet" },

    };

    public static readonly SkillTreeNodeConfig LightFeetConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = $"Increase movement speed by {float.Round((EffectConfig.LightFeetConfig.SpeedModifier - 1f) * 100, 0)}% for {EffectConfig.LightFeetConfig.BoostTime}s.",
        IconPath = "AbilityIcon_Merged/stealth/light_feet.png",
        CooldownKey = $"{EffectConfig.LightFeetConfig.Cooldown}s",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 500,
        UIPosition = new Vector2(450, 230),
        SkillKey = "LightFeet",
        ParentNodeKeys = new string[] { "Invisibility" },
        ChildrenNodeKeys = new string[] { "Shuriken", "ShadowStep" },

    };

    public static readonly SkillTreeNodeConfig ShurikenConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Throw a shuriken which scores a critical hit if you struck the back of an enemy.",
        BaseDamageKey = EffectConfig.ProjectileConfig.ShurikenDamageBase,
        RangeDescriptionKey = $"{EffectConfig.ProjectileConfig.ShurikenRange}m",
        CooldownKey = $"{EffectConfig.ProjectileConfig.ShurikenCooldown}s",
        IconPath = "AbilityIcon_Merged/stealth/shuriken.png",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 850,
        UIPosition = new Vector2(450, 430),
        SkillKey = "Shuriken",
        ParentNodeKeys = new string[] { "LightFeet" },
        ChildrenNodeKeys = new string[] { "BearTrap", "Backstab" },
    };

    public static readonly SkillTreeNodeConfig BearTrapConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = $"Setup a hidden trap to snare your enemies. The trap needs a brief moment to arm and will last {EffectConfig.BearTrapConfig.TrapLifeTime}s.",
        BaseDamageKey = EffectConfig.BearTrapConfig.TrapBaseDamage,
        RangeDescriptionKey = $"{EffectConfig.BearTrapConfig.MaxSetupDistance}m",
        CooldownKey = $"{EffectConfig.BearTrapConfig.Cooldown}s",
        MaximumLevel = 1,
        IconPath = "AbilityIcon_Merged/stealth/bear_trap.png",
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 1650,
        UIPosition = new Vector2(450, 630),
        SkillKey = "BearTrap",
        ParentNodeKeys = new string[] { "Shuriken" },
        ChildrenNodeKeys = new string[] { },
    };

    public static readonly SkillTreeNodeConfig ShadowStepConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Dash to your movement direction.",
        RangeDescriptionKey = $"{EffectConfig.ShadowStepConfig.MovementDistance}m",
        CooldownKey = $"{EffectConfig.ShadowStepConfig.Cooldown}s",
        IconPath = "AbilityIcon_Merged/stealth/shadow_step.png",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 1500,
        UIPosition = new Vector2(230, 430),
        SkillKey = "ShadowStep",
        ParentNodeKeys = new string[] { "LightFeet" },
        ChildrenNodeKeys = new string[] { "TotalDarkness" },
    };

    public static readonly SkillTreeNodeConfig BackstabConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = "Throw A Kunai. If it hits, you can teleport and backstab the victim.",
        BaseDamageKey = EffectConfig.BackStabConfig.BaseDmg,
        RangeDescriptionKey = $"{EffectConfig.BackStabConfig.KunaiRange}m",
        CooldownKey = $"{EffectConfig.BackStabConfig.Cooldown}s",
        IconPath = "AbilityIcon_Merged/stealth/backstab.png",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 2350,
        UIPosition = new Vector2(230, 630),
        SkillKey = "Backstab",
        ParentNodeKeys = new string[] { "Shuriken" },
        ChildrenNodeKeys = new string[] { },
    };

    public static readonly SkillTreeNodeConfig TotalDarknessConfig = new SkillTreeNodeConfig()
    {
        DescriptionTextKey = $"Blind nearby opponents, making their vision extremely limited for {EffectConfig.TotalDarknessConfig.BlindTime}s.",
        CooldownKey = $"{EffectConfig.TotalDarknessConfig.Cooldown}s",
        IconPath = "AbilityIcon_Merged/stealth/total_darkness.png",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Stealth,
        UpgradeCost = 3500,
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
        DescriptionTextKey = "Use your elemental power to manifest a more powerful punch.",
        BaseDamageKey = EffectConfig.IceFistConfig.BaseDamage,
        RangeDescriptionKey = "Melee",
        CooldownKey = $"{EffectConfig.IceFistConfig.Cooldown}s",
        IconPath = "AbilityIcon_Merged/elemental/ice_fist.png",
        MaximumLevel = 1,
        NType = NodeType.SkillUnlock,
        NTab = SkillTreeTabs.Elemental,
        UpgradeCost = 100,
        UIPosition = new Vector2(450, 30), // This will be set to the item's offset value
        SkillKey = "IceFist",
        ParentNodeKeys = new string[] { },
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
            // Brawler
            { "ShoulderCrash", ShoulderCrashNodeConfig },
            { "GroundStomp", GroundStompConfig },
            { "Rage", RageConfig },
            { "DoublePunch", DoublePunchConfig },
            { "SelfDestruct", SelfDestructConfig },
            { "BattleCry", BattleCryConfig },
            { "ClawSlash", ClawSlashConfig },
            { "LeapSlam", LeapSlamConfig },
            // Psionic
            { "SpoonThrow", SpoonThrowConfig },
            { "Befuddle", BefuddleConfig },
            { "Psybolt", PsyboltConfig },
            { "SelfHeal", SelfHealConfig },
            { "Regeneration", RegenerationConfig },
            { "Hypnotize", HypnotizeConfig },
            { "PsionicBeam", PsionicBeamConfig },
            { "PsyThrow", PsyThrowConfig },
            // Stealth
            { "Invisibility", InvisibilityConfig },
            { "LightFeet", LightFeetConfig },
            { "Shuriken", ShurikenConfig },
            { "BearTrap", BearTrapConfig },
            { "ShadowStep", ShadowStepConfig },
            { "Backstab", BackstabConfig },
            { "TotalDarkness", TotalDarknessConfig },
            // Elemental
            {"IceFist", IceFistNodeConfig}
        };

    // [Add Skill] Item 3: Put Classification Here
    public static readonly HashSet<string> AttrBoostSkills = new HashSet<string>()
        { "HealthBoost", "AttackBoost", "HealthBoost2", "AttackBoost2" };

    public static readonly HashSet<string> ActiveSkills = new HashSet<string>()
    {
        "Punch", "RollOut", "Shield",
        // Brawler
        "ShoulderCrash", "GroundStomp", "Rage", "DoublePunch", "SelfDestruct", "BattleCry", "ClawSlash", "LeapSlam",
        // Psionic
        "SpoonThrow", "Befuddle", "Psybolt", "SelfHeal", "Regeneration", "Hypnotize", "PsionicBeam", "PsyThrow",
        // Stealth
        "Invisibility", "LightFeet", "Shuriken", "BearTrap", "ShadowStep", "Backstab", "TotalDarkness",
        // Elemental
        "IceFist"
    };

    public static readonly HashSet<string> ReplacementSkills = new HashSet<string>() { "Punch2", "Punch3" };

    public static readonly HashSet<string> SkillEnhanceSkills = new HashSet<string>() { };
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
        {SC.IceFistNodeConfig.SkillKey, typeof(AbilityIceFist)}
    };
}