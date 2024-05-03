using AO;

/// <summary>
/// Base Ability Config. Pass modified data to player based on player stats on the server
/// </summary>
public class AbilityConfig : System<AbilityConfig>
{
    #region RollOut

    public struct RollOutConfig
    {
        public float Duration = 10f;
        public float ContactDamage = 5f;
        public float SpeedBuffMultiplier = 1.5f;
        public float BumpStrength = 10f;

        public RollOutConfig()
        {
        }
    }

    public static RollOutConfig GetPlayerRollOutConfig(int level)
    {
        RollOutConfig cfg = new RollOutConfig();
        
        // Modify the config on the server side in this function based on parameters
        
        return cfg;
    }
    

    #endregion
}