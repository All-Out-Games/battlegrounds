using System.Collections;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.VFX;

namespace Assembly.scripts.Effects.ActiveSkills;
using AO;

public class AbilityGroundStomp : FightAbility
{
    public override string SkillKey => "GroundStomp";
    public override Type Effect => typeof(EffectGroundStomp);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        return fp.GetSkillTree().GetSkillLevel("GroundStomp") > 2
            ? EffectConfig.GroundStompConfig.Cooldown - 1
            : EffectConfig.GroundStompConfig.Cooldown;
    }
}

public class EffectGroundStomp : FightEffectWithNoFlinch
{
    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => true;
    public override bool FreezePlayer => true;

    protected EffectConfig.GroundStompConfig Config;
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.SetAnimTrigger("groundstomp");
        AssignConfig(EffectConfig.GroundStompConfig.GetDefault(FightPlayer.CurrentAttack, FightPlayer.GetSkillTree().GetSkillLevel("GroundStomp")));
        FightPlayer.SpineAnimator.OnEvent += OnAnimationEvent;

        SoundId = SFX.Play(SFXKeys.GroundStompAudio, DefaultSoundDesc);
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.SpineAnimator.OnEvent -= OnAnimationEvent;
    }
    

    public override void OnAnimationEvent(string eventName)
    {
        base.OnAnimationEvent(eventName);
        if (eventName == "Attack")
        {
            FightClubGameManager.Instance.ClientSpawn(VFXPrefabKeys.LeapSlamCraterVfxPath, FightPlayer.Entity.Position,
                entity =>
                {
                    entity.LocalScale *= Config.StompSizeMultiplier;
                });
            Stomp();
        }
    }

    public void AssignConfig(EffectConfig.GroundStompConfig cfg)
    {
        Config = cfg;
        DurationRemaining = MainLayer.GetCurrentStateLength();
    }
    
    public void Stomp()
    {
        Vector2 selfPos = FightPlayer.Entity.Position;

        FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Config.StompDamage, DamageType.AOE);
        info.SkillKey = SkillConfig.GroundStompConfig.SkillKey;
        info.CrateImmediateDestroy = true;
        
        var cbPlayers = FightClubGameManager.Instance.OverlapCircleForDamageables(selfPos, EffectConfig.GroundStompConfig.StompRadius * Config.StompSizeMultiplier, Player);
        foreach (var other in cbPlayers)
        {
            if(!other.Damageable()) continue;
            other.TakeDamage(FightPlayer, info);
        }
    }
    
    
}