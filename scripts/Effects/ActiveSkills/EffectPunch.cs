using System.Collections;
using AO;

public class AbilityPunch : FightAbility
{
    public override string SkillKey => "Punch";
    public override Type Effect => typeof(EffectPunch);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    
}

public class EffectPunch : FightEffect
{
    protected EffectConfig.PunchConfig Config;
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => true;
    
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        
        AssignConfig(EffectConfig.GetPlayerPunchConfig(1, FightPlayer.CurrentAttack));
        
        Punch();
    }

    public override void OnEffectEnd(bool interrupt)
    {
        
    }
    
    public void AssignConfig(EffectConfig.PunchConfig cfg)
    {
        DurationRemaining = EffectConfig.PunchConfig.PunchAnimationTime;
        Config = cfg;
    }
    
    public void Punch()
    {
        FightPlayer.SetAnimTrigger("punch"); // Animation can be done locally first...
        //FightPlayer.CallClient_SetAnimTriggerBroadcast("punch");
        Coroutine.Start(Entity, DelayActivePunchHitbox(EffectConfig.PunchConfig.PunchActivationTime));
    }

    IEnumerator DelayActivePunchHitbox(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        Physics.RaycastHit rc;
        var hit = Physics.RaycastWithWhitelist(Entity.Position, FightPlayer.GetPunchDirection(),
            EffectConfig.PunchConfig.PunchRange, FightClubGameManager.Instance.GetCombatPlayersAsEntities(), out rc);

        if (rc.Entity != null)
        {
            FightPlayer other = rc.Entity.GetComponent<FightPlayer>();
                
            FightPlayer.DamageInfo info = new FightPlayer.DamageInfo();
            other.TakeDamage(Config.PunchDamage, FightPlayer, info);
        }
    }

    protected void OnPunchCollisionEnter(Entity other)
    {
        // NOT IN USE; Collider's on enter function will not function properly when activated without moving
        FightPlayer otherPlayer = other.GetComponent<FightPlayer>();
        if (otherPlayer != null)
        {
            FightPlayer.DamageInfo info = new FightPlayer.DamageInfo();
            otherPlayer.TakeDamage(Config.PunchDamage, FightPlayer, info);
        }
    }


}