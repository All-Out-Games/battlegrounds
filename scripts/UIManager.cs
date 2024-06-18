using AO;

public partial class UIManager : System<UIManager>
{

    private Dictionary<string, UniqueUIWindow> UniqueUiWindows = new(); // [PrefabPath : Window Class]
    // Unique window will close all other instances when a new one opens. Overlay windows only themselves
    // You must manage overlay windows yourself
    private Dictionary<string, BaseUIWindow> OverlayWindows = new(); 


    private string _scoreTxt;
    private string _resourceTxt;
    private string _atkTxt;
    private string _multiplierTxt;
    private string _moneyTxt;

    public string PopupTxt;
    public float PopupRemainingTime;

    private FontAsset _defaultFont;
    private UI.ButtonSettings _defaultButtonSettings;
    private UI.TextSettings _defaultTextSettings;

    private UICanvas _mainCanvas;
    public override void Awake()
    {
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
        PopupRemainingTime = time;
        PopupTxt = txt;
    }

    [ClientRpc]
    public static void SetGlobalPopup(string txt, float time)
    {
        UIManager mgr = Instance;
        mgr.PopupTxt = txt;
        mgr.PopupRemainingTime = time;
    }

    [ClientRpc]
    public static void SetPlayerPopup(ulong netId, string txt, float time)
    {
        if (Network.IsClient)
        {
            if (Entity.FindByNetworkId(netId).GetComponent<FightPlayer>().IsLocal)
            {
                UIManager mgr = Instance;
                mgr.PopupTxt = txt;
                mgr.PopupRemainingTime = time;
            }
        }
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

    public void CloseAllUniqueWindow()
    {
        foreach (var window in UniqueUiWindows)
        {
            if (window.Value.IsActive)
            {
                window.Value.CloseWindow();
            }
        }
        return;
    }

    public BaseUIWindow OpenOverlayWindow(string prefabPath)
    {
        if (!OverlayWindows.TryGetValue(prefabPath, out var wd))
        {
            // If not exist, create one
            wd = UIWindow.InstantiateWindow(prefabPath) as BaseUIWindow;
            if (wd == null)
            {
                Log.Error($"Cannot get a UniqueUIWindow component from {prefabPath}!");
                return null;
            }
            
            wd.Entity.SetParent(FindCanvas().Entity,false);
            OverlayWindows.Add(prefabPath, wd);
            wd.OnInstantiate();
        }
        wd.OpenWindow();
        return wd;
    }

    public BaseUIWindow CloseOverlayWindow(string prefabPath = null)
    {
        if (prefabPath == null)
        {
            foreach (var window in OverlayWindows)
            {
                if (window.Value.IsActive)
                {
                    window.Value.CloseWindow();
                }
            }
            return null;
        }
        if (!OverlayWindows.TryGetValue(prefabPath, out var wd))
        {
            Log.Error($"Overlay Window {prefabPath} is not created yet!");
            return null;
        }
        wd.CloseWindow();
        return wd;
    }

    public override void Update()
    {
        // Update timers
        {
            
            if (PopupRemainingTime > 0)
            {
                //Log.Warn(_popupTxt);
                PopupRemainingTime -= Time.DeltaTime;
                // Draw the popup
                {
                    var centerRect = UI.ScreenRect.CenterRect().Grow(235).CutBottom(50);
                    UI.Text(centerRect, $"{PopupTxt}", new UI.TextSettings() {Font = _defaultFont, Size = 24, Color = Vector4.Black, 
                        VerticalAlignment = UI.VerticalAlignment.Center, HorizontalAlignment = UI.HorizontalAlignment.Center,
                        WordWrap = true, Outline = true, OutlineColor = Vector4.White
                    });
                }
            }
            else
            {
                PopupRemainingTime = 0;
                PopupTxt = "";
            }
            
        }
        
        // Admin menus
        if ( Network.LocalPlayer != null && Network.LocalPlayer.IsAdmin)
        {


            // Draw the side buttons
            {

                var sideBarRect = UI.ScreenRect.LeftCenterRect().Grow(330, 100, 330, 0).Offset(5, 0);

                var buttonRect = sideBarRect.CutTop(100);
                if (UI.Button(buttonRect, $"Dash", new UI.ButtonSettings() { Sprite = Assets.GetAsset<Texture>("$AO/new/main_menu/bottom_bar/button.png") }, 
                        _defaultTextSettings).Clicked)
                {
                    var player = (FightPlayer)Network.LocalPlayer;
                    Log.Debug("Dash!");
                    Vector2 dashDir = player.LastInputs.Length < 0.001
                        ? (player.GetFacingDirection() ? Vector2.Right : Vector2.Left)
                         : player.LastInputs;
                    TestServerRPC.CallServer_AddDashToNetworkID(player.Entity.NetworkId, dashDir * 250f, 0.5f);
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

                sideBarRect.CutTop(10);
                
                var buttonRect3 = sideBarRect.CutTop(100);
                if (UI.Button(buttonRect3, $"Boom Effect",
                        new UI.ButtonSettings()
                            { Sprite = Assets.GetAsset<Texture>("$AO/new/main_menu/bottom_bar/button.png") },
                        _defaultTextSettings).Clicked)
                {
                    var player = (FightPlayer)Network.LocalPlayer;
                    Prefab explosion = Assets.GetAsset<Prefab>("SelfDestructExplosion.prefab");
                    Entity expEntity = explosion.Instantiate();
                    expEntity.SetParent(player.Entity, false);
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
                    OpenUniqueUIWindow(UniqueWindowKeys.AbilityVendorPath);
                }
                
                
            }
        }

        
        
    }
}