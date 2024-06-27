namespace Assembly.scripts.Effects;

public class EffectNoMovementWithInvincibility : EffectNoMovement
{
    public override bool IsValidTarget => false;

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