using System.Collections;
using AO;


public partial class EffectPunch : FightEffect
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
        FightPlayer.SetSkillBlockCast(true);
    }

    public override void OnEffectEnd(bool interrupt)
    {
        FightPlayer.SetSkillBlockCast(false);
    }
    
    public void AssignConfig(EffectConfig.PunchConfig cfg)
    {
        DurationRemaining = EffectConfig.PunchConfig.PunchAnimationTime;
        Config = cfg;
    }
    
    public void Punch()
    {
        if (Network.IsServer)
        {
            FightPlayer.CallClient_SetAnimTrigger("punch");
            Coroutine.Start(Entity, DelayActivePunchHitbox(EffectConfig.PunchConfig.PunchActivationTime));
        }
    }

    IEnumerator DelayActivePunchHitbox(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (Network.IsServer)
        {
            Physics.RaycastHit rc;
            var hit = Physics.RaycastWithWhitelist(Entity.Position, FightPlayer.GetPunchDirection(),
                EffectConfig.PunchConfig.PunchRange, FightClubGameManager.Instance.GetCombatPlayers().ToArray(), out rc);

            if (rc.Entity != null)
            {
                FightPlayer other = rc.Entity.GetComponent<FightPlayer>();
                
                FightPlayer.DamageReactionInfo info = new FightPlayer.DamageReactionInfo();
                other.TakeDamage(Config.PunchDamage, FightPlayer, info);
                // TODO: Player dealt damage to others event (for reward and stuff)
            }
            Log.Debug($"Shin: Falcon Punch! Dmg = {Config.PunchDamage}");
        }
        
        //FightPlayer.AddPlayerPunchCollisionFunction(OnPunchCollisionEnter);
    }

    protected void OnPunchCollisionEnter(Entity other)
    {
        // NOT IN USE; Collider's on enter function will not function properly when activated without moving
        FightPlayer otherPlayer = other.GetComponent<FightPlayer>();
        if (otherPlayer != null)
        {
            if (Network.IsServer)
            {
                FightPlayer.DamageReactionInfo info = new FightPlayer.DamageReactionInfo();
                otherPlayer.TakeDamage(Config.PunchDamage, FightPlayer, info);
            }

        }
    }


}