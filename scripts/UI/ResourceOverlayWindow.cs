using System.Collections;
using AO;

namespace Assembly.scripts.UI;

public class ResourceOverlayWindow : BaseUIWindow
{
    private FightPlayer _localPlayer;
    
    [Serialized] private UIText _coinText;
    [Serialized] private UIText _damageText;
    [Serialized] private UIText _eliminationText;

    [Serialized] private UIButton _skillBookButton;
    [Serialized] private Entity _sidebar;
    
    private Coroutine _coroutineC;

    public override void OnDestroy()
    {
        base.OnDestroy();
        _localPlayer.CoinUpdateEvent -= UpdateCoin;
        _localPlayer.TotalDamageUpdateEvent -= UpdateDamage;
        _localPlayer.TotalElminationUpdateEvent -= UpdateElimination;
        _localPlayer.PlayerSwitchZoneEvent -= SetSkillButton;
        _skillBookButton.OnClicked -= OnSkillBtnClicked;
        
    }

    public void HookupEvents(ref Action<int> coinUpdateEvt, ref Action<int> dmgUpdateEvt, ref Action<int> killUpdateEvt)
    {
        _localPlayer = (FightPlayer)Network.LocalPlayer;
        coinUpdateEvt += UpdateCoin;
        dmgUpdateEvt += UpdateDamage;
        killUpdateEvt += UpdateElimination;
        _localPlayer.PlayerSwitchZoneEvent += SetSkillButton;
        _skillBookButton.OnClicked += OnSkillBtnClicked;
    }

    public void UpdateCoin(int coin)
    {
        _coinText.Text = coin.ToString();
        if (_coroutineC == null || _coroutineC.Finished)
        {
            _coroutineC = Coroutine.Start(this.Entity, TextChangeEmphasize(_coinText, 0.1f, 0.15f,
                64f, 32f));
        }
        else
        {
            Coroutine.ActiveCoroutines.Remove(_coroutineC);
            _coroutineC = Coroutine.Start(this.Entity, TextChangeEmphasize(_coinText, 0.1f, 0.15f,
                64f, 32f));
        }
    }

    public void UpdateDamage(int dmg)
    {
        _damageText.Text = dmg.ToString();
    }

    public void UpdateElimination(int kills)
    {
        _eliminationText.Text = kills.ToString();
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
}