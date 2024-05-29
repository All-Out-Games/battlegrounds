namespace Assembly.scripts.SkillSlots.Abilities;
using AO;

public class AbilityShoulderCrash : FightAbility
{

    public override Type Effect => typeof(EffectShoulderCrash);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => 5f;
}