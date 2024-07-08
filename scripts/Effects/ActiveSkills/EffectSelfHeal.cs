using AO;
using StreamReader = AO.StreamReader;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilitySelfHeal : FightAbility
{
    public override string SkillKey => "SelfHeal";

    public override Type Effect => typeof(EffectSelfHeal);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    
    public override float Cooldown => EffectConfig.SelfHealConfig.Cooldown;
}



public class EffectSelfHeal : FightEffect
{
    public override bool IsActiveEffect => false;
    protected override int InterruptLevel => 1000;
    public override bool BlockAbilityActivation => true;

    protected override bool PreventMovement => true;

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        FightPlayer.OnReceiveDamage += OnDamageEvent;
        DurationRemaining = EffectConfig.SelfHealConfig.ChannelTime;
        FightPlayer.UnsetAnimTrigger("selfheal_end");
        FightPlayer.SetAnimTrigger("selfheal");
    }

    public override void NetworkDeserialize(StreamReader reader)
    {
        base.NetworkDeserialize(reader);
        FightPlayer.OnReceiveDamage += OnDamageEvent;
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.OnReceiveDamage -= OnDamageEvent;
        if (!interrupt)
        {
            FightPlayer.DamageInfo healInfo = FightPlayer.DamageInfo.CreateHealInfo(EffectConfig.SelfHealConfig.HealAmtBase);
            FightPlayer.TakeDamage(FightPlayer, healInfo);
            FightPlayer.SetAnimTrigger("selfheal_end");
            FightPlayer.AddEffect<EffectGenericPostActionDelay>();
        }
    }
}