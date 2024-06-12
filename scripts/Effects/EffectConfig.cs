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
        public static readonly float Cooldown = 8f;
        
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
        public static readonly int SpoonDamageBase = 5;
        public static readonly int SpoonThrowCooldown = 3;
        public static readonly float SpoonRange = 10f;
        public static float SpoonLifetime = 2f;

        public float Speed = 15f;
        public float ProjectileLifetime = 2f;
        public float ThrowAnimationLength = 0.3f; // You can use this as delay (or use animation event) to spawn the projectile
        public int Damage = 0;

        public string ProjectilePrefabKey;

        public ProjectileConfig()
        {
            
        }
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
            Damage = ProjectileConfig.SpoonDamageBase + attack,
            ProjectilePrefabKey = "BroccoliProjectile.prefab",
            ProjectileLifetime = ProjectileConfig.SpoonLifetime
        };
        return cfg;
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
        public static int BaseDmg = 10;
        public static int BaseSelfDmg = 12;
        public static float Cooldown = 10f;
        public static float ActivationTime = 0.5f;
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
}