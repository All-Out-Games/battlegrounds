using AO;

namespace Assembly.scripts.Effects;

public class EffectDodge : FightEffect
{
    public override bool IsActiveEffect => false;
    public float DodgeChance = 0;
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
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
        if (Network.IsServer)
        {
            if (Random.Shared.NextSingle() < DodgeChance)
            {
                info.ReactionInfo.Flinch = false;
                info.OverrideDamageNumber = FightPlayer.DamageInfo.DamageNumberOverrideType.Dodged;
                info.ReactionInfo.Amount = 0;
            }
        }
        
    }
}