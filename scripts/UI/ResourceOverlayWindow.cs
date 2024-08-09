using System.Collections;
using AO;
using Assembly.scripts.UI.LevelUp;

namespace Assembly.scripts.UI;

public class ResourceOverlayWindow : BaseUIWindow
{
    private FightPlayer _localPlayer;
    
    [Serialized] private UIText _coinText;

    [Serialized] private UIButton _skillBookButton;
    [Serialized] private Entity _sidebar;

    [Serialized] private UIText _levelText;
    [Serialized] private UIText _curExpText;
    [Serialized] private UIText _nextExpText;

    [Serialized] private UIRect _expBarMaskRect;

    [Serialized] private LevelUpWindow _levelUpWindow;
    
    private Coroutine _coroutineC;
    private int _currentLevel;
    private int _currentExp;
    private int _baselineExp;
    private int _nextExp;

    private bool _isFirstLevelup = true; 

    public override void OnDestroy()
    {
        base.OnDestroy();
        _localPlayer.CoinUpdateEvent -= UpdateCoin;
        
        _localPlayer.PlayerSwitchZoneEvent -= SetSkillButton;
        _skillBookButton.OnClicked -= OnSkillBtnClicked;
        
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

    public void SetSkillButton(int status)
    {
        // Log.Error($"STATUS RECEIVED : {status}");
        _sidebar.LocalEnabled = status != (int)PlayerStatus.Combat;
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

    public void UpdateCurExpTxt(int exp)
    {
        _currentExp = exp;
        _curExpText.Text = $"{exp}";
        ExpBarMaskUpdate();
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
            Coroutine.ActiveCoroutines.Remove(_coroutineC);
            _coroutineC = Coroutine.Start(this.Entity, TextChangeEmphasize(_levelText, 0.1f, 0.15f,
                100f, 50f));
        }

        _baselineExp = LevelingData.BaselineXp[level];
        _currentLevel = level;
        _nextExp = nextExp;
        _currentLevel = level;
        ExpBarMaskUpdate();

        if (!_isFirstLevelup)
        {
            // We do not pop for the first event for every client because that would be from their saves
            _levelUpWindow.PopAtLevelUp(_currentLevel);
        }

        _isFirstLevelup = false;
    }

    private void ExpBarMaskUpdate()
    {
        float totalLevelExp = _nextExp - _baselineExp;
        float currentLevelExp = _currentExp - _baselineExp;
        _expBarMaskRect.Max = _expBarMaskRect.Max with { X = currentLevelExp / totalLevelExp };
    }
}