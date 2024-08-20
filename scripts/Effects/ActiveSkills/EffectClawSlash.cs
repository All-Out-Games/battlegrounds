using AO;
using Assembly.scripts.SceneObjects;
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
    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        return fp.GetSkillTree().GetSkillLevel("ClawSlash") > 2
            ? EffectConfig.ClawSlashConfig.Cooldown - 1
            : EffectConfig.ClawSlashConfig.Cooldown;
    }
    
}

public class EffectDualClaw : FightEffect
{
    public override bool IsActiveEffect => false;
    private FightAbility _slash;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        
        _slash = FightPlayer.GetSkillSlots().GetAbilityInstance(typeof(AbilityClawSlash));
        _slash.CooldownRemaining = 0;
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        if (!interrupt)
        {
            _slash.CooldownRemaining = EffectConfig.ClawSlashConfig.Cooldown;
        }
    }
}

public class EffectClawSlash : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    
    private EffectConfig.ClawSlashConfig _config;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        AssignConfig(EffectConfig.ClawSlashConfig.GetDefault(FightPlayer.CurrentAttack, FightPlayer.GetSkillTree().GetSkillLevel("ClawSlash")));

        FightPlayer.SetAnimTrigger("clawslash");
        FightPlayer.SetAimTarget(AbilityPositionOrDirection + FightPlayer.Entity.Position);
        DurationRemaining = MainLayer.GetCurrentStateLength();
        Log.Debug($"Current Animation = {DurationRemaining}, Current State = {MainLayer.CurrentState.Name}");
        FightPlayer.SpineAnimator.OnEvent += OnAnimationEvent;

        SoundId = SFX.Play(SFXKeys.ClawSlashAudio, DefaultSoundDesc);
    }
    

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.SpineAnimator.OnEvent -= OnAnimationEvent;

        if (FightPlayer.HasSkill("DualClaw"))
        {
            if (!FightPlayer.HasEffect<EffectDualClaw>())
            {
                FightPlayer.AddEffect<EffectDualClaw>(FightPlayer, EffectConfig.ClawSlashConfig.DualClawTime);
            }
            else
            {
                FightPlayer.RemoveEffect<EffectDualClaw>(true);
            }
        }
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
            if(other == FightPlayer || !other.Damageable()) continue;
            info.SkillKey = SkillConfig.ClawSlashConfig.SkillKey;
            other.TakeDamage(FightPlayer, info);
            other.GetEffectMgr().AddBleed(FightPlayer.Entity, _config.BleedTime, _config.BleedDmg);
        }
    }
}