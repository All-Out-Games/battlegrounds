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

        public float Duration = 5f;
        public int ContactDamage = 5;
        public float SpeedBuffMultiplier = 1.35f;
        public float BumpStrength = 35f;
        public float Cooldown = 10f;
        

        public RollOutConfig()
        {
        }
    }

    public static RollOutConfig GetPlayerRollOutConfig(int attack)
    {
        RollOutConfig cfg = new RollOutConfig
        {
            // Modify the config on the server side in this function based on parameters
            ContactDamage = RollOutConfig.BumpDmgBase + attack
        };

        return cfg;
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

        public float DashDuration = 0.5f;
        public float DashSpeed = 275f;
        
        public int ContactDamage = 5;
        public float BumpStrength = 140f;
        public float Cooldown = 8f;
        public ShoulderCrashConfig()
        {
            
        }


    }

    public static ShoulderCrashConfig GetPlayerShoulderCrashConfig(int attack)
    {
        ShoulderCrashConfig cfg = new ShoulderCrashConfig
        {
            ContactDamage = attack + ShoulderCrashConfig.BumpDmgBase
        };
        return cfg;
    }
    #endregion

    #region Cfg: Shield

    public struct ShieldConfig
    {
        public static int ShieldAmtBase = 20;
        
        public float Duration = 8f;
        public float Cooldown = 8f;
        public int ShieldAmt = ShieldAmtBase;

        public ShieldConfig()
        {
            
        }
    }

    public static ShieldConfig GetPlayerShieldConfig()
    {
        return new ShieldConfig();
    }

    #endregion

    #region Cfg: Ranged Projectile

    public struct ProjectileConfig
    {
        public static int ProjectileDamageBase = 5;
        
        public float Range = 30f;
        public float Speed = 15f;
        
        public float Cooldown = 8f;
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
            Damage = ProjectileConfig.ProjectileDamageBase + attack,
            ProjectilePrefabKey = "BroccoliProjectile.prefab"
        };
        return cfg;
    }

    #endregion
}