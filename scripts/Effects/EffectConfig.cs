using AO;

/// <summary>
/// Base Ability Config. Pass modified data to player based on player stats on the server
/// This data will be passed through network, therefore IT SHOULD NOT CONTAIN ANY CLASS REFERENCES! (i.e. data only)
/// </summary>
public static class EffectConfig
{
    
    #region Cfg: RollOut

    public struct RollOutConfig
    {
        public static int BumpDmgBase = 9;
        public static float Cooldown = 7f;
        
        public float Duration = 5f;
        public int ContactDamage = 5;
        public float SpeedBuffMultiplier = 1.35f;
        public float BumpStrength = 35f;
        
        

        public RollOutConfig()
        {
        }
        
        public static RollOutConfig GetDefault(int attack)
        {
            RollOutConfig cfg = new RollOutConfig
            {
                // Modify the config on the server side in this function based on parameters
                ContactDamage = RollOutConfig.BumpDmgBase + attack
            };

            return cfg;
        }
    }


    

    #endregion

    #region Cfg: Punch

    public struct PunchConfig
    {
        public static int PunchDmgBase = 1;
        public static int PunchDmgGrowth = 2;
        public static float PunchAnimationTime = 0.6f; // Entire duration of the punch animation
        public static float PunchActivationTime = 0.25f;  // Delay time before activating the collider
        public static float PunchRange = 2;
        
        
        public int PunchDamage = 5;
        
        
        public PunchConfig()
        {
        
        }
    }

    public static PunchConfig GetPlayerPunchConfig(int level, int atk = 0)
    {
        PunchConfig cfg = new PunchConfig
        {
            PunchDamage = PunchConfig.PunchDmgGrowth * level + PunchConfig.PunchDmgBase + atk
        };
        return cfg;
    }

    #endregion

    #region Cfg: ShoulderCrash

    public struct ShoulderCrashConfig
    {
        public static int BumpDmgBase = 9;
        public static float Cooldown = 4f;
        
        public float DashDuration = 1f;
        public float DashSpeed = 200f;
        
        public int ContactDamage = 5;
        public float BumpStrength = 140f;
        
        public ShoulderCrashConfig()
        {
            
        }


        public static ShoulderCrashConfig GetDefault(int attack)
        {
            ShoulderCrashConfig cfg = new ShoulderCrashConfig
            {
                ContactDamage = attack + ShoulderCrashConfig.BumpDmgBase
            };
            return cfg;
        }
    }

    
    #endregion

    #region Cfg: Shield

    public struct ShieldConfig
    {
        public static readonly int ShieldAmtBase = 20;
        public static readonly float Cooldown = 6f;
        
        public float Duration = 8f;
        public int ShieldAmt = ShieldAmtBase;

        public ShieldConfig()
        {
            
        }
        
        public static ShieldConfig GetDefault()
        {
            return new ShieldConfig();
        }
    }

    #endregion

    #region Cfg: Ranged Projectile

    /// <summary>
    /// Projectile logic is self-contained, therefore this effect for throwing stuff can be reused.
    /// </summary>
    public struct ProjectileConfig
    {
        // Spoon
        public static readonly int SpoonDamageBase = 3;
        public static readonly float SpoonThrowCooldown = 3f;
        public static readonly float SpoonRange = 10f;
        public static readonly float SpoonLifetime = 0.6f;

        // Befuddle
        public static readonly int BefuddleDamageBase = 1;
        public static readonly float BefuddleCooldown = 7;
        public static readonly float BefuddleRange = 10f;
        public static readonly float BefuddleLifetime = 1f;
        public static readonly float BefuddleConfusionTime = 2.75f;
        public static readonly float BefuddleConfusionIntensity = 75f; // Higher will make player walk faster in confused state
        
