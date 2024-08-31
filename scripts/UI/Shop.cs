namespace Assembly.scripts.UI;

using AO;

// TODO: Replace this shop with engine level feature
public class Shop : System<Shop>
{
    public static readonly Vector4 LightRed = new Vector4(255.0f/255.0f, 81.0f/255.0f, 65.0f/255.0f, 1.0f);
    public static readonly Vector4 DarkRed = new Vector4(181.0f/255.0f, 41.0f/255.0f, 28.0f/255.0f, 1.0f);
    public static readonly Vector4 LightGreen = new Vector4(144.0f/255.0f, 221.0f/255.0f, 13.0f/255.0f, 1.0f);
    public static readonly Vector4 DarkGreen = new Vector4(127.0f/255.0f, 217.0f/255.0f, 27.0f/255.0f, 1.0f);
    public static readonly Vector4 LightBlue = new Vector4(0.0f/255.0f, 177.0f/255.0f, 253.0f/255.0f, 1.0f);
    public static readonly Vector4 DarkBlue = new Vector4(4.0f/255.0f, 163.0f/255.0f, 254.0f/255.0f, 1.0f);
    public static readonly Vector4 Gold = new Vector4(245.0f/255.0f, 211.0f/255.0f, 42.0f/255.0f, 1.0f);
    public static readonly Vector4 Purple = new Vector4(161.0f/255.0f, 48.0f/255.0f, 214.0f/255.0f, 1.0f);
    public static readonly Vector4 PathColor = new Vector4(248.0f/255.0f, 225.0f/255.0f, 164.0f/255.0f, 1.0f);

    public Random Random;

    public string ScrollToIAP;

    public override void Start()
    {
        if (Network.IsServer)
        {
            Purchasing.SetPurchaseHandler(OnPurchase);
        }
        Random = new Random();
    }

    public override void Update()
    {
    }

    public string GetProductIdForItem(string itemId)
    {
        var item = ShopData.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
        {
            return string.Empty;
        }
        return item.ProductId;
    }
    

