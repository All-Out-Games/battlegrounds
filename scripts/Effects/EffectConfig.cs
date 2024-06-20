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
        public static float Cooldown = 8f;
        
        public float DashDuration = 0.5f;
        public float DashSpeed = 275f;
        
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
        public static readonly float BefuddleConfusionIntensity = 75f;
        
        // PsyBolt
        public static readonly int PsyboltDamageBase = 6;
        public static readonly float PsyboltCooldown = 4.5f;
        public static readonly float PsyboltRange = 10f;
        public static readonly float PsyboltLifeTime = 1f;
        public static readonly float PsyboltKnockbackStrength = 165f;

        public float Speed = 15f;
        public float ProjectileLifetime = 2f;
        public float ThrowAnimationLength = 0.3f; // You can use this as delay (or use animation event) to spawn the projectile
        public int Damage = 0;

        public string ProjectilePrefabKey;

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
                ProjectilePrefabKey = "BroccoliProjectile.prefab",
                ProjectileLifetime = SpoonLifetime
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
                Speed = PsyboltRange / PsyboltLifeTime
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
        public static readonly float StompActivationTime = 0.25f;  // Delay time before activating the collider
        public static readonly float Cooldown = 5f;

        public int StompDamage = 0;
        public float StompRadius = 4;
        
        public GroundStompConfig()
        {
        
        }

        public static GroundStompConfig GetDefault(int atk)
        {
            GroundStompConfig cfg = new GroundStompConfig()
            {
                StompDamage = atk + GroundStompConfig.StompDamageBase
            };
            return cfg;
        }
    }



    #endregion

    #region cfg: Rage

    public struct RageConfig
    {
        public static int AtkBoostBase = 6;
        public static float Cooldown = 7f;
        public float Duration = 5f;
        public int AtkBoost;

        public RageConfig()
        {
            
        }

        public static RageConfig GetDefault()
        {
            return new RageConfig() with {AtkBoost = AtkBoostBase};
        }
    }
    

    #endregion

    #region Cfg: DoublePunch

    public struct DoublePunchConfig
    {
        public static int BaseDmg = 2;
        public static float Cooldown = 4f;
        public static float PunchAnimationTime = 0.4f; // A bit quicker than normal punch
        public static float PunchActivationTime = 0.2f;
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
        public static float ActivationTime = 1f;
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
        public static readonly float RoarActivationTime = 0.5f;
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
        public static readonly float SlashAnimationTime = 0.6f;
        public static readonly float SlashActivationTime = 0.25f;
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
        public static int SlamDamageBase = 10;
        public static float Cooldown = 12f;
        
        public float DashDuration = 0.4f;
        public float SlamDuration = 0.1f;

        public int SlamDamage = 5;
        public float BumpStrength = 280f;
        
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
        public static readonly string FxPath = "RegenerationAura.prefab";
    }

    #endregion

    #region cfg: Hypnotize

    public struct HypnotizeConfig
    {
        public static readonly float HypnotizeTime = 2.5f;
        public static readonly float HypnotizeRange = 2f;
        public static readonly float Cooldown = 8f;
    }

    #endregion
}