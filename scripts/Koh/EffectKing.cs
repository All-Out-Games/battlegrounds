using AO;

namespace Assembly.Koh;

public partial class EffectKing : FightEffect
{
    public override bool IsActiveEffect => false;
    public override float SpeedModifier => 2.8f; // TODO: Change to 0.8 before publish!
    
    
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.RegisterSpeedModify(this);
        FightPlayer.CurrentAttack += 5;
        FightPlayer.MaxHealth += 20;
        FightPlayer.DamageInfo healInfo = FightPlayer.DamageInfo.CreateHealInfo(20);

        FightPlayer.TakeDamage(FightPlayer, healInfo);
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.RemoveSpeedModify(this);
        FightPlayer.CurrentAttack -= 5;
        FightPlayer.MaxHealth -= 20;
        if (FightPlayer.CurrentHealth > FightPlayer.MaxHealth)
        {
            FightPlayer.CurrentHealth = FightPlayer.MaxHealth;
        }
    }

    private static EffectKing _kingInstance;

    public static EffectKing KingInstance
    {
        get
        {
            if (_kingInstance.Alive())
            {
                return _kingInstance;
            }
            var players = Scene.Components<FightPlayer>().ToList();
            var king = players.Find(fp => fp.HasEffect<EffectKing>());
            return king.Alive() ? king.GetEffect<EffectKing>() : null;
        }
        set => _kingInstance = value;
    }

    public FightPlayer GetKing()
    {
        return FightPlayer;
    }
    
    [ClientRpc]
    public static void GrantKing(FightPlayer fp)
    {
        if (fp.Alive() && fp.HasEffect<EffectKing>() && fp == KingInstance?.Player)
        {
            return;
        }
        var players = Scene.Components<FightPlayer>().ToList();
        foreach (var p in players)
        {
            if (p.Alive())
            {
                if (p != fp)
                {
                    p.RemoveEffect<EffectKing>(true);
                }
                else
                {
                    if (!p.HasEffect<EffectKing>())
                    {
                        KingInstance = p.AddEffect<EffectKing>();
                    }
                }
                
            }
        }
    }
}