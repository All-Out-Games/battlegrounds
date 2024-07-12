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

public class EffectHypnotizeCaster : FightEffect
{
    public override bool IsActiveEffect => true;

    public override bool BlockAbilityActivation => true;

    protected override bool PreventMovement => true;

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        FightPlayer.SetAnimTrigger("hypnotize");
        DurationRemaining = MainLayer.GetCurrentStateLength();
    }
}

public class EffectHypnotize : FightEffectWithNoFlinch
{
    public override bool IsActiveEffect => true;
    protected override int InterruptLevel => 1000;

    protected override bool PreventMovement => true;

    public override bool BlockAbilityActivation => true;

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        FightPlayer.SetAnimTrigger("knockdown");
        FightPlayer.OnReceiveDamage += OnDamageEvent;
        DurationRemaining = EffectConfig.HypnotizeConfig.HypnotizeTime;

        Caster.AddEffect<EffectHypnotizeCaster>();
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.OnReceiveDamage -= OnDamageEvent;
        FightPlayer.SetAnimTrigger("knockdown_end");
        FightPlayer.AddEffect<EffectGenericPostActionDelay>();
    }

    public override void NetworkDeserialize(StreamReader reader)
    {
        base.NetworkDeserialize(reader);
        FightPlayer.OnReceiveDamage += OnDamageEvent;
    }
}
