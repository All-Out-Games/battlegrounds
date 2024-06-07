using AO;
using Assembly.scripts.Effects.ActiveSkills;

namespace Assembly.scripts.SkillSlots.Abilities;

public class AbilityGroundStomp : FightAbility
{
    public override string SkillKey => "GroundStomp";
    public override Type Effect => typeof(EffectGroundStomp);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    
    public override bool CanUse()
    {
        FightPlayer = (FightPlayer)Player;
        return FightPlayer.SkillCastGeneralCheck();
    }

    public override void OnActivate(Player targetPlayer, Vector2 positionOrDirection, float magnitude)
    {
        base.OnActivate(targetPlayer, positionOrDirection, magnitude);
    }
}