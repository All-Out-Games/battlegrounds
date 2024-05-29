using AO;
using Assembly.scripts.SkillSlots.Abilities;

public class AbilityPunch : FightAbility
{
    public override Type Effect => typeof(EffectPunch);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override bool CanUse()
    {
        //FightPlayer = (FightPlayer)Player;
        //return !FightPlayer.SkillCastGeneralCheck() && FightPlayer.HasEffect<EffectPunch>();
        return true;
    }

    public override void OnActivate(Player targetPlayer, Vector2 positionOrDirection, float magnitude)
    {
        base.OnActivate(targetPlayer, positionOrDirection, magnitude);
        FightPlayer _player = FightPlayer==null ? (FightPlayer)Player : FightPlayer;
        Log.Debug($"Player is null? {_player.Name}");
        EffectConfig.PunchConfig cfg = EffectConfig.GetPlayerPunchConfig(1, _player.CurrentAttack); 
        
        EffectPunch punch = AppliedEffect as EffectPunch;
        Log.Debug($"Applied Effect is null? {AppliedEffect == null}");
    }
}