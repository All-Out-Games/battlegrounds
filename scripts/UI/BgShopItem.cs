using AO;

namespace Assembly.scripts.UI;

public partial class BgShop
{
    public static readonly List<ShopCategory.ProductDescription> StarterProducts = new()
    {
        new()
        {
            Id = "starter_pack1", SparksProductId = "66cce39be9d028e4047b4687", Price = 0, Description = "Get 7,500 XP, which is enough to level you up to Lv.5.",
            SubCategory = "Pass"
        },
        new()
        {
            Id = "starter_pack2", SparksProductId = "66cce3e1007431448b01c811", Price = 0,
            Description = "Get 27,500 XP, which is enough to level you up to Lv.10.",
            SubCategory = "Pass"
        },
        new()
        {
            Id = "starter_pack3", SparksProductId = "66cce47ee9d028e4047b468a", Price = 0,
            Description = "Get 65,000 XP, which is enough to level you up to Lv.15.",
            SubCategory = "Pass"
        },
    };
    
    public static readonly List<ShopCategory.ProductDescription> PotionProducts = new()
    {
        new () { Id = "xp_booster_3x",                  SparksProductId = "66c959936203839a49e78260", Price = 0, Description = "Get 3x Exp for 15min."},
        new () { Id = "xp_booster_5x",          SparksProductId = "66c959ad7c153a47bed5102f", Price = 0, Description = "Get 5x Exp for 15min."},
        new () { Id = "xp_booster_7x",             SparksProductId = "66c95a367c153a47bed51030", Price = 0, Description = "Get 7x Exp for 15min." },
        
    };

    public static readonly List<ShopCategory.ProductDescription> ResourceProducts = new()
    {
        new () { Id = "gems_1000",             SparksProductId = "66cd6aed2f5300747619dfcf", Price = 0, Description = "Get 1000 Glory to upgrade your skills."},
        new () { Id = "gems_5000",             SparksProductId = "66cd6c7e0cad8f4bf7405094", Price = 0, Description = "Get 5000 Glory to upgrade your skills."},
        new () { Id = "gems_10000",             SparksProductId = "66cd6c990cad8f4bf7405095", Price = 0, Description = "Get 10000 Glory to max out your skills."},
        
        new () { Id = "coins_300",             SparksProductId = "673a845054d47ee427a09bf7", Price = 0, Description = "Get 300 coins to buy skills."},
        new () { Id = "coins_1000",             SparksProductId = "673a846619d7981b15615360",Price = 0, Description = "Get 100 coins to buy skills." },
        new () { Id = "coins_3000",             SparksProductId = "673a84a054d47ee427a09bfc", Price = 0, Description = "Get 3000 coins to buy skills." },
        new () { Id = "coins_10000",             SparksProductId = "673a84b95829946eee036a41",Price = 0 , Description = "Woah, 10000 coins!"},
    };
}