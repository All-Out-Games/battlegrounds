namespace Assembly.scripts.SkillSlots.Abilities;
using AO;

public class AbilityShoulderCrash : FightAbility
{
    public override string SkillKey => "ShoulderCrash";
    public override string SkillIconPath => "ability_icon_tmp/ShoulderCrash_Tmp.png";

    public override Type Effect => typeof(EffectShoulderCrash);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => 5f;
    
    public override float Cooldown => 7.0f;
}