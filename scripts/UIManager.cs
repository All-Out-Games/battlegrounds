using System.Collections;
using AO;
using Assembly.scripts;
using Assembly.scripts.UI;

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

    private float _timerNextGlobalUIUpdate;
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
    public static void SetStatusPopup(string txt, float time, PlayerStatus status)
    {
        // Broadcast to player with specific status
        var fp = Network.LocalPlayer as FightPlayer;
        if (fp?.PlayerStatus == status)
        {
            SetGlobalPopup(txt, time);
        }
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

    public T GetOverlayWindow<T>(string prefabPath = null) where T : BaseUIWindow
    {
        if (!OverlayWindows.TryGetValue(prefabPath, out var wd))
        {
            Log.Error($"Overlay Window {prefabPath} NOT FOUND");
            return null;
        }
        else
        {
            return wd as T;
        }
    }

    [ClientRpc]
    public static void SetExpBoostText(bool active)
    {
        if (Network.IsClient)
        {
            var overlay = Instance.GetOverlayWindow<ResourceOverlayWindow>("ResourcesOverlayWindow.prefab");
            if (overlay != null)
            {
                overlay.SetExtraExpActive(active);
            }
        }
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
                    UI.Text(centerRect, $"{PopupTxt}", new UI.TextSettings() {Font = _defaultFont, Size = 40, Color = Vector4.Black, 
                        VerticalAlignment = UI.VerticalAlignment.Center, HorizontalAlignment = UI.HorizontalAlignment.Center,
                        WordWrap = true, Outline = true, OutlineColor = Vector4.White, OutlineThickness = 1f
                    });
                }
            }
            else
            {
                PopupRemainingTime = 0;
                PopupTxt = "";
            }
            
        }

        // Global UI Update
        {
            // This should be the ONLY IsServer in this file!
            if (Network.IsServer)
            {
                if (_timerNextGlobalUIUpdate < 60)
                {
                    _timerNextGlobalUIUpdate = 0;

                    CallClient_SetExpBoostText(LevelingData.DoubleXP(DateTime.Now));
                }

                _timerNextGlobalUIUpdate += Time.DeltaTime;
            }
            
        }
        
    }
}