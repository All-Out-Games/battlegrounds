using AO;

namespace Assembly.Koh;

public partial class EffectKing : FightEffect
{
    public override bool IsActiveEffect => false;
    public override float SpeedModifier => 0.8f;
    
    
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

    [ClientRpc]
    public static void GrantKing(FightPlayer fp)
    {
        if (fp.HasEffect<EffectKing>())
        {
            return;
        }
        var players = Scene.Components<FightPlayer>().ToList();
        foreach (var p in players)
        {
            p.RemoveEffect<EffectKing>(true);
        }

        fp.AddEffect<EffectKing>();
    }
}