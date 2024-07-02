namespace Assembly.scripts.Effects;

public class FightEffectWithNoFlinch : FightEffect
{
    public override bool IsActiveEffect => false;

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
        info.ReactionInfo.Flinch = false;
    }
}