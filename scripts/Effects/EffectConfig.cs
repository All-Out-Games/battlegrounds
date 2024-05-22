using AO;

/// <summary>
/// Base Ability Config. Pass modified data to player based on player stats on the server
/// </summary>
public static class EffectConfig
{
    
    #region RollOut

    public struct RollOutConfig
    {
        public static int BumpDmgBase = 9;
        public static int BumpDmgGrowth = 3;
        
        public float Duration = 10f;
        public int ContactDamage = 5;
        public float SpeedBuffMultiplier = 1.5f;
        public float BumpStrength = 35f;
        public float Cooldown = 5f;
        

        public RollOutConfig()
        {
        }
    }

    public static RollOutConfig GetPlayerRollOutConfig(int level)
    {
        RollOutConfig cfg = new RollOutConfig
        {
            // Modify the config on the server side in this function based on parameters
            ContactDamage = RollOutConfig.BumpDmgBase + level * RollOutConfig.BumpDmgGrowth
        };

        return cfg;
    }
    

    #endregion

    #region Punch

    public struct PunchConfig
    {
        public static int PunchDmgBase = 5;
        public static int PunchDmgGrowth = 2;
        public static float PunchAnimationTime = 0.6f; // Entire duration of the punch animation
        public static float PunchActivationTime = 0.25f;  // Delay time before activating the collider
        public static float PunchRange = 2;
        
        
        public int PunchDamage = 5; // TODO: The final damage should be determined by the player's stats
        
        
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
}