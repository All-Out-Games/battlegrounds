using AO;

public class UIManager : System<UIManager>
{
    // Update using event when you need to. This should avoid fetching references each frame which causes a slight overhead
    public Action<FightPlayer> UpdateUIEvent;
    public Action<string, float, Player> PopupEvent;

    private Dictionary<string, UniqueUIWindow> UniqueUiWindows = new(); // [PrefabPath : Window Class]

    private string _scoreTxt;
    private string _resourceTxt;
    private string _atkTxt;
    private string _multiplierTxt;
    private string _moneyTxt;

    private string _popupTxt;
    private float _popupRemainingTime;

    private FontAsset _defaultFont;
    private UI.ButtonSettings _defaultButtonSettings;
    private UI.TextSettings _defaultTextSettings;

    private UICanvas _mainCanvas;
    public override void Awake()
    {
        PopupEvent = SetPopup;
        _defaultFont = Assets.GetAsset<FontAsset>("$AO/fonts/Barlow-SemiBold.ttf");
        _defaultButtonSettings = new UI.ButtonSettings()
            { Sprite = Assets.GetAsset<Texture>("$AO/new/main_menu/bottom_bar/button.png") };
        _defaultTextSettings = new UI.TextSettings() { Font = _defaultFont, Size = 24, Color = Vector4.LightBlue };
    }
    
    public override void Start()
    {
        _scoreTxt = "0";
        _resourceTxt = "0";
        _moneyTxt = "0";
    }

    public UICanvas FindCanvas()
    {
        _mainCanvas ??= Entity.FindByName("Canvas").GetComponent<UICanvas>();
        return _mainCanvas;
    }
    public void SetPopup(string txt, float time, Player player)
    {
        if (!player.IsLocal)
        {
            return;
        }
        _popupRemainingTime = time;
        _popupTxt = txt;
        
    }

    public UniqueUIWindow OpenUniqueUIWindow(string prefabPath)
    {
        // Try get existing window
        UniqueUIWindow uniqueWd;
        if (!UniqueUiWindows.TryGetValue(prefabPath, out uniqueWd))
        {
            // If not exist, create one
            uniqueWd = UIWindow.InstantiateWindow(prefabPath) as UniqueUIWindow;
            if (uniqueWd == null)
            {
                Log.Error($"Cannot get a UniqueUIWindow component from {prefabPath}!");
                return null;
            }
            
            uniqueWd.Entity.SetParent(FindCanvas().Entity,false);
            UniqueUiWindows.Add(prefabPath, uniqueWd);
            uniqueWd.OnInstantiate();
        }
        
        // Close all that is currently active, then open window
        foreach (var kv in UniqueUiWindows)
        {
            UniqueUIWindow v = kv.Value;
            if (v.IsActive)
            {
                v.CloseWindow();
            }
        }
        uniqueWd.OpenWindow();
        return uniqueWd;
    }

    public UniqueUIWindow GetUniqueWindow(string prefabKey)
    {
        UniqueUIWindow uwd;
        bool exist = UniqueUiWindows.TryGetValue(prefabKey, out uwd);
        if (!exist)
        {
            Log.Error($"UIManager: Window {prefabKey} not created yet.");
        }
        return uwd;
    }

