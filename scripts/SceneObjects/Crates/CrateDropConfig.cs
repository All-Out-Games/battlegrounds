using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assembly.scripts.SceneObjects.Crates
{
    public class CrateDropConfig
    {
        public string DropName = "Nothing"; // Used for handler
        public string DropDisplayName = "Nothing!"; // Display under the thing
        public string DropTexturePath = ""; // Load Texture on drop

        public bool Special = false; // Call a special handler when spawning the dropped item
    }
    public partial class CratesConfig
    {
        public static CrateDropConfig CoinsConfig = new CrateDropConfig(){
            DropName = "Coins",
            DropDisplayName = "",
            DropTexturePath = "Props/DropItems/HealthPotionS.png",
            Special = true
        };
        
        public static CrateDropConfig HealthPotionSConfig = new CrateDropConfig(){
            DropName = "HealthPotionS",
            DropDisplayName = "Small Health Potion",
            DropTexturePath = "Props/DropItems/HealthPotionS.png"
        };

        public static CrateDropConfig HealthPotionMConfig = new CrateDropConfig(){
            DropName = "HealthPotionS",
            DropDisplayName = "Health Potion",
            DropTexturePath = "Props/DropItems/HealthPotionM.png"
        };

        public static CrateDropConfig HealthPotionLConfig = new CrateDropConfig(){
            DropName = "HealthPotionS",
            DropDisplayName = "Large Health Potion",
            DropTexturePath = "Props/DropItems/HealthPotionL.png"
        };
    }
}