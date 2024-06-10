namespace Assembly.scripts.SkillSlots.Abilities;
using AO;

public class AbilityShoulderCrash : FightAbility
{
    public override string SkillKey => "ShoulderCrash";

    public override Type Effect => typeof(EffectShoulderCrash);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => 5f;
    
    public override float Cooldown => EffectConfig.ShoulderCrashConfig.Cooldown;
}