using System.Collections;
using AO;
using Assembly.scripts.UI.LevelUp;

namespace Assembly.scripts.UI;

public class ResourceOverlayWindow : BaseUIWindow
{
    private FightPlayer _localPlayer;
    
    [Serialized] private UIText _coinText;
    [Serialized] private UIText _gemText;

    [Serialized] private UIButton _skillBookButton;
    [Serialized] private Entity _sidebar;

    [Serialized] private UIText _levelText;
    [Serialized] private UIText _curExpText;
    [Serialized] private UIText _nextExpText;

    [Serialized] private UIRect _expBarMaskRect;

    [Serialized] private LevelUpWindow _levelUpWindow;

    [Serialized] private Entity _extraXp;

    [Serialized] public AfkInfoWindow AfkInfoWindow;
    [Serialized] public UIText AfkExpText;
    [Serialized] public UIButton AfkInfoBtn;

    [Serialized] public Entity ExpPotionIndicator;
    [Serialized] public UIText ExpBoostTime;

    [Serialized] public Entity Shop;
    [Serialized] public UIButton ShopBtn;

    [Serialized] public UIText SpectralText;

    // Champion update
    [Serialized] public ChampionInfoWindow ChampionInfoWindow;
    [Serialized] public UIButton ChampionButton;
    [Serialized] public UIImage ChampionIndicator;
    [Serialized] public Entity ChampionButtonEntity;

    public static Texture ChampionIcon = Assets.GetAsset<Texture>("UI/Champion.png");
    public static Texture ChampionInactiveIcon = Assets.GetAsset<Texture>("UI/Champion_inactive.png");

    private Coroutine _coroutineC;
    private int _currentLevel;
    private int _currentExp;
    private int _baselineExp;
    private int _nextExp;

    private bool _isFirstLevelup = true;


    public override void OnInstantiate()
    {
        base.OnInstantiate();
        Coroutine.Start(Entity, RefuseToPop());
        // For the first second in the game, we do not pop the level up window
        // because we don't want the player to see the level up screen
        AfkInfoBtn.OnClicked += ShowAfkWindow;
        ShopBtn.OnClicked += OnShopButtonClicked;
        ChampionButton.OnClicked += ShowChampionWindow;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        _localPlayer.CoinUpdateEvent -= UpdateCoin;
        
        _localPlayer.PlayerSwitchZoneEvent -= SetSkillButton;
        _skillBookButton.OnClicked -= OnSkillBtnClicked;
        ShopBtn.OnClicked -= OnShopButtonClicked;
        
    }

    public void HookupEvents(ref Action<int> coinUpdateEvt)
    {
        _localPlayer = (FightPlayer)Network.LocalPlayer;
        coinUpdateEvt += UpdateCoin;
        
        _localPlayer.PlayerSwitchZoneEvent += SetSkillButton;
        _skillBookButton.OnClicked += OnSkillBtnClicked;
        
    }

    public void UpdateCoin(int coin)
    {
        _coinText.Text = coin.ToString();
    }

    public void UpdateGem(int gem)
    {
        _gemText.Text = gem.ToString();
    }
    
    public static IEnumerator TextChangeEmphasize(UIText txt, float expandTime, float shrinkTime, float expandSize,
        float originalSize)
    {
        float timer = 0;
        float size = originalSize;
        float totalTime = shrinkTime + expandTime;
        while (Coroutine.Timer(ref timer, expandTime))
        {
            size = AOMath.Lerp(originalSize, expandSize, timer/expandTime);
            txt.Settings = txt.Settings with { Size = size };
            yield return null;
        }

        while (Coroutine.Timer(ref timer, totalTime))
        {
            size = AOMath.Lerp(expandSize, originalSize, timer/totalTime);
            txt.Settings = txt.Settings with { Size = size };
            yield return null;
        }

        txt.Settings = txt.Settings with { Size = originalSize };
    }

    public IEnumerator RefuseToPop()
    {
        _isFirstLevelup = true;
        yield return new WaitForSeconds(1);
        _isFirstLevelup = false;
    }

    public void SetSkillButton(int status)
    {
        // Log.Error($"STATUS RECEIVED : {status}");
        _sidebar.LocalEnabled = status != (int)PlayerStatus.Combat;
        Shop.LocalEnabled = status != (int)PlayerStatus.Combat;
        if (status == (int)PlayerStatus.AFK)
        {
            AfkInfoBtn.Entity.LocalEnabled = true;
            CalculateAfkExp();
        }
        else
        {
            AfkInfoBtn.Entity.LocalEnabled = false;
        }

    }
    

    public void CalculateAfkExp()
    {
        int exp = AfkInfoWindow.CalculateAfkInfo();
        AfkExpText.Text = $"+{exp}/min";
    }

    public void UpdateExpBoosterTime(int boostMin)
    {
        if (boostMin > 0)
        {
            ExpPotionIndicator.LocalEnabled = true;
            ExpBoostTime.Text = $"{boostMin} min";
        }
        else
        {
            ExpPotionIndicator.LocalEnabled = false;
        }
    }

    public void ShowAfkWindow()
    {
        AfkInfoWindow.OpenWindow();
    }

    public void ShowChampionWindow()
    {
        ChampionInfoWindow.OpenWindow();
        ChampionInfoWindow.SetLocalChampionData();
    }

    public void SetExtraExpActive(bool active)
    {
        _extraXp.LocalEnabled = active;
    }

    private void OnSkillBtnClicked()
    {
        var wd = UIManager.Instance.GetUniqueWindow(UniqueWindowKeys.AbilityLoadoutPagePath);
        if (wd != null)
        {
            if (wd.IsActive)
            {
                wd.CloseWindow();
            }
            else
            {
                UIManager.Instance.OpenUniqueUIWindow(UniqueWindowKeys.AbilityLoadoutPagePath);
            }
        }
        else
        {
            UIManager.Instance.OpenUniqueUIWindow(UniqueWindowKeys.AbilityLoadoutPagePath);
        }
    }
    
    public void OnShopButtonClicked()
    {
        BgShop.Instance.ItemShopOpen = true;
    }

    public void UpdateCurExpTxt(int exp)
    {
        _currentExp = exp;
        _curExpText.Text = $"{exp}";
        ExpBarMaskUpdate();
        
        // Champion stuff
        if (_currentExp >= LevelingData.BaselineXp[LevelingData.MaxLevel]) // Player is on the last level
        {
            ChampionButtonEntity.LocalEnabled = true;
            ChampionIndicator.Entity.LocalEnabled = true;
            if (exp >= LevelingData.NextLevelXp[LevelingData.MaxLevel]) // Player filled level bar
            {
                ChampionButton.Settings = ChampionButton.Settings with { Sprite = ChampionIcon, SpritePressed = ChampionIcon};
                ChampionIndicator.Sprite = ChampionIcon;

            }
            else
            {
                ChampionButton.Settings = ChampionButton.Settings with { Sprite = ChampionInactiveIcon, SpritePressed = ChampionInactiveIcon};
                ChampionIndicator.Sprite = ChampionInactiveIcon;
            }
        }
        else
        {
            ChampionButtonEntity.LocalEnabled = false;
            ChampionIndicator.Entity.LocalEnabled = false;
        }
    }

    public void UpdateLevelingTxt(int level, int nextExp)
    {
        
        _levelText.Text = $"{level+1}";
        _nextExpText.Text = nextExp.ToString();
        if (_coroutineC == null || _coroutineC.Finished)
        {
            _coroutineC = Coroutine.Start(this.Entity, TextChangeEmphasize(_levelText, 0.1f, 0.15f,
                100f, 50f));
        }
        else
        {
            //Scene.CGD.ActiveCoroutines.Remove(_coroutineC);
            _coroutineC = Coroutine.Start(this.Entity, TextChangeEmphasize(_levelText, 0.1f, 0.15f,
                100f, 50f));
        }

        _baselineExp = LevelingData.BaselineXp[level];
        _currentLevel = level;
        _nextExp = nextExp;
        _currentLevel = level;
        ExpBarMaskUpdate();

        Log.Info("Tried pop leveling text");
        if (!_isFirstLevelup)
        {
            // We do not pop for the first event for every client because that would be from their saves
            _levelUpWindow.PopAtLevelUp(_currentLevel);
        }
    }

    public void PopSparkles()
    {
        _levelUpWindow?.PlaySparkles();
    }

    public void PopChampionPromotion()
    {
        _levelUpWindow?.PopChampion();
    }

    private void ExpBarMaskUpdate()
    {
        float totalLevelExp = _nextExp - _baselineExp;
        float currentLevelExp = _currentExp - _baselineExp;
        _expBarMaskRect.Max = _expBarMaskRect.Max with { X = currentLevelExp / totalLevelExp };
    }
}