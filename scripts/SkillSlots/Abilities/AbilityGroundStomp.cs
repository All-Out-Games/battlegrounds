using AO;
using Assembly.scripts.Effects.ActiveSkills;

namespace Assembly.scripts.SkillSlots.Abilities;

public class AbilityGroundStomp : FightAbility
{
    public override string SkillKey => "GroundStomp";
    public override Type Effect => typeof(EffectGroundStomp);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    public override float Cooldown => EffectConfig.GroundStompConfig.Cooldown;
}