using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AO;

namespace Assembly.scripts.SceneObjects.Crates
{
    public struct CrateDropConfig
    {
        [Serialized] public string DropName = "Nothing"; // Used for handler
        [Serialized] public string DropDisplayName = "Nothing!"; // Popup text
        [Serialized] public string DropTexturePath = ""; // Load Texture on drop

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
        public static readonly CrateDropConfig CoinsConfig = new CrateDropConfig(){
            DropName = "Coin",
            DropDisplayName = "Coin +5!",
            DropTexturePath = "Props/DropItems/coin/coin_1.png",
        };
        
        public static readonly CrateDropConfig HealthPotionSConfig = new CrateDropConfig(){
            DropName = "HealthPotionS",
            DropDisplayName = "Small Health Potion",
            DropTexturePath = "Props/DropItems/HealthPotionS.png"
        };

        public static readonly CrateDropConfig HealthPotionMConfig = new CrateDropConfig(){
            DropName = "HealthPotionM",
            DropDisplayName = "Health Potion",
            DropTexturePath = "Props/DropItems/HealthPotionM.png"
        };

        public static readonly CrateDropConfig HealthPotionLConfig = new CrateDropConfig(){
            DropName = "HealthPotionL",
            DropDisplayName = "Large Health Potion",
            DropTexturePath = "Props/DropItems/HealthPotionL.png"
        };

        public static readonly CrateDropConfig ExpPotionLConfig = new CrateDropConfig()
        {
            DropName = "ExpPotionL",
            DropDisplayName = "Exp +5000!",
            DropTexturePath = "Props/DropItems/ExpPotionL.png"
        };

        public static readonly CrateDropConfig MagicPunchPotionConfig = new CrateDropConfig()
        {
            DropName = "MagicPunchPotion",
            DropDisplayName = "Magic Punch Potion",
            DropTexturePath = "Props/DropItems/MagicPunchPotion.png"
        };
        
        public static readonly CrateDropConfig RagePotionConfig = new CrateDropConfig()
        {
            DropName = "RagePotion",
            DropDisplayName = "Rage Potion",
            DropTexturePath = "Props/DropItems/RagePotion.png"
        };

        public static readonly Dictionary<string, CrateDropConfig> CrateDropConfigs = new()
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