namespace Assembly.scripts.Effects;

public class EffectOvershield : FightEffect
{
    protected EffectConfig.ShieldConfig Config;
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => false;
    public override bool IsValidTarget => true;
    

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        
        FightPlayer.MaxShield += Config.ShieldAmt;
        FightPlayer.CurrentShield += Config.ShieldAmt;
        FightPlayer.OnReceiveDamage += OnDamageEvent;
    }

    public void AssignConfig(EffectConfig.ShieldConfig cfg)
    {
        Config = cfg;
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);

        FightPlayer.CurrentShield -= Config.ShieldAmt;
        FightPlayer.MaxShield -= Config.ShieldAmt;
        FightPlayer.OnReceiveDamage -= OnDamageEvent;
    }
    
    
    protected override void OnDamageEvent(FightPlayer source, FightPlayer.DamageInfo info)
    {
        if (info.ReactionInfo.ShieldBroken)
        {
            FightPlayer.RemoveEffect<EffectOvershield>(true);
        }
    }
}