    public bool DrawSingleShopEntry(Rect entryRect, ShopData.ShopEntry entry, UI.ScrollView scrollView, UI.TextSettings buyButtonTextSettings, UI.ButtonSettings buyButtonSettings, Vector4 backgroundColor, UI.HorizontalAlignment descAlign)
    {
        UI.ExpandCurrentScrollView(entryRect);

        var item = ShopData.Items.Find(i => i.Id == entry.ItemId);
        if (item == null)
        {
            Log.Error($"Could not find item {entry.ItemId}");
            return false;
        }

        var product = Purchasing.GetProduct(item.ProductId);
        if (!product.IsValid())
        {
            Log.Error($"Invalid product {item.ProductId}");
            return false;
        }

        if (entry.HideIfOwned && product.IsGamePass && Purchasing.OwnsGamePassLocal(item.ProductId))
        {
            return false;
        }
        
        
        var fp = Network.LocalPlayer as FightPlayer;
        bool disabled = item.Kind == ShopData.ItemKind.Boost && fp?.ExpBoostTime > 0;
        using var _1 = UI.PUSH_DISABLED(disabled);
        if (disabled)
        {
            UI.PushColorMultiplier(new Vector4(0.5f, 0.5f, 0.5f, 1f));
        }

        using var __ = UI.PUSH_ID(entry.ItemId);

        if (item.HardcodedIcon.Has())
        {
            Texture iconTexture = Assets.GetAsset<Texture>(item.HardcodedIcon);
            buyButtonSettings.Sprite = iconTexture;
        }

        var buyButtonResult = UI.BeginButton(entryRect, string.Empty, buyButtonSettings, default);
        {
            using var _123 = AllOut.Defer(UI.EndButton);

            if (!item.HardcodedIcon.Has())
            {
                UI.Image(buyButtonResult.Rect, FightClubGameManager.References.FrameWhite, backgroundColor, SceneReferenceHolder.WhiteFrameSlice);
            }

            var entryCutRect = buyButtonResult.Rect.Inset(15, 25, 25, 25);
            var titleTextRect = entryCutRect.CutTop(100);
            
            if (item.HardcodedIcon.Has())
            {
                // UI.Image(iconsRect.FitAspect(product.Icon.Aspect), product.Icon, Vector4.White);
            }
            else
            {
                var hasDescription = product.Description.Has();
                Rect iconsRect = entryCutRect.InsetBottom(85);
                if (hasDescription)
                {
                    iconsRect = iconsRect.Offset(0, 0);
                }
                if (product.Icon != null)
                {
                    UI.Image(iconsRect.FitAspect(product.Icon.Aspect), product.Icon, Vector4.White);
                }
                if (hasDescription)
                {
                    var color = new Vector4((float)0xB1 / 255.0f, (float)0xF3 / 255.0f, (float)0xFA / 255.0f, 1);
                    UI.Text(iconsRect.Offset(0, -75).Inset(0, 0, 0, 0), product.Description, new UI.TextSettings() {
                        Font = UIManager.DefaultFont,
                        Size = 30,
                        Color = color,
                        Outline = true,
                        OutlineThickness = 3,
                        HorizontalAlignment = descAlign,
                        VerticalAlignment = UI.VerticalAlignment.Center,
                        WordWrap = true
                    });
                }
            }

            UI.Text(titleTextRect, product.Name, new UI.TextSettings() {
                Font = UIManager.DefaultFont,
                Size = 36 * item.TitleSizeMultiplier,
                Color = new Vector4((float)0xFE / 255.0f, (float)0xCB / 255.0f, (float)0x34 / 255.0f, 1),
                Outline = true,
                OutlineThickness = 3,
                HorizontalAlignment = UI.HorizontalAlignment.Center,
                VerticalAlignment = UI.VerticalAlignment.Center,
                WordWrap = true,
                LineHeightMultiplier = 0.8f
            });

            var costRect = buyButtonResult.Rect.BottomRect().GrowTop(75);
            UI.Image(costRect, FightClubGameManager.References.FrameWhiteBottom, new Vector4(0, 0, 0, 0.25f));
            var costTextRect = UI.Text(costRect, $"   {product.Price}", buyButtonTextSettings);
            UI.Image(costTextRect.LeftRect().FitAspect(FightClubGameManager.References.SparkIcon.Aspect, Rect.FitAspectKind.KeepHeight).Inset(5).Offset(0, 0), FightClubGameManager.References.SparkIcon);
        }

        if (buyButtonResult.Clicked)
        {
            Purchasing.PromptPurchase(item.ProductId);
        }

        if (product.IsGamePass && Purchasing.OwnsGamePassLocal(item.ProductId))
        {
            UI.Button(entryRect, "already_owned", new UI.ButtonSettings() {
                Sprite = FightClubGameManager.References.FrameWhite,
                Slice = SceneReferenceHolder.WhiteFrameSlice,
                AllColors = new Vector4(0, 0, 0, 0.8f)
            }, default);
            var checkmarkRect = entryRect.CenterRect().Grow(100);
            UI.Image(checkmarkRect, FightClubGameManager.References.CheckMark, Vector4.White);
        }

        if (ScrollToIAP.Has() && ScrollToIAP == entry.ItemId)
        {
            ScrollToIAP = string.Empty;
            scrollView.ScrollTo(entryRect);
        }

        if (disabled)
        {
            UI.PopColorMultiplier();
            UI.Text(entryRect, $"{fp.ExpBoostTime} Min Left", new UI.TextSettings() {
                Font = UIManager.DefaultFont,
                Size = 36,
                Color = new Vector4((float)0xFE / 255.0f, (float)0xCB / 255.0f, (float)0x34 / 255.0f, 1),
                Outline = true,
                OutlineThickness = 3,
                HorizontalAlignment = UI.HorizontalAlignment.Center,
                VerticalAlignment = UI.VerticalAlignment.Center,
                WordWrap = true,
                LineHeightMultiplier = 0.8f
            });
        }
        
        return true;
    }
    

    public const float GridPadding = 18;

