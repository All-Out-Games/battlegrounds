using AO;
using Assembly.scripts.Effects.ActiveSkills;

namespace Assembly.scripts.SkillSlots.Abilities;

public class AbilityDoublePunch : FightAbility
{
    public override string SkillKey => "DoublePunch";
    public override Type Effect => typeof(EffectDoublePunch);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    
}