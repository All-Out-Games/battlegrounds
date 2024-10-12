using System;
using AO;
namespace Assembly.scripts.SceneObjects.Crates
{
    public partial struct CratesConfig
    {
        // Use Util.SampleWeightedList to determine what's in the crate when it's spawning
        public int Prob = 0; // Relative Prob i.e. weights in the weighted list. Doesn't have to add up to 100
        public int HitPoint = 2; // HP of the crate. Punch reduces it by 1, AoE skills destroy them immediately.
        public string ItemName = "Nothing"; // Must correspond to CrateDropConfig
        public bool Special = false; // If a special prefab need to be spawned
        public Vector4 Tint = Vector4.White;

        public static CratesConfig HealthPotionSDropCfg = new CratesConfig{
            Prob = 80,
            ItemName = "HealthPotionS"
        };

        public static CratesConfig HealthPotionMDropCfg = new CratesConfig{
            Prob = 50,
            ItemName = "HealthPotionM"
        };

        public static CratesConfig HealthPotionLDropCfg = new CratesConfig{
            Prob = 25,
            HitPoint = 3, // Rarer items have higher HP crates as an indication of "You hit something big!"
            ItemName = "HealthPotionL"
        };

        public static CratesConfig CoinsDropCfg = new CratesConfig{
            Prob = 500,
            HitPoint = 1,
            ItemName = "Coin"
        };

        public static CratesConfig ExpPotionLCfg = new CratesConfig()
        {
            Prob = 1,
            HitPoint = 5,
            ItemName = "ExpPotionL",
            Tint = new Vector4(0.5f, 0.1f, 0.95f, 1f)
        };

        public static CratesConfig MagicPunchPotionCfg = new CratesConfig()
        {
            Prob = 20,
            HitPoint = 4,
            ItemName = "MagicPunchPotion",
            Tint = new Vector4(1f, 0.431f, 0.78f, 1f)
        };

        public static CratesConfig RagePotionCfg = new CratesConfig()
        {
            Prob = 20,
            HitPoint = 4,
            ItemName = "RagePotion",
            Tint = new Vector4(1f, 0.1f, 0.1f, 1f)
        };
        
        // Modify this list and create corresponding Item config in CrateDropConfig.cs
        public static List<CratesConfig> AllPossibleItems = new List<CratesConfig>()
        {
            HealthPotionSDropCfg,
            HealthPotionMDropCfg,
            HealthPotionLDropCfg,
            CoinsDropCfg,
            ExpPotionLCfg,
            MagicPunchPotionCfg,
            RagePotionCfg
        };

        public CratesConfig()
        {
        }
    }
}