    public bool DrawShop(string windowTitle, ShopData.ShopDefinition shopDefinition)
    {
        using var _3 = UI.PUSH_LAYER(10);

        bool result = true;
        Rect windowRect = UIManager.Instance.DoNormalWindowFrame(windowTitle, Vector4.White, FightClubGameManager.References.SparkIcon, 1200, 700, ref result, UIManager.Instance.TimeShopOpened);

        var buyButtonSettings = UIManager.Instance.GetButtonSettings(FightClubGameManager.References.FrameWhite);
        buyButtonSettings.Slice = SceneReferenceHolder.WhiteFrameSlice;

        var buttonTextSettings = new UI.TextSettings() 
        {
            Font = UIManager.DefaultFont,
            Size = 32,
            Color = Vector4.Black,
            HorizontalAlignment = UI.HorizontalAlignment.Center,
            VerticalAlignment = UI.VerticalAlignment.Center,
            WordWrap = false,
            WordWrapOffset = 0,
            Offset = new Vector2(0, 10),
            // DropShadow = false,
            // Outline= false,
        };

        var itemNameTextSettings = new UI.TextSettings()
        {
            Font = UIManager.DefaultFont,
            Size = 32,
            Color = Vector4.Black,
            HorizontalAlignment = UI.HorizontalAlignment.Left,
            VerticalAlignment = UI.VerticalAlignment.Center,
        };

        var tabButtonSettings = new UI.ButtonSettings()
        {
            Sprite = FightClubGameManager.References.FrameWhite,
            Slice = SceneReferenceHolder.WhiteFrameSlice,
            PressScaling = 0.8f,
        };

        // Tab is unused at this moment
        var tabButtonTextSettings = itemNameTextSettings;
        tabButtonTextSettings.Offset = new Vector2(-13, 0);
        tabButtonTextSettings.HorizontalAlignment = UI.HorizontalAlignment.Center;
        string clickedTab = string.Empty;

        {
            var buyButtonTextSettings = new UI.TextSettings()
            {
                Font = UIManager.DefaultFont,
                Size = 48,
                Color = new Vector4((float)0x83 / 255.0f, (float)0xE3 / 255.0f, (float)0x4C / 255.0f, 1),
                HorizontalAlignment = UI.HorizontalAlignment.Center,
                VerticalAlignment = UI.VerticalAlignment.Center,
                Outline = true,
                OutlineThickness = 3,
            };

            var scrollRect = windowRect.Inset(2, 0, 2, 0);
            var scrollView = UI.PushScrollView("pets_scroll_view", scrollRect, new UI.ScrollViewSettings() { Vertical = true, ClipPadding = new Vector4(0,5,0,5) });
            using var _ = AllOut.Defer(UI.PopScrollView);

            var first = true;
            foreach (var category in shopDefinition.Categories)
            {
                if (!first)
                {
                    var padding = scrollView.contentRect.CutTop(20);
                    UI.Image(padding.Inset(5, 50, 5, 50), null, new Vector4(0, 0, 0, 1));
                }
                first = false;
                scrollView.contentRect.CutTop(15);
                var categoryTitleRect = scrollView.contentRect.CutTop(75);
                scrollView.contentRect.CutTop(10);
                var titleBgSerial = IM.GetNextSerial();
                var actualTitleRect = UI.Text(categoryTitleRect, category.Name, new UI.TextSettings() {
                    Font = UIManager.DefaultFont,
                    Size = 60,
                    Color = Shop.Gold,
                    Outline = true,
                    OutlineThickness = 3,
                    HorizontalAlignment = UI.HorizontalAlignment.Center,
                    VerticalAlignment = UI.VerticalAlignment.Bottom
                });
                IM.SetNextSerial(titleBgSerial);
                UI.Image(actualTitleRect.Grow(2, 20, 2, 20), FightClubGameManager.References.FrameWhite, new Vector4(0, 0, 0, 0.5f), SceneReferenceHolder.WhiteFrameSlice);

                if (clickedTab == category.Name)
                {
                    clickedTab = string.Empty;
                    scrollView.ScrollTo(categoryTitleRect.TopRect(), UI.ScrollToRectKind.Top);
                }

                var gridRect = scrollView.contentRect.TopRect().GrowBottom(400);
                var productsGrid = UI.GridLayout.Make(gridRect, 3, 1, UI.GridLayout.SizeSource.ElementCount, GridPadding);
                Rect lastGridElement = scrollView.contentRect;
                foreach (var row in category.Rows)
                {
                    if (row.ShowIfOwnOther.Has() && !Purchasing.OwnsGamePassLocal(GetProductIdForItem(row.ShowIfOwnOther)))
                    {
                        continue;
                    }

                    switch (row.DisplaySize)
                    {
                        case ShopData.ItemDisplaySize.SingleBigEntry:
                        {
                            Util.Assert(row.Entries.Count == 1);
                            lastGridElement = productsGrid.Next();
                            lastGridElement = lastGridElement.Encapsulate(productsGrid.Next());
                            lastGridElement = lastGridElement.Encapsulate(productsGrid.Next());
                            DrawSingleShopEntry(lastGridElement, row.Entries[0], scrollView, buyButtonTextSettings, buyButtonSettings, category.BackgroundColor, UI.HorizontalAlignment.Center);
                            break;
                        }
                        case ShopData.ItemDisplaySize.DoubleEntry:
                        {
                            Util.Assert(row.Entries.Count == 2);
                            lastGridElement = productsGrid.Next();
                            lastGridElement = lastGridElement.Encapsulate(productsGrid.Next());
                            lastGridElement = lastGridElement.Encapsulate(productsGrid.Next());
                            Rect left  = lastGridElement.SubRect(0, 0, 0.5f, 1, 0, GridPadding/2, 0, 0);
                            Rect right = lastGridElement.SubRect(0.5f, 0, 1, 1, 0, 0, 0, GridPadding/2);
                            DrawSingleShopEntry(left,  row.Entries[0], scrollView, buyButtonTextSettings, buyButtonSettings, category.BackgroundColor, UI.HorizontalAlignment.Center);
                            DrawSingleShopEntry(right, row.Entries[1], scrollView, buyButtonTextSettings, buyButtonSettings, category.BackgroundColor, UI.HorizontalAlignment.Center);
                            break;
                        }
                        case ShopData.ItemDisplaySize.TripleEntry:
                        {
                            Util.Assert(row.Entries.Count == 3);
                            foreach (var entry in row.Entries)
                            {
                                lastGridElement = productsGrid.Next();
                                DrawSingleShopEntry(lastGridElement, entry, scrollView, buyButtonTextSettings, buyButtonSettings, category.BackgroundColor, UI.HorizontalAlignment.Center);
                            }
                            break;
                        }
                    }
                }
                var totalGridRect = gridRect.Encapsulate(lastGridElement.BottomRect().CutTop(GridPadding));
                scrollView.contentRect.CutTopUnscaled(totalGridRect.Height);
                UI.ExpandCurrentScrollView(totalGridRect);
            }
        }

        ScrollToIAP = string.Empty;

        return result;
    }

