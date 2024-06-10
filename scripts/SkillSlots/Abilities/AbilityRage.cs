using AO;
using Assembly.scripts.Effects.ActiveSkills;

namespace Assembly.scripts.SkillSlots.Abilities;

public class AbilityRage : FightAbility
{
    public override string SkillKey => "Rage";
    
    public override Type Effect => typeof(EffectRage);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => EffectConfig.RageConfig.Cooldown;
}