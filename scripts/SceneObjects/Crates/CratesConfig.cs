using System;
using AO;
namespace Assembly.scripts.SceneObjects.Crates
{
    public partial class CratesConfig
    {
        // Use Util.SampleWeightedList to determine what's in the crate when it's spawning
        public float Prob = 0; // Relative Prob i.e. weights in the weighted list. Doesn't have to add up to 100
        public string ItemName = "Nothing"; // Must correspond to CrateDropConfig

        public static CratesConfig HealthPotionSDropCfg = new CratesConfig{
            Prob = 24,
            ItemName = "HealthPotionS"
        };

        public static CratesConfig HealthPotionMDropCfg = new CratesConfig{
            Prob = 12,
            ItemName = "HealthPotionM"
        };

        public static CratesConfig HealthPotionLDropCfg = new CratesConfig{
            Prob = 6,
            ItemName = "HealthPotionL"
        };

        public static CratesConfig CoinsDropCfg = new CratesConfig{
            Prob = 30,
            ItemName = "Coins"
        };

    }
}