using AO;
using StreamReader = AO.StreamReader;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityHypnotize : FightAbility
{
    public override string SkillKey => "Hypnotize";

    public override Type Effect => typeof(EffectHypnotize);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Select;
    public override float MaxDistance => EffectConfig.HypnotizeConfig.HypnotizeRange;
    public override int MaxTargets => 1;
    
    public override float Cooldown => EffectConfig.HypnotizeConfig.Cooldown;

    public override bool CanTarget(Player player)
    {
        return GenericCanTarget(player as FightPlayer) && !player.HasEffect<EffectHypnotize>();
    }
}


public class EffectHypnotize : EffectStun
{
    public override bool IsActiveEffect => false;
    protected override int InterruptLevel => 1000;
    public override bool BlockAbilityActivation => true;

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        FightPlayer.OnReceiveDamage += OnDamageEvent;
        DurationRemaining = EffectConfig.HypnotizeConfig.HypnotizeTime;
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.OnReceiveDamage -= OnDamageEvent;
    }

    public override void NetworkDeserialize(StreamReader reader)
    {
        base.NetworkDeserialize(reader);
        FightPlayer.OnReceiveDamage += OnDamageEvent;
    }
}