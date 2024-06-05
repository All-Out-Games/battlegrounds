namespace Assembly.scripts.SkillSlots.Abilities;
using AO;

public class AbilitySpoonThrow : FightAbility
{
    public override string SkillKey => "SpoonThrow";
    public override string SkillIconPath => "ability_icon_tmp/SpoonThrow_Tmp.png";

    public override Type Effect => typeof(EffectProjectileThrow);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => 15f;
    public override int MaxTargets => 1;
    
    public override float Cooldown => 3.0f;
}