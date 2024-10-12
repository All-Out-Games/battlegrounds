using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AO;

namespace Assembly.scripts.SceneObjects.Crates
{
    public struct CrateDropConfig
    {
        public string DropName = "Nothing"; // Used for handler
        public string DropDisplayName = "Nothing!"; // Popup text
        public string DropTexturePath = ""; // Load Texture on drop

        public CrateDropConfig()
        {
        }

        public static string GetDisplayString(FightPlayer grantedPlayer, CrateDropConfig cfg)
        {
            if (!grantedPlayer.Alive())
            {
                return "";
            }
            string res = cfg.DropDisplayName;
            switch (cfg.DropName)
            {
                case "ExpPotionL":
                    if (grantedPlayer.Level >= LevelingData.MaxLevel)
                    {
                        res = "Glory +200";
                    }
                    break;
                default:
                    break;
            }
            return res;
        }
    }
    
    public partial struct CratesConfig
    {
        public static CrateDropConfig CoinsConfig = new CrateDropConfig(){
            DropName = "Coin",
            DropDisplayName = "Coin +5!",
            DropTexturePath = "Props/DropItems/coin/coin_1.png",
        };
        
        public static CrateDropConfig HealthPotionSConfig = new CrateDropConfig(){
            DropName = "HealthPotionS",
            DropDisplayName = "Small Health Potion",
            DropTexturePath = "Props/DropItems/HealthPotionS.png"
        };

        public static CrateDropConfig HealthPotionMConfig = new CrateDropConfig(){
            DropName = "HealthPotionM",
            DropDisplayName = "Health Potion",
            DropTexturePath = "Props/DropItems/HealthPotionM.png"
        };

        public static CrateDropConfig HealthPotionLConfig = new CrateDropConfig(){
            DropName = "HealthPotionL",
            DropDisplayName = "Large Health Potion",
            DropTexturePath = "Props/DropItems/HealthPotionL.png"
        };

        public static CrateDropConfig ExpPotionLConfig = new CrateDropConfig()
        {
            DropName = "ExpPotionL",
            DropDisplayName = "Exp +5000!",
            DropTexturePath = "Props/DropItems/ExpPotionL.png"
        };

        public static CrateDropConfig MagicPunchPotionConfig = new CrateDropConfig()
        {
            DropName = "MagicPunchPotion",
            DropDisplayName = "Magic Punch Potion",
            DropTexturePath = "Props/DropItems/MagicPunchPotion.png"
        };
        
        public static CrateDropConfig RagePotionConfig = new CrateDropConfig()
        {
            DropName = "RagePotion",
            DropDisplayName = "Rage Potion",
            DropTexturePath = "Props/DropItemsRagePotion.png"
        };

        public static Dictionary<string, CrateDropConfig> CrateDropConfigs = new()
        {
            {CoinsConfig.DropName, CoinsConfig},
            {HealthPotionSConfig.DropName, HealthPotionSConfig},
            {HealthPotionMConfig.DropName, HealthPotionMConfig},
            {HealthPotionLConfig.DropName, HealthPotionLConfig},
            {ExpPotionLConfig.DropName, ExpPotionLConfig},
            {MagicPunchPotionConfig.DropName, MagicPunchPotionConfig},
            {RagePotionConfig.DropName, RagePotionConfig}
        };
    }
}