    public bool OnPurchase(Player purchaser, string productId)
    {
        var item = ShopData.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null) 
        {
            Log.Error($"Failed to find item: {productId}. Cannot complete purhcase!");
            return false;
        }

        var (success, _) = GrantItem(purchaser, item);
        Save.ForceSavePlayer(purchaser);
        return success;
    }

    public long THOUSAND(long n)
    {
        return n * 1000;
    }

    public long MILLION(long n)
    {
        return n * 1000 * 1000;
    }

    public float MINUTES(float seconds)
    {
        return seconds * 60;
    }

    public float HOURS(float seconds)
    {
        return seconds * 3600;
    }
    

    public (bool, string) GrantItem(Player p, ShopData.Item item, bool allowOpeningEggs = true)
    {
        FightPlayer player = (FightPlayer)p;
        
        if (item.Kind == ShopData.ItemKind.Pass)
        {
            switch (item.Id)
            {
                case "starter_pack1": player.Exp += LevelingData.BaselineXp[4];
                    return (true, "");
                case "starter_pack2": player.Exp += LevelingData.BaselineXp[9];
                    return (true, "");
                case "starter_pack3": player.Exp += LevelingData.BaselineXp[14];
                    return (true, "");
                default:
                    Log.Error($"Unknown pass: {item.Id}");
                    return (false, "");
            }
        }

        if (item.Kind == ShopData.ItemKind.Boost)
        {
            switch (item.Id)
            {
                // TODO
                /*case "2x_trophy_potion":         player.ServerGiveTemporaryBuff(StatModifierKind.Trophies,   2.0f, MINUTES(10)); return (true, "");
                case "2x_coins_potion":          player.ServerGiveTemporaryBuff(StatModifierKind.Money,      2.0f, MINUTES(20)); return (true, "");
                case "2x_click_power_potion":    player.ServerGiveTemporaryBuff(StatModifierKind.ClickPower, 2.0f, MINUTES(30)); return (true, "");
                case "potion_bundle":
                {
                    player.ServerGiveTemporaryBuff(StatModifierKind.Trophies,   2.0f, MINUTES(10));
                    player.ServerGiveTemporaryBuff(StatModifierKind.Money,      2.0f, MINUTES(20));
                    player.ServerGiveTemporaryBuff(StatModifierKind.ClickPower, 2.0f, MINUTES(30));
                    return (true, "");
                }*/
                case "xp_booster_3x": player.AddExpBoostTime(15, 3);
                    return (true, "");
                case "xp_booster_5x": player.AddExpBoostTime(15, 5);
                    return (true, "");
                case "xp_booster_7x": player.AddExpBoostTime(15, 7);
                    return (true, "");
            }

            Log.Error($"Unknown boost: {item.Id}");
            return (false, "");
        }

        if (item.Kind == ShopData.ItemKind.Coins)
        {
            switch (item.Id)
            {
                case "gems_1000": player.Gem += 1000;
                    return (true, "");
                case "gems_5000": player.Gem += 5000;
                    return (true, "");
                case "gems_10000": player.Gem += 10000;
                    return (true, "");
            }
            Log.Error($"Unknown coin pack: {item.Id}");
            return (false, "");
        }

        if (item.Kind == ShopData.ItemKind.Pack)
        {
            var pack = ShopData.Packs.FirstOrDefault(p => p.Id == item.Id);
            if (pack == null)
            {
                Log.Error($"Could not find pack definition for {item.Id}");
                return (false, "");
            }

            foreach(var itemId in pack.Items)
            {
                var packItem = ShopData.Items.FirstOrDefault(i => i.Id == itemId);
                if (packItem == null)
                {
                    Log.Error($"Could not find pack item definition for {itemId}");
                    continue;
                }

                var (success, data) = GrantItem(player, packItem, false);
                if (!success)
                {
                    Log.Error($"Failed to grant pack item: {packItem.Id}");
                }
                
            }
            

            return (true, "");
        }

        Log.Error($"Unhandled item kind: {item.Kind}. Id: {item.Id}");
        return (false, "");
    }

    public static T EvaluateWeightedRandom<T>(Random random, List<T> list, Func<T, float> weightGetter) where T : class
    {
        float totalWeight = list.Sum(weightGetter);
        var rnd = random.NextSingle() * totalWeight;

        T selected = null;
        float cursor = 0;
        foreach(var elem in list)
        {
            if (rnd >= cursor && rnd < (cursor + weightGetter(elem)))
            {
                selected = elem;
                break;
            }
            cursor += weightGetter(elem);
        }
        return selected;
    }
}

