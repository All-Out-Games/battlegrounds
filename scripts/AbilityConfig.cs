using AO;

/// <summary>
/// Base Ability Config. Pass modified data to player based on player stats on the server
/// </summary>
public class AbilityConfig : System<AbilityConfig>
{
    #region RollOut

    public struct RollOutConfig
    {
        public float Duration = 10;
        public float ContactDamage = 5;
        public float SpeedBuffMultiplier = 1.5f;

        public RollOutConfig()
        {
        }
    }

    public RollOutConfig GetPlayerRollOutConfig(int level)
    {
        RollOutConfig cfg = new RollOutConfig();
        return cfg;
    }
    

    #endregion
}