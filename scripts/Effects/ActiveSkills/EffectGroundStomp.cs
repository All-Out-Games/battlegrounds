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
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        AssignConfig(EffectConfig.GroundStompConfig.GetDefault(FightPlayer.CurrentAttack));

        Stomp();
    }
    
    public override void OnEffectEnd(bool interrupt)
    {
        
    }

    public void AssignConfig(EffectConfig.GroundStompConfig cfg)
    {
        Config = cfg;
        DurationRemaining = EffectConfig.GroundStompConfig.StompAnimationTime;
    }
    
    public void Stomp()
    {
        FightPlayer.SetAnimTrigger("fart");
        Coroutine.Start(Entity, DelayActiveStompHitbox(EffectConfig.GroundStompConfig.StompActivationTime));
    }
    
    IEnumerator DelayActiveStompHitbox(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        //Log.Debug($"STOMP! Dmg = {Config.StompDamage}");
        Vector2 selfPos = FightPlayer.Entity.Position;

        var cbPlayers = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(selfPos, Config.StompRadius);
        foreach (var other in cbPlayers)
        {
            FightPlayer.DamageInfo info = new FightPlayer.DamageInfo() { DmgType = DamageType.AOE};
            if(other.Entity.NetworkId == FightPlayer.Entity.NetworkId) continue;
                
            other.TakeDamage(Config.StompDamage, FightPlayer, info);
        }
        
        //FightPlayer.AddPlayerPunchCollisionFunction(OnPunchCollisionEnter);
    }
    
}