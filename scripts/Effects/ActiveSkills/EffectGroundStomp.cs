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
    public override float Cooldown => EffectConfig.GroundStompConfig.Cooldown;
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
        AssignConfig(EffectConfig.GroundStompConfig.GetDefault(FightPlayer.CurrentAttack));
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
            FightClubGameManager.Instance.ClientSpawn(VFXPrefabKeys.LeapSlamCraterVfxPath, FightPlayer.Entity.Position);
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
        
        var cbPlayers = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(selfPos, EffectConfig.GroundStompConfig.StompRadius);
        foreach (var other in cbPlayers)
        {
            if(other.Entity.NetworkId == FightPlayer.Entity.NetworkId) continue;
                
            other.TakeDamage(FightPlayer, info);
        }
    }
    
    
}