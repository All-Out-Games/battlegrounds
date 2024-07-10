using AO;
using Assembly.scripts.VFX;
using Assembly.scripts.Zones;

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
    
    private EffectConfig.ClawSlashConfig _config;

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        AssignConfig(EffectConfig.ClawSlashConfig.GetDefault(FightPlayer.CurrentAttack));

        FightPlayer.SetAnimTrigger("clawslash");
        FightPlayer.SetMouseIKPosition(AbilityPositionOrDirection);
        DurationRemaining = MainLayer.GetCurrentStateLength();
        Log.Debug($"Current Animation = {DurationRemaining}, Current State = {MainLayer.CurrentState.Name}");
        FightPlayer.SpineAnimator.OnEvent += OnAnimationEvent;
    }
    

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.SpineAnimator.OnEvent -= OnAnimationEvent;
    }

    public override void OnAnimationEvent(string eventName)
    {
        base.OnAnimationEvent(eventName);
        Log.Warn($"{eventName} Triggered");
        if (eventName == "Attack")
        {
            Slash();
        }
    }


    public void AssignConfig(EffectConfig.ClawSlashConfig cfg)
    {
        _config = cfg;
        DurationRemaining = EffectConfig.ClawSlashConfig.SlashAnimationTime;
    }

    public void Slash()
    {
        Vector2 selfPos = FightPlayer.Entity.Position + AbilityPositionOrDirection * EffectConfig.ClawSlashConfig.SlashRadius;
        FightClubGameManager.Instance.ClientSpawn(VFXPrefabKeys.ClawSlashVFXPath, selfPos, entity =>
        {
            entity.LocalRotation = FightClubUtils.AngleBetween(Vector2.Left, AbilityPositionOrDirection);
        });

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