public static class ShopData
{
    public static ShopDefinition MainWorldShopDefinition = new ShopDefinition()
    {
        Categories = new List<ShopCategory>()
        {

            new ShopCategory()
            {
                Name = "Enhancements",
                BackgroundColor = new Vector4((float)0xB5 / 255.0f, (float)0x7A / 255.0f, (float)0xF8 / 255.0f, 1),
                Rows = new List<ShopRow>()
                {
                    new ShopRow()
                    {
                        DisplaySize = ItemDisplaySize.TripleEntry,
                        Entries = new()
                        {
                            new () { ItemId = "xp_booster_3x",     Icons = new() { "" } },
                            new () { ItemId = "xp_booster_5x",             Icons = new() { "" } },
                            new () { ItemId = "xp_booster_7x",             Icons = new() { "" } },

                        },
                    },
                    new ShopRow()
                    {
                        DisplaySize = ItemDisplaySize.TripleEntry,
                        Entries = new()
                        {
                            new () { ItemId = "gems_1000",     Icons = new() { "" } },
                            new () { ItemId = "gems_5000",             Icons = new() { "" } },
                            new () { ItemId = "gems_10000",             Icons = new() { "" } },

                        },
                    },
                    new ShopRow()
                    {
                        DisplaySize = ItemDisplaySize.SingleBigEntry,
                        Entries = new()
                        {
                            new () { ItemId = "starter_pack1",     Icons = new() { "" } },
                        }
                    },
                    new ShopRow()
                    {
                        DisplaySize = ItemDisplaySize.SingleBigEntry,
                        Entries = new()
                        {
                            new () { ItemId = "starter_pack2",     Icons = new() { "" } },
                        }
                    },
                    new ShopRow()
                    {
                        DisplaySize = ItemDisplaySize.SingleBigEntry,
                        Entries = new()
                        {
                            new () { ItemId = "starter_pack3",     Icons = new() { "" } },
                        }
                    },
                }
            },
            
        }
    };
    

