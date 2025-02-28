namespace Assembly.scripts.UI;

using AO;
public partial class BgShop : System<BgShop>
{
    public Shop ItemShop;
    public bool ItemShopOpen;
    private List<ShopCategory.ProductDescription> _allProducts;
    
    public override void Start()
    {
        _allProducts = new List<ShopCategory.ProductDescription>();
        _allProducts.AddRange(StarterProducts);
        _allProducts.AddRange(PotionProducts);
        _allProducts.AddRange(ResourceProducts);
        
        if (Network.IsServer) Purchasing.SetPurchaseHandler(SparksPurchaseHandler);
        

        ItemShop = Economy.CreateShop("Item Shop");
        ItemShop.SetPurchaseModifier(OnBeforeItemPurchase);
        if (Network.IsClient)
        {
            //ItemShop.SetCustomDisplay(CustomItemShopDisplay);
            ItemShop.SetCustomDisplay(CustomItemShopDisplay);
        }
        if (Network.IsServer)
        {
            // Don't need this rn. We don't have non-spark product
            //ItemShop.SetPurchaseHandler(OnItemPurchase);
        }
        
        var starterCat = ItemShop.AddCategory("Starter Packs");
        starterCat.Icon = "Props/DropItems/VengeancePotion.png";
        foreach (var p in StarterProducts)
        {
            starterCat.AddProduct(p);
        }
        
        var potionCat = ItemShop.AddCategory("Potions");
        potionCat.Icon = "Props/DropItems/ExpPotionL.png";
        foreach (var p in PotionProducts)
        {
            potionCat.AddProduct(p);
        }
        
        var resourceCat = ItemShop.AddCategory("Resources");
        resourceCat.Icon = "Props/DropItems/coin/coin_1.png";
        foreach (var p in ResourceProducts)
        {
            resourceCat.AddProduct(p);
        }
        
        base.Start();
    }
    
    public override void Update()
    {
        if (ItemShopOpen)
        {
            Rect rect = UI.ScreenRect.CenterRect().Grow(300, 500, 300, 500);
            using var _ = UI.PUSH_LAYER(100);
            ItemShopOpen = ItemShop.Draw(rect);
        }
    }

    private bool SparksPurchaseHandler(Player p, string productId)
    {
        bool success = false;
        FightPlayer player = (FightPlayer)p;
        
        if (player.Alive())
        {
            var prod = _allProducts.FirstOrDefault(prod => prod.SparksProductId == productId);
            success = true;

            if (productId == "678014341902847169c60258")
            {
                return true; // This is the KoTH early access. Special case, should grant nothing but return true.
            }
            
            switch (prod.Id)
            {
                case "starter_pack1": player.Exp += LevelingData.BaselineXp[4];
                    break;
                case "starter_pack2": player.Exp += LevelingData.BaselineXp[9];
                    break;
                case "starter_pack3": player.Exp += LevelingData.BaselineXp[14];
                    break;
                
                case "xp_booster_3x": player.AddExpBoostTime(15, 3);
                    break;
                case "xp_booster_5x": player.AddExpBoostTime(15, 5);
                    break;
                case "xp_booster_7x": player.AddExpBoostTime(15, 7);
                    break;
                
                case "gems_1000": player.Gem += 1000;
                    break;
                case "gems_5000": player.Gem += 5000;
                    break;
                case "gems_10000": player.Gem += 10000;
                    break;
                case "coins_300": player.Coins += 300;
                    break;
                case "coins_1000": player.Coins += 1000;
                    break;
                case "coins_3000": player.Coins += 3000;
                    break;
                case "coins_10000": player.Coins += 10000;
                    break;
                
                case "spectral_1x": player.SpectralCount += 1;
                    break;
                case "spectral_5x": player.SpectralCount += 5;
                    break;
                case "spectral_15x": player.SpectralCount += 15;
                    break;
                default:
                    Log.Error($"Product Id: {productId} is not found in registered products!");
                    success = false;
                    break;
            }
            
        }
        else
        {
            Log.Error("Trying to grant item to a destroyed player!");
            success = false;
        }
        
        //var (success, _) = GrantItem(p, item);
        Save.ForceSavePlayer(p);
        return success;
    }

