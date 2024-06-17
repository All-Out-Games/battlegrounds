using System.Collections;

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

public class EffectGroundStomp : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => true;
    public override bool FreezePlayer => true;

    protected EffectConfig.GroundStompConfig Config;
    protected bool Stomped = false;
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        AssignConfig(EffectConfig.GroundStompConfig.GetDefault(FightPlayer.CurrentAttack));
        FightPlayer.SetAnimTrigger("fart");
    }
    
    public override void OnEffectEnd(bool interrupt)
    {
        
    }

    public override void OnEffectUpdate()
    {
        if (!Stomped && ElapsedTime > EffectConfig.GroundStompConfig.StompActivationTime)
        {
            Stomp();
            Stomped = true;
        }
    }

    public void AssignConfig(EffectConfig.GroundStompConfig cfg)
    {
        Config = cfg;
        DurationRemaining = EffectConfig.GroundStompConfig.StompAnimationTime;
    }
    
    public void Stomp()
    {
        Vector2 selfPos = FightPlayer.Entity.Position;

        FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Config.StompDamage, DamageType.AOE);
        
        var cbPlayers = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(selfPos, Config.StompRadius);
        foreach (var other in cbPlayers)
        {
            if(other.Entity.NetworkId == FightPlayer.Entity.NetworkId) continue;
                
            other.TakeDamage(FightPlayer, info);
        }
    }
    
    
}