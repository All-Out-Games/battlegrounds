using Assembly.scripts.Effects.ActiveSkills;
using AO;

namespace Assembly.scripts.SkillSlots.Abilities;

public class AbilityBattleCry : FightAbility
{
    public override string SkillKey => "BattleCry";
    public override Type Effect => typeof(EffectBattleCry);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    public override float Cooldown => EffectConfig.BattleCryConfig.Cooldown;
}