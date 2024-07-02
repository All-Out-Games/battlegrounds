namespace Assembly.scripts.Effects;

public class FightEffectWithImmunity : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool IsValidTarget => false;
    protected override bool PreventDamage => true;
    
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        FightPlayer.RegisterPreDamageEvent(this);
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.RemovePreDamageEvent(this);
    }

    public override void PreDamageMod(ref FightPlayer.DamageInfo info)
    {
        base.PreDamageMod(ref info);
        info.ReactionInfo.Amount = 0;
        info.ReactionInfo.Flinch = false;
        info.AwardCoin = false;
    }
}