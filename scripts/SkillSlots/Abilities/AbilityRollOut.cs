using AO;
namespace Assembly.scripts.SkillSlots.Abilities;

public class AbilityRollOut : FightAbility
{
    public override string SkillKey => "RollOut";
    public override string SkillIconPath => "ability_icon_tmp/RollOut_Tmp.png";
    
    public override Type Effect => typeof(EffectRollOut);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => 8.0f;
}