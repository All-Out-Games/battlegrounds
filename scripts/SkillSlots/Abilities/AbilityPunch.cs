using AO;

namespace Assembly.scripts.SkillSlots.Abilities;

public class AbilityPunch : FightAbility
{
    public override string SkillKey => "Punch";
    public override Type Effect => typeof(EffectPunch);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    
}