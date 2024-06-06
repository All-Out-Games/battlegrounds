using AO;
using Assembly.scripts.SkillSlots.Abilities;

public class AbilityPunch : FightAbility
{
    public override string SkillKey => "Punch";
    public override Type Effect => typeof(EffectPunch);
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