    public static List<Pack> Packs = new List<Pack>()
    {
        
    };

    public static List<Item> Items = new List<Item>()
    {
        new () { Id = "starter_pack1",        ProductId = "66cce39be9d028e4047b4687", Currency = Currency.Sparks, Kind = ItemKind.Pass, },
        new () { Id = "starter_pack2",        ProductId = "66cce3e1007431448b01c811", Currency = Currency.Sparks, Kind = ItemKind.Pass, },
        new () { Id = "starter_pack3",        ProductId = "66cce47ee9d028e4047b468a", Currency = Currency.Sparks, Kind = ItemKind.Pass, },


        new () { Id = "xp_booster_3x",                  ProductId = "66c959936203839a49e78260", Currency = Currency.Sparks, Kind = ItemKind.Boost, },
        new () { Id = "xp_booster_5x",          ProductId = "66c959ad7c153a47bed5102f", Currency = Currency.Sparks, Kind = ItemKind.Boost, },
        new () { Id = "xp_booster_7x",             ProductId = "66c95a367c153a47bed51030", Currency = Currency.Sparks, Kind = ItemKind.Boost, },
        
        new () { Id = "gems_1000",             ProductId = "66cd6aed2f5300747619dfcf", Currency = Currency.Sparks, Kind = ItemKind.Coins, },
        new () { Id = "gems_5000",             ProductId = "66cd6c7e0cad8f4bf7405094", Currency = Currency.Sparks, Kind = ItemKind.Coins, },
        new () { Id = "gems_10000",             ProductId = "66cd6c990cad8f4bf7405095", Currency = Currency.Sparks, Kind = ItemKind.Coins, },
    };

    public class Pack
    {
        public string Id;
        public List<string> Items;
    }

    public class ShopDefinition
    {
        public List<ShopCategory> Categories;
    }

    public class ShopCategory
    {
        public string Name;
        public Vector4 BackgroundColor = new Vector4(1, 1, 1, 1);
        public List<ShopRow> Rows;
    }

    public enum ItemDisplaySize
    {
        SingleBigEntry,
        DoubleEntry,
        TripleEntry,
    }

    public class ShopRow
    {
        public bool EventOnly;
        public ItemDisplaySize DisplaySize;
        public List<ShopEntry> Entries;
        public string ShowIfOwnOther;
    }

    public class ShopEntry
    {
        public string ItemId;
        public string Background;
        public List<string> Icons;
        public bool New;
        public bool Limitedtime;
        public bool HideIfOwned;
    }

    public enum ItemKind
    {
        Boost,
        Pass,
        Coins,
        Trophies,
        Pack,
        SkipRebirth,
    }

    public enum Currency
    {
        Coins,
        Trophies,
        Sparks,
    }

    public class Item
    {
        public string Id;
        public string ProductId;
        public string Name;
        public string Description;
        public string Category;
        public ItemKind Kind;
        public Currency Currency;
        public long Cost;
        public float TitleSizeMultiplier = 1.0f;

        public int IntData;
        public string StringData;

        public string HardcodedIcon;
    }
}
