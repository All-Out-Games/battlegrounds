using AO;

namespace Assembly.scripts.UI;

public partial class BgShop
{
    public static readonly List<ShopCategory.ProductDescription> StarterProducts = new()
    {
        new()
        {
            Name = "10 Luck Coupons", Rarity = ItemRarity.Uncommon,
            Id = "10_luck_coupon", SparksProductId = "6776179ee9dab16189587b54", Price = 0, Description = "Get 10 luck coupons to reroll your loadout.",
            SubCategory = "King of the Hill", Icon = "UI/KoH/LuckCoupon_10.png"
        },
        new()
        {
            Name = "Luck Coupon Bundle", Rarity = ItemRarity.Rare,
            Id = "25_luck_coupon", SparksProductId = "677617fce9dab16189587b65", Price = 0,
            Description = "Get 25% extra value by buying this bundle!",
            SubCategory = "King of the Hill", Icon = "UI/KoH/LuckCoupon_25.png"
        },
        new()
        {
            Name = "Luck Coupon Pack", Rarity = ItemRarity.Epic,
            Id = "50_luck_coupon", SparksProductId = "6776189ee9dab16189587b89", Price = 0,
            Description = "Get 50% extra value by buying this pack!",
            SubCategory = "King of the Hill", Icon = "UI/KoH/LuckCoupon_50.png"
        },
        new()
        {
            Name = "KoH Game Pass", Rarity = ItemRarity.Mythic,
            Id = "koh_gamepass", SparksProductId = "677616d6e9dab16189587b41", Price = 0,
            Description = "Buy it to be able to see the content of your random class. Your respawn will be shortened to 4s, and you get double glory when you win a round.",
            SubCategory = "Pass", Icon = "UI/KoH/GamePass.png"
        }
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