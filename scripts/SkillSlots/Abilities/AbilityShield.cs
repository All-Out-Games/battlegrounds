namespace Assembly.scripts.SkillSlots.Abilities;
using AO;

public class AbilityShield : FightAbility
{
    public override string SkillKey => "Shield";
    public override string SkillIconPath => "ability_icon_tmp/Shield_Tmp.png";

    public override Type Effect => typeof(EffectShield);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;
}