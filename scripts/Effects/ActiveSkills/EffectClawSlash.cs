using AO;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityClawSlash : FightAbility
{
    public override string SkillKey => "ClawSlash";
    public override Type Effect => typeof(EffectClawSlash);
    public override bool MonitorEffectDuration => false;

    public override float MaxDistance => EffectConfig.ClawSlashConfig.SlashRadius;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float Cooldown => EffectConfig.ClawSlashConfig.Cooldown;
}

public class EffectClawSlash : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;

    private bool _slash = false;
    private EffectConfig.ClawSlashConfig _config;

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        AssignConfig(EffectConfig.ClawSlashConfig.GetDefault(FightPlayer.CurrentAttack));
        FightPlayer.SetAnimTrigger("punch"); 
    }

    public override void OnEffectUpdate()
    {
        if (Util.OneTime(ElapsedTime > EffectConfig.ClawSlashConfig.SlashActivationTime, ref _slash))
        {
            Slash();
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        
    }

    public void AssignConfig(EffectConfig.ClawSlashConfig cfg)
    {
        _config = cfg;
        DurationRemaining = EffectConfig.ClawSlashConfig.SlashAnimationTime;
    }

    public void Slash()
    {
        Vector2 selfPos = FightPlayer.Entity.Position + AbilityPositionOrDirection * EffectConfig.ClawSlashConfig.SlashRadius;

        var cbPlayers = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(selfPos, EffectConfig.ClawSlashConfig.SlashRadius);
        foreach (var other in cbPlayers)
        {
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.SlashDamage);
            if(other.Entity.NetworkId == FightPlayer.Entity.NetworkId) continue;
                
            other.TakeDamage(FightPlayer, info);
            other.GetEffectMgr().AddBleed(FightPlayer.Entity, _config.BleedTime, _config.BleedDmg);
        }
    }
}