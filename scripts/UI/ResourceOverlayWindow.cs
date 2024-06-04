using System.Collections;
using AO;

namespace Assembly.scripts.UI;

public class ResourceOverlayWindow : BaseUIWindow
{
    private FightPlayer _localPlayer;
    
    [Serialized] private UIText _coinText;
    [Serialized] private UIText _damageText;
    [Serialized] private UIText _eliminationText;

    private Action<int> c, d, k;
    private Coroutine CoroutineC, CoroutineD, CoroutineK;

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
        if (CoroutineC == null || CoroutineC.Finished)
        {
            CoroutineC = Coroutine.Start(this.Entity, TextChangeEmphasize(_coinText, 0.1f, 0.15f,
                64f, 32f));
        }
        else
        {
            Coroutine.ActiveCoroutines.Remove(CoroutineC);
            CoroutineC = Coroutine.Start(this.Entity, TextChangeEmphasize(_coinText, 0.1f, 0.15f,
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

    protected IEnumerator TextChangeEmphasize(UIText txt, float expandTime, float shrinkTime, float expandSize,
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
}