    public void CustomItemShopDisplay(GameProduct product, Rect rect)
    {
        rect.CutTop(10);
        var descriptionRect = rect.CutTop(200).Inset(0, 15, 0, 15);
        UI.Text(descriptionRect, product.Description, new UI.TextSettings()
        {
            Font = UI.Fonts.Barlow,
            Size = 32,
            VerticalAlignment = UI.VerticalAlignment.Top,
            HorizontalAlignment = UI.HorizontalAlignment.Center,
            Color = Vector4.White,
            WordWrap = true,
            Outline = true,
            OutlineThickness = 3.0f,
            DoAutofit = true,
            AutofitMinSize = 16,
            AutofitMaxSize = 32,
        });
        descriptionRect = rect.CutTop(25);

        if (Network.LocalPlayer != null && product.SubCategory == "Pass")
        {
            bool owned = Purchasing.OwnsGamePassLocal(product.SparksProductId);
            UI.Text(descriptionRect, owned? "Owned" : "", new UI.TextSettings()
            {
                Font = UI.Fonts.Barlow,
                Size = 24,
                VerticalAlignment = UI.VerticalAlignment.Top,
                HorizontalAlignment = UI.HorizontalAlignment.Center,
                Color = GlobalData.GreyColor,
                WordWrap = true,
                Outline = true,
                OutlineThickness = 3.0f,
                DoAutofit = true,
                AutofitMinSize = 24,
                AutofitMaxSize = 24,
            });
        }

    }
    
    public PurchaseModification OnBeforeItemPurchase(Player _player, GameProduct product)
    {
        var player = (FightPlayer)_player;

        var modification = new PurchaseModification(product);

        modification.ModifyProduct = false;
        if (player.Alive())
        {
            if (product.SubCategory == "Pass")
            {
                if (Purchasing.OwnsGamePassLocal(product.SparksProductId))
                {
                    modification.ModifyProduct = true;
                    modification.Color = PurchaseButtonColor.Grey;
                    modification.OnBuyButtonClicked = () => PurchaseFail(_player, "You already purchased this item!");
                }
            }

            // Players are only allowed to buy the same exp boosts (prevent overwriting)
            if (product.SubCategory == "EXP Boost" && player.ExpBoostMultiplier != 1)
            {
                switch (product.Id)
                {
                    case "xp_booster_3x":
                        if (player.ExpBoostMultiplier != 3)
                        {
                            modification.ModifyProduct = true;
                            modification.Color = PurchaseButtonColor.Grey;
                            modification.OnBuyButtonClicked = () => PurchaseFail(_player, "You own a booster of different multiplier. Please wait until it expires.");
                        }
                        break;
                    case "xp_booster_5x":
                        if (player.ExpBoostMultiplier != 5)
                        {
                            modification.ModifyProduct = true;
                            modification.Color = PurchaseButtonColor.Grey;
                            modification.OnBuyButtonClicked = () => PurchaseFail(_player, "You own a booster of different multiplier. Please wait until it expires.");
                        }
                        break;
                    case "xp_booster_7x":
                        if (player.ExpBoostMultiplier != 7)
                        {
                            modification.ModifyProduct = true;
                            modification.Color = PurchaseButtonColor.Grey;
                            modification.OnBuyButtonClicked = () => PurchaseFail(_player, "You own a booster of different multiplier. Please wait until it expires.");
                        }
                        break;
                }
            }
        }

        

        return modification;
    }


    void PurchaseFail(Player player, string text = "Purchase failed.")
    {
        if (player.IsLocal)
        {
            Notifications.Show(text);
        }
    }
}