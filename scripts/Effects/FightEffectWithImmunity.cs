using AO;

namespace Assembly.scripts.Effects;

public class FightEffectWithImmunity : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool IsValidTarget => false;
    protected override bool PreventDamage => true;

    protected virtual string InvincibilityReason => "Immunity";
    
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.RegisterPreDamageEvent(this);
        FightPlayer.AddInvincibilityReason(InvincibilityReason);

        if (InvincibilityReason == "Immunity")
        {
            Log.Warn("Warning: A Effect with Immunity doesn't override the InvincibilityReason field. You need a unique string for it!");
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.RemovePreDamageEvent(this);
        FightPlayer.RemoveInvincibilityReason(InvincibilityReason);
    }

    public override void PreDamageMod(ref FightPlayer.DamageInfo info)
    {
        base.PreDamageMod(ref info);
        if(info.ReactionInfo.Amount > 0) info.ReactionInfo.Amount = 0; // Does not affect healing
        info.ReactionInfo.Flinch = false;
        info.AwardCoin = false;
        info.OverrideDamageNumber = FightPlayer.DamageInfo.DamageNumberOverrideType.Immune;
    }
}