    public override void Update()
    {
        // Update timers
        {
            
            if (_popupRemainingTime > 0)
            {
                //Log.Warn(_popupTxt);
                _popupRemainingTime -= Time.DeltaTime;
                // Draw the popup
                {
                    var centerRect = UI.ScreenRect.CenterRect().Grow(235).CutBottom(50);
                    UI.Text(centerRect, $"{_popupTxt}", new UI.TextSettings() {Font = _defaultFont, Size = 24, Color = Vector4.Black, 
                        VerticalAlignment = UI.VerticalAlignment.Center, HorizontalAlignment = UI.HorizontalAlignment.Center,
                        WordWrap = true, Outline = true, OutlineColor = Vector4.White
                    });
                }
            }
            else
            {
                _popupRemainingTime = 0;
                _popupTxt = "";
            }
            
        }
        
        // Admin menus
        if (Network.LocalPlayer != null && Network.LocalPlayer.IsAdmin)
        {
            // Draw the score
            {
                var topBarRect = UI.ScreenRect.CutTop(80f);
                var currencyRect = topBarRect.CutLeft(225f).Offset(550f, -10f);
                UI.Image(currencyRect, Assets.GetAsset<Texture>("$AO/new/main_menu/bottom_bar/button.png"), Vector4.White);
                UI.Text(currencyRect, $"Score: {_scoreTxt}", new UI.TextSettings() {Font = _defaultFont,Size = 40, Color = Vector4.LightGreen, VerticalAlignment = UI.VerticalAlignment.Center, HorizontalAlignment = UI.HorizontalAlignment.Center });
                // Draw resources
                var resourceRect = topBarRect.CutLeft(225f).Offset(550f, -10f);
                UI.Image(resourceRect, Assets.GetAsset<Texture>("$AO/new/main_menu/bottom_bar/button.png"), Vector4.White);
                UI.Text(resourceRect, $"Material: {_resourceTxt}", new UI.TextSettings() {Font = _defaultFont, Size = 40, Color = Vector4.Green, VerticalAlignment = UI.VerticalAlignment.Center, HorizontalAlignment = UI.HorizontalAlignment.Center });
                // Draw money
                var moneyRect = topBarRect.CutLeft(225f).Offset(550f, -10f);
                UI.Image(moneyRect, Assets.GetAsset<Texture>("$AO/new/main_menu/bottom_bar/button.png"), Vector4.White);
                UI.Text(moneyRect, $"Money: {_moneyTxt}", new UI.TextSettings() { Font = _defaultFont, Size = 40, Color = Vector4.LightGreen, VerticalAlignment = UI.VerticalAlignment.Center, HorizontalAlignment = UI.HorizontalAlignment.Center });
            }


            // Draw the side buttons
            {

                var sideBarRect = UI.ScreenRect.LeftCenterRect().Grow(330, 100, 330, 0).Offset(5, 0);

                var buttonRect = sideBarRect.CutTop(100);
                if (UI.Button(buttonRect, $"Slot 1", new UI.ButtonSettings() { Sprite = Assets.GetAsset<Texture>("$AO/new/main_menu/bottom_bar/button.png") }, 
                        _defaultTextSettings).Clicked)
                {
                    var player = (FightPlayer)Network.LocalPlayer;
                    Log.Debug("Casting Slot 1");
                    player.GetEffectMgr().CallServer_CastRollOut("Slot1");
                }

                // Spacing
                sideBarRect.CutTop(10);

                var buttonRect2 = sideBarRect.CutTop(100);
                if (UI.Button(buttonRect2, $"Add BUMP", new UI.ButtonSettings() { Sprite = Assets.GetAsset<Texture>("$AO/new/main_menu/bottom_bar/button.png") }, 
                        _defaultTextSettings).Clicked)
                {
                    var player = (FightPlayer)Network.LocalPlayer;
                    Log.Info("Adding A bump!");
                    player.AddBump(new Vector2(20, 0), false);  // Add 
                    TestServerRPC.CallServer_AddBumpToNetworkID(player.Entity.NetworkId, new Vector2(120, 0));
                }
                
                // Spacing
                sideBarRect.CutTop(10);
                
                var buttonRect3 = sideBarRect.CutTop(100);
                if (UI.Button(buttonRect3, $"Unlock Rollout Slot 1",
                        new UI.ButtonSettings()
                            { Sprite = Assets.GetAsset<Texture>("$AO/new/main_menu/bottom_bar/button.png") },
                        _defaultTextSettings).Clicked)
                {
                    var player = (FightPlayer)Network.LocalPlayer;
                    Log.Info("Rollout Unlocked in Slot 1");
                    player.GetSkillTree().CallServer_UpgradeSkill("RollOut", 1);
                    player.GetSkillSlots().UpdateSlot("Slot1", 1, "RollOut");
                }
                
                // Spacing
                sideBarRect.CutTop(10);
                
                var buttonRect4 = sideBarRect.CutTop(100);
                if (UI.Button(buttonRect4, $"Ability Vendor",
                        new UI.ButtonSettings()
                            { Sprite = Assets.GetAsset<Texture>("$AO/new/main_menu/bottom_bar/button.png") },
                        _defaultTextSettings).Clicked)
                {
                    var player = (FightPlayer)Network.LocalPlayer;
                    OpenUniqueUIWindow("AbilityVendorMenuWindow.prefab");
                }
            }
        }

        
        
    }
}