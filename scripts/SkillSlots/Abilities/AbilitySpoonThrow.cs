namespace Assembly.scripts.SkillSlots.Abilities;
using AO;

public class AbilitySpoonThrow : FightAbility
{
    public override string SkillKey => "SpoonThrow";

    public override Type Effect => typeof(EffectProjectileThrow);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => EffectConfig.ProjectileConfig.SpoonRange;
    public override int MaxTargets => 1;
    
    public override float Cooldown => EffectConfig.ProjectileConfig.SpoonThrowCooldown;
}