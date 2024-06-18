using AO;
using StreamReader = AO.StreamReader;

public class AbilityShield : FightAbility
{
    public override string SkillKey => "Shield";

    public override Type Effect => typeof(EffectShield);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    
    public override float Cooldown => EffectConfig.ShieldConfig.Cooldown;
}
/// <summary>
/// Base class of a shield ability.
/// Shield abilities will overwrite each other. Only one of them may exist on a player.
/// </summary>
public class EffectShield : FightEffect
{
    protected EffectConfig.ShieldConfig Config;
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => false;
    public override bool IsValidTarget => true;

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        
        AssignConfig(EffectConfig.ShieldConfig.GetDefault());
        DurationRemaining = Config.Duration;
        
        FightPlayer.MaxShield = Config.ShieldAmt;
        FightPlayer.CurrentShield = Config.ShieldAmt;
        FightPlayer.OnReceiveDamage += OnDamageReaction;
    }

    public void AssignConfig(EffectConfig.ShieldConfig cfg)
    {
        Config = cfg;
    }

    public override void OnEffectEnd(bool interrupt)
    {
        if (interrupt)
        {
            Log.Warn("Shield Premature Removal!");
        }
        FightPlayer.CurrentShield = 0;
        FightPlayer.MaxShield = 0;
        FightPlayer.OnReceiveDamage -= OnDamageReaction;
    }

    public override void NetworkDeserialize(StreamReader reader)
    {
        base.NetworkDeserialize(reader);
        FightPlayer.OnReceiveDamage += OnDamageReaction;
    }

    protected void OnDamageReaction(FightPlayer source, FightPlayer.DamageInfo info)
    {
        if (info.ReactionInfo.ShieldBroken)
        {
            FightPlayer.RemoveEffect<EffectShield>(true);
        }
    }

}