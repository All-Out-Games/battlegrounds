namespace Assembly.scripts;
using AO;
public class UIManager : System<UIManager>
{
    // Update using event when you need to. This should avoid fetching references each frame which causes a slight overhead
    public Action<FightPlayer> UpdateUIEvent;
    public Action<string, float, Player> PopupEvent;

    private string _scoreTxt;
    private string _resourceTxt;
    private string _atkTxt;
    private string _multiplierTxt;
    private string _moneyTxt;

    private string _popupTxt;
    private float _popupRemainingTime;

    private FontAsset _defaultFont;
    
    public override void Awake()
    {
        PopupEvent = SetPopup;
        UpdateUIEvent = UpdateUI;
        _defaultFont = Assets.GetAsset<FontAsset>("$AO/fonts/Barlow-SemiBold.ttf");
    }
    
    public override void Start()
    {
        _scoreTxt = "0";
        _resourceTxt = "0";
        _moneyTxt = "0";

        UpdateUI((FightPlayer)Network.LocalPlayer);
    }
    

    public void UpdateUI(FightPlayer player)
    {
        if (player == null || Network.LocalPlayer == null)
        {
            return;
        }

        if (!player.IsLocal)
        {
            return;
        }
        //_scoreTxt = player.Score.ToString();
        //_resourceTxt = player.Resource.ToString();
        //_atkTxt = player.Atk.ToString();
        //_multiplierTxt = player.Multiplier.ToString();
        //_moneyTxt = player.Money.ToString();
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
                    UI.Text(centerRect, $"{_popupTxt}", new UI.TextSettings() {Size = 24, Color = Vector4.Black, 
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
        
        /*// Draw the score
        {
            var topBarRect = UI.ScreenRect.CutTop(0.05f);
            var currencyRect = topBarRect.CutLeft(0.088f).Offset(0.215f, -0.007f);
            UI.Image(currencyRect, Assets.GetAsset<Texture>("$AO/new/main_menu/bottom_bar/button.png"), Vector4.White);
            UI.Text(currencyRect, $"Score: {_scoreTxt}", new UI.TextSettings() {Size = 40, Color = Vector4.LightGreen, VerticalAlignment = UI.VerticalAlignment.Center, HorizontalAlignment = UI.HorizontalAlignment.Center });
            // Draw resources
            var resourceRect = topBarRect.CutLeft(0.088f).Offset(0.215f, -0.007f);
            UI.Image(resourceRect, Assets.GetAsset<Texture>("$AO/new/main_menu/bottom_bar/button.png"), Vector4.White);
            UI.Text(resourceRect, $"Material: {_resourceTxt}", new UI.TextSettings() {Size = 40, Color = Vector4.Green, VerticalAlignment = UI.VerticalAlignment.Center, HorizontalAlignment = UI.HorizontalAlignment.Center });
            // Draw money
            var moneyRect = topBarRect.CutLeft(0.088f).Offset(0.215f, -0.007f);
            UI.Image(moneyRect, Assets.GetAsset<Texture>("$AO/new/main_menu/bottom_bar/button.png"), Vector4.White);
            UI.Text(moneyRect, $"Money: {_moneyTxt}", new UI.TextSettings() { Size = 40, Color = Vector4.LightGreen, VerticalAlignment = UI.VerticalAlignment.Center, HorizontalAlignment = UI.HorizontalAlignment.Center });
        }*/


        // Draw the side buttons
        {
            //Log.Error("???");

            var sideBarRect = UI.ScreenRect.LeftCenterRect().Grow(110, 100, 110, 0).Offset(5, 0);

            var buttonRect = sideBarRect.CutTop(100);
            if (UI.Button(buttonRect, $"+Atk: {_atkTxt}", new UI.ButtonSettings() { Sprite = Assets.GetAsset<Texture>("$AO/new/main_menu/bottom_bar/button.png") }, new UI.TextSettings() { Size = 24, Color = Vector4.LightBlue }).clicked)
            {
                var player = (FightPlayer)Network.LocalPlayer;
                Log.Info("I'm upgrading a stat!");
                // player.CallServer_UpgradeAtk();
            }

            // Spacing
            sideBarRect.CutTop(10);

            var buttonRect2 = sideBarRect.CutTop(100);
            if (UI.Button(buttonRect2, $"+Mtp: {_multiplierTxt}", new UI.ButtonSettings() { Sprite = Assets.GetAsset<Texture>("$AO/new/main_menu/bottom_bar/button.png") }, new UI.TextSettings() {Size = 24, Color = Vector4.LightBlue }).clicked)
            {
                var player = (FightPlayer)Network.LocalPlayer;
                Log.Info("I'm upgrading another stat!");
                // player.CallServer_UpgradeMtp();
            }
            
        }
        
        
    }
}