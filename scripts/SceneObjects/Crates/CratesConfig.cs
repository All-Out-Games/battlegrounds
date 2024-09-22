using System;
using AO;
namespace Assembly.scripts.SceneObjects.Crates
{
    public partial class CratesConfig
    {
        // Use Util.SampleWeightedList to determine what's in the crate when it's spawning
        public int Prob = 0; // Relative Prob i.e. weights in the weighted list. Doesn't have to add up to 100
        public int HitPoint = 2; // HP of the crate. Punch reduces it by 1, AoE skills destroy them immediately.
        public string ItemName = "Nothing"; // Must correspond to CrateDropConfig
        public bool Special = false; // If a special prefab need to be spawned

        public static CratesConfig HealthPotionSDropCfg = new CratesConfig{
            Prob = 5,
            ItemName = "HealthPotionS"
        };

        public static CratesConfig HealthPotionMDropCfg = new CratesConfig{
            Prob = 3,
            ItemName = "HealthPotionM"
        };

        public static CratesConfig HealthPotionLDropCfg = new CratesConfig{
            Prob = 2,
            HitPoint = 3, // Rarer items have higher HP crates as an indication of "You hit something big!"
            ItemName = "HealthPotionL"
        };

        public static CratesConfig CoinsDropCfg = new CratesConfig{
            Prob = 50,
            HitPoint = 1,
            ItemName = "Coin"
        };

        
        
        // Modify this list and create corresponding Item config in CrateDropConfig.cs
        public static List<CratesConfig> AllPossibleItems = new List<CratesConfig>()
        {
            HealthPotionSDropCfg,
            HealthPotionMDropCfg,
            HealthPotionLDropCfg,
            CoinsDropCfg
        };
    }
}