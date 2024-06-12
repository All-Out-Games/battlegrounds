using Assembly.scripts.Effects.ActiveSkills;
using AO;

namespace Assembly.scripts.SkillSlots.Abilities;

public class AbilitySelfDestruct : FightAbility
{
    public override string SkillKey => "SelfDestruct";
    public override Type Effect => typeof(EffectSelfDestruct);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;
}