        // PsyBolt
        public static readonly int PsyboltDamageBase = 6;
        public static readonly float PsyboltCooldown = 4.5f;
        public static readonly float PsyboltRange = 10f;
        public static readonly float PsyboltLifeTime = 1f;
        public static readonly float PsyboltKnockbackStrength = 165f;
        
        // Shuriken
        public static readonly int ShurikenDamageBase = 5;
        public static readonly float ShurikenBackDamageModifier = 1.5f;
        public static readonly float ShurikenCooldown = 4.5f;
        public static readonly float ShurikenRange = 6f;
        public static readonly float ShurikenLifetime = 0.5f;

        public float Speed = 15f;
        public float ProjectileLifetime = 2f;
        public float ThrowAnimationLength = 0.3f; // You can use this as delay (or use animation event) to spawn the projectile
        public int Damage = 0;
        public string ProjectilePrefabKey;
        public string ThrowTrigger = "throw";

        public ProjectileConfig()
        {
            
        }
        
        /// <summary>
        /// Projectile config can be reused, you can make it timed / ranged in your effect codes
        /// Here the default config is for the spoon throw skill
        /// </summary>
        /// <param name="attack"></param>
        /// <returns></returns>
        public static ProjectileConfig GetPlayerSpoonThrowConfig(int attack)
        {
            ProjectileConfig cfg = new ProjectileConfig()
            {
                Damage = SpoonDamageBase + attack,
                ProjectilePrefabKey = "SpoonProjectile.prefab",
                ProjectileLifetime = SpoonLifetime,
            };
            return cfg;
        }

        public static ProjectileConfig GetPlayerBefuddleConfig(int attack)
        {
            ProjectileConfig cfg = new ProjectileConfig()
            {
                Damage = BefuddleDamageBase + attack,
                ProjectilePrefabKey = "BefuddleProjectile.prefab",
                ProjectileLifetime = BefuddleLifetime,
                Speed = BefuddleRange / BefuddleLifetime
            };
            return cfg;
        }

        public static ProjectileConfig GetPlayerPsyboltConfig(int attack)
        {
            ProjectileConfig cfg = new ProjectileConfig()
            {
                Damage = PsyboltDamageBase + attack,
                ProjectilePrefabKey = "PsyboltProjectile.prefab",
                ProjectileLifetime = PsyboltLifeTime,
                Speed = PsyboltRange / PsyboltLifeTime,
                ThrowTrigger = "psybolt"
            };
            return cfg;
        }

        public static ProjectileConfig GetPlayerShurikenConfig(int attack)
        {
            ProjectileConfig cfg = new ProjectileConfig()
            {
                Damage = ShurikenDamageBase + attack,
                ProjectilePrefabKey = "ShurikenProjectile.prefab",
                ProjectileLifetime = ShurikenLifetime,
                Speed = ShurikenRange / ShurikenLifetime
            };
            return cfg;
        }
    }



    #endregion

    #region Cfg: GroundStomp

    public struct GroundStompConfig
    {
        public static readonly int StompDamageBase = 1;
        public static readonly float StompAnimationTime = 0.6f; // Entire duration of the punch animation
        public static readonly float Cooldown = 4f;

        public int StompDamage = 0;
        public float StompRadius = 4;
        
        public GroundStompConfig()
        {
        
        }

        public static GroundStompConfig GetDefault(int atk)
        {
            GroundStompConfig cfg = new GroundStompConfig()
            {
                StompDamage = atk + StompDamageBase
            };
            return cfg;
        }
    }



    #endregion

    #region cfg: Rage

    public struct RageConfig
    {
        public static int AtkBoostBase = 6;
        public static float Cooldown = 4f;
        public static float Duration = 5f;
        public int AtkBoost;

        public RageConfig()
        {
            
        }

        public static RageConfig GetDefault()
        {
            return new RageConfig { AtkBoost = AtkBoostBase };
        }
    }
    

    #endregion

    #region Cfg: DoublePunch

    public struct DoublePunchConfig
    {
        public static int BaseDmg = 2;
        public static float Cooldown = 3f;
        public static float PunchAnimationTime = 0.4f; // A bit quicker than normal punch
        public static float PunchRange = 2;

        public int PunchDamage = BaseDmg;
        public float BumpStrength = 70;

        public DoublePunchConfig()
        {
            
        }

        public static DoublePunchConfig GetDefault(int attack)
        {
            return new DoublePunchConfig() with { PunchDamage = BaseDmg + attack};
        }
    }

    #endregion

    #region Cfg: Self Destruct

    public struct SelfDestructConfig
    {
        public static int BaseDmg = 12;
        public static int BaseSelfDmg = 10;
        public static float Cooldown = 11f;
        public static float BlastRange = 8f;
        public static float BumpStrength = 140f;

        public int BlastDamage;
        public int SelfDamage;

        public static SelfDestructConfig GetDefault(int attack)
        {
            return new SelfDestructConfig()
            {
                BlastDamage = BaseDmg + attack,
                SelfDamage = BaseSelfDmg
            };
        }
    }

    #endregion

    #region cfg: BattleCry

    public struct BattleCryConfig
    {
        public static readonly float Cooldown = 7f;
        public static readonly float RoarAnimationTime = 0.9f;
        public static readonly int RoarDmgBase = -2;
        
        public float StunTime = 0.8f;
        public float RoarRadius = 3;
        public int RoarDamage = 0;

        public BattleCryConfig()
        {
            
        }

        public static BattleCryConfig GetDefault(int atk)
        {
            var cfg = new BattleCryConfig() {RoarDamage = atk + RoarDmgBase};
            return cfg;
        }
    }

    #endregion

    #region cfg: ClawSlash

    public struct ClawSlashConfig
    {
        public static readonly float Cooldown = 5f;
        public static readonly float SlashAnimationTime = 0.25f;
        public static readonly float SlashActivationTime = 0.05f;
        public static readonly int SlashDmgBase = 1;
        public static readonly float SlashRadius = 2;

        public float BleedTime = 5f;
        public int BleedDmg = 1;
        public int SlashDamage = 1;

        public ClawSlashConfig()
        {
            
        }

        public static ClawSlashConfig GetDefault(int atk)
        {
            var cfg = new ClawSlashConfig() { SlashDamage = SlashDmgBase + atk };
            return cfg;
        }
    }

    #endregion

    #region cfg: LeapSlam

    public struct LeapSlamConfig
    {
        public static int SlamDamageBase = 5;
        public static float KnockDownTime = 1f;
        public static float Cooldown = 7f;
        

        public int SlamDamage = 5;
        public float SlamRadius = 3f;
        public float BumpStrength = 150f;
        
        public LeapSlamConfig()
        {
            
        }


        public static LeapSlamConfig GetDefault(int attack)
        {
            LeapSlamConfig cfg = new LeapSlamConfig
            {
                SlamDamage = SlamDamageBase + attack
            };
            return cfg;
        }
    }

    #endregion

    #region cfg: SelfHeal

    public struct SelfHealConfig
    {
        public static readonly int HealAmtBase = 35;
        public static readonly float ChannelTime = 2.4f;
        public static readonly float Cooldown = 7f;
        public static readonly string FxPath = "";
    }

    #endregion

    #region cfg: Regenerate

    public struct RegenerateConfig
    {
        public static readonly int PerSecondHeal = 3;
        public static readonly float HealTime = 6f;
        public static readonly float Cooldown = 6f;
    }

    #endregion

    #region cfg: Hypnotize

    public struct HypnotizeConfig
    {
        public static readonly float HypnotizeTime = 5f;
        public static readonly float HypnotizeRange = 2f;
        public static readonly float Cooldown = 8f;
    }

    #endregion

    #region cfg: PsionicBeam

    public struct PsionicBeamConfig
    {
        public static readonly float Cooldown = 6f;
        public static readonly float MinimumRange = 4f;
        public static readonly float MaximumRange = 8f;
        public static readonly float Degrees = 15f; // Half the entire angle
        public static readonly int PsionicBeamDmgBase = 3;
        public static readonly float CarveTime = 0.4f;
        
        
        public static readonly float BeamCarveInterval = 0.3f; // Length of each interval. Calculate total intervals at runtime
        public static readonly float CarveFadeTime = 0.6f;
        public static readonly float CarvePersistTime = 3f;
        public static readonly float EyeOffsetX = -1.5f;
        public static readonly float EyeOffsetY = 0.7f;

        public static readonly string FissueEndPrefabPath = "PsionicBeamGroundFissueEnd.prefab";
        public static readonly string FissuePrefabPath = "PsionicBeamGroundFissue.prefab";
        
        
        public int Damage;

        public static PsionicBeamConfig GetDefault(int atk)
        {
            return new PsionicBeamConfig() { Damage = atk + PsionicBeamDmgBase };
        }
    }

    #endregion

    #region cfg: PsyThrow

    public struct PsyThrowConfig
    {
        public static readonly float Cooldown = 7f;
        public static readonly float Range = 8f;
        public static readonly float ThrowRange = 12f;
        public static readonly float ThrowStrength = 180f;
        public static readonly float GrabTime = 3f;
        public static readonly int DmgBase = 3;
        public static readonly float SelfDmgModifier = 0.5f; // If you throw the enemy to yourself, you take half the damage.

        public int Damage;
        public int SelfDamage;

        public static PsyThrowConfig GetDefault(int atk)
        {
            return new PsyThrowConfig() { Damage = atk + DmgBase, SelfDamage = (int)((atk + DmgBase) * SelfDmgModifier) };
        }
    }

    #endregion

    #region cfg: Invisibility

    public struct InvisibilityConfig
    {
        public static readonly float Cooldown = 6f;
        public static readonly float InvisTime = 4f;
    }

    #endregion

    #region cfg: LightFeet

    public struct LightFeetConfig
    {
        public static float SpeedModifier = 1.3f;
        public static float Cooldown = 5f;
        public static float BoostTime = 5f;

    }

    #endregion

    #region cfg: BearTrap

    public struct BearTrapConfig
    {
        public static int TrapBaseDamage = 2;
        public static float TrapLifeTime = 10f;
        public static float Cooldown = 7f;

        public static string TrapPrefabPath = "BearTrap.prefab";

        public int Damage;

        public static BearTrapConfig GetDefault(int atk)
        {
            BearTrapConfig cfg = new BearTrapConfig { Damage = TrapBaseDamage + atk };
            return cfg;
        }
    }

    #endregion

    #region cfg: ShadowStep

    public struct ShadowStepConfig
    {
        public static float Cooldown = 2f;
        public static float MovementDistance = 4f;
    }

    #endregion

    #region cfg: Backstab

    public struct BackStabConfig
    {
        public static float Cooldown = 9f;
        public static readonly float KunaiRange = 6f;
        public static readonly float KunaiLifetime = 0.5f;
        public static ProjectileConfig GetKunaiConfig()
        {
            return new ProjectileConfig()
            {
                Damage = 0,
                ProjectilePrefabKey = "KunaiProjectile.prefab",
                ProjectileLifetime = KunaiLifetime,
                Speed = KunaiRange / KunaiLifetime
            };
        }

        public static int BaseDmg = 8;
        public static float DamageDelay = 0.3f;
        public static float BackstabTime = 1.0f;

        public int Damage;

        public static BackStabConfig GetDefault(int atk)
        {
            return new BackStabConfig() { Damage = atk + BaseDmg };
        }
    }

    #endregion

    #region cfg: TotalDarkness

    public struct TotalDarknessConfig
    {
        public static float Cooldown = 6f;
        public static float Range = 6f;
        public static float BlindTime = 4f;
    }

    #endregion
}