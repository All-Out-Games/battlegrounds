using AO;

/// <summary>
/// Base Ability Config. Pass modified data to player based on player stats on the server
/// </summary>
public class AbilityConfig : System<AbilityConfig>
{
    #region RollOut

    public struct RollOutConfig
    {
        public static int BumpDmgBase = 9;
        public static int BumpDmgGrowth = 3;
        
        public float Duration = 10f;
        public int ContactDamage = 5;
        public float SpeedBuffMultiplier = 1.5f;
        public float BumpStrength = 114f;
        

        public RollOutConfig()
        {
        }
    }

    public static RollOutConfig GetPlayerRollOutConfig(int level)
    {
        RollOutConfig cfg = new RollOutConfig();
        
        // Modify the config on the server side in this function based on parameters
        cfg.ContactDamage = RollOutConfig.BumpDmgBase + level * RollOutConfig.BumpDmgGrowth * level;

        return cfg;
    }
    

    #endregion
}