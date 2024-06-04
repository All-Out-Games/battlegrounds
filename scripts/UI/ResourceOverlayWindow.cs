using AO;

namespace Assembly.scripts.UI;

public class ResourceOverlayWindow : BaseUIWindow
{
    private FightPlayer _localPlayer;
    
    [Serialized] private UIText _coinText;
    [Serialized] private UIText _damageText;
    [Serialized] private UIText _eliminationText;

    private Action<int> c, d, k;

    public override void OnDestroy()
    {
        base.OnDestroy();
        c -= UpdateCoin;
        d -= UpdateDamage;
        k -= UpdateElimination;
    }

    public void HookupEvents(ref Action<int> coinUpdateEvt, ref Action<int> dmgUpdateEvt, ref Action<int> killUpdateEvt)
    {
        c = coinUpdateEvt;
        d = dmgUpdateEvt;
        k = killUpdateEvt;
        
        coinUpdateEvt += UpdateCoin;
        dmgUpdateEvt += UpdateDamage;
        killUpdateEvt += UpdateElimination;
    }

    public void UpdateCoin(int coin)
    {
        _coinText.Text = coin.ToString();
    }

    public void UpdateDamage(int dmg)
    {
        _damageText.Text = dmg.ToString();
    }

    public void UpdateElimination(int kills)
    {
        _eliminationText.Text = kills.ToString();
    }
}