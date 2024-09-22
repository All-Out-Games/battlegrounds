using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assembly.scripts.SceneObjects.Crates
{
    public class CrateDropConfig
    {
        public string DropName = "Nothing"; // Used for handler
        public string DropDisplayName = "Nothing!"; // Popup text
        public string DropTexturePath = ""; // Load Texture on drop
    }
    
    public partial class CratesConfig
    {
        public static CrateDropConfig CoinsConfig = new CrateDropConfig(){
            DropName = "Coin",
            DropDisplayName = "Coin +5 !",
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

        public static Dictionary<string, CrateDropConfig> CrateDropConfigs = new()
        {
            {CoinsConfig.DropName, CoinsConfig},
            {HealthPotionSConfig.DropName, HealthPotionSConfig},
            {HealthPotionMConfig.DropName, HealthPotionMConfig},
            {HealthPotionLConfig.DropName, HealthPotionLConfig},
        };
    }
}