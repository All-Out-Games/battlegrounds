using AO;

namespace Assembly.scripts.UI;

public partial class BgShop
{
    public static readonly List<ShopCategory.ProductDescription> StarterProducts = new()
    {
        new()
        {
            Name = "Starter Pack Lv.5", Rarity = ItemRarity.Common,
            Id = "starter_pack1", SparksProductId = "66cce39be9d028e4047b4687", Price = 0, Description = "Get 7,500 XP, which is enough to level you up to Lv.5.",
            SubCategory = "Pass", Icon = "Props/DropItems/HealthPotionS.png"
        },
        new()
        {
            Name = "Starter Pack Lv.10", Rarity = ItemRarity.Uncommon,
            Id = "starter_pack2", SparksProductId = "66cce3e1007431448b01c811", Price = 0,
            Description = "Get 27,500 XP, which is enough to level you up to Lv.10.",
            SubCategory = "Pass", Icon = "Props/DropItems/HealthPotionM.png"
        },
        new()
        {
            Name = "Starter Pack Lv.15", Rarity = ItemRarity.Rare,
            Id = "starter_pack3", SparksProductId = "66cce47ee9d028e4047b468a", Price = 0,
            Description = "Get 65,000 XP, which is enough to level you up to Lv.15.",
            SubCategory = "Pass", Icon = "Props/DropItems/HealthPotionL.png"
        },
        new()
        {
            Name = "Mega Starter Pack", Rarity = ItemRarity.Mythic,
            Id = "starter_pack4", SparksProductId = "67ba22c40a64e563bd0bb383", Price = 0,
            Description = "Get 217500 XP, which is enough to level you up to Lv.30. Currently on a discount and will return to regular price (4999) sparks on Mar. 12!",
            SubCategory = "Pass", Icon = "Props/Shop/MegaPotion4.png"
        },
    };
    
    public static readonly List<ShopCategory.ProductDescription> PotionProducts = new()
    {
        new ()
        {
            Name = "EXP Booster 3x", Rarity = ItemRarity.Rare,
            Id = "xp_booster_3x",                  SparksProductId = "66c959936203839a49e78260", Price = 0, Description = "Get 3x Exp for 15min.", Icon = "Props/DropItems/ExpPotionS.png",
            SubCategory = "EXP Boost"
        },
        new ()
        {
            Name = "EXP Booster 5x", Rarity = ItemRarity.Epic,
            Id = "xp_booster_5x",          SparksProductId = "66c959ad7c153a47bed5102f", Price = 0, Description = "Get 5x Exp for 15min.", Icon = "Props/DropItems/ExpPotionL.png",
            SubCategory = "EXP Boost"
        },
        new ()
        {
            Name = "EXP Booster 7x", Rarity = ItemRarity.Legendary,
            Id = "xp_booster_7x",             SparksProductId = "66c95a367c153a47bed51030", Price = 0, Description = "Get 7x Exp for 15min.",Icon = "Props/DropItems/ExpPotionM.png" ,
            SubCategory = "EXP Boost"
        },
        new()
        {
            Name = "Spectral Potion", Rarity = ItemRarity.Rare,
            Id = "spectral_1x",             SparksProductId = "674ce976a25c7b4822207748", Price = 0,
            Description = "Consume to use Spectral Spawn. You get XP for spectating as a specter, or you can materialize and take some easy kills.",Icon = "Props/Shop/SpectralPotion.png" ,
            SubCategory = "Spectre Mode"
        },
        new()
        {
            Name = "Spectral Potion Bundle", Rarity = ItemRarity.Epic,
            Id = "spectral_5x",             SparksProductId = "674ce9b71dfd313ecb20dbca", Price = 0, 
            Description = "Get 5 Spectral Potions!",Icon = "Props/Shop/SpectralPotion5.png" ,
            SubCategory = "Spectre Mode"
        },
        new()
        {
            Name = "Spectral Potion Value Pack", Rarity = ItemRarity.Legendary,
            Id = "spectral_15x",             SparksProductId = "674cea08c2dd855678f8a554", Price = 0, 
            Description = "Get 15 Spectral Potions!",Icon = "Props/Shop/SpectralPotion15.png" ,
            SubCategory = "Spectre Mode"
        },
    };

    public static readonly List<ShopCategory.ProductDescription> ResourceProducts = new()
    {
        new ()
        {
            Name = "Small Glory Pack", Rarity = ItemRarity.Epic,
            Id = "gems_1000",             SparksProductId = "66cd6aed2f5300747619dfcf", Price = 0, Description = "Get 1000 Glory to upgrade your skills.", Icon = "Props/Shop/GlorySmall.png"
        },
        new ()
        {
            Name = "Medium Glory Pack", Rarity = ItemRarity.Legendary,
            Id = "gems_5000",             SparksProductId = "66cd6c7e0cad8f4bf7405094", Price = 0, Description = "Get 5000 Glory to upgrade your skills.", Icon = "Props/Shop/GloryMedium.png"
        },
        new ()
        {
            Name = "Large Glory Pack", Rarity = ItemRarity.Mythic,
            Id = "gems_10000",             SparksProductId = "66cd6c990cad8f4bf7405095", Price = 0, Description = "Get 10000 Glory to max out your skills.", Icon = "Props/Shop/GloryLarge.png"
        },
        
        new ()
        {
            Name = "A Handful of Coins", Rarity = ItemRarity.Common, Icon = "Props/Shop/Pack1.png",
            Id = "coins_300",             SparksProductId = "673a845054d47ee427a09bf7", Price = 0, Description = "Get 300 coins to buy skills."
        },
        new ()
        {
            Name = "Small Coin Pack", Rarity = ItemRarity.Uncommon,Icon = "Props/Shop/Pack2.png",
            Id = "coins_1000",             SparksProductId = "673a846619d7981b15615360",Price = 0, Description = "Get 1000 coins to buy skills."
        },
        new ()
        {
            Name = "Large Coin Pack", Rarity = ItemRarity.Rare,Icon = "Props/Shop/Pack3.png",
            Id = "coins_3000",             SparksProductId = "673a84a054d47ee427a09bfc", Price = 0, Description = "Get 3000 coins to buy skills."
        },
        new ()
        {
            Name = "A Load of Coins", Rarity = ItemRarity.Epic,Icon = "Props/Shop/Pack4.png",
            Id = "coins_10000",             SparksProductId = "673a84b95829946eee036a41",Price = 0 , Description = "Woah, 10000 coins!"
        },
    };
}