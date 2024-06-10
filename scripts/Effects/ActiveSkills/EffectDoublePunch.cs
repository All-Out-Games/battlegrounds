namespace Assembly.scripts.Effects.ActiveSkills;
using System.Collections;
using AO;

public class EffectDoublePunch : FightEffect
{
    protected EffectConfig.DoublePunchConfig Config;
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => true;

    
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        
        AssignConfig(EffectConfig.DoublePunchConfig.GetDefault(FightPlayer.CurrentAttack));
        
        Punch();
    }

    public override void OnEffectEnd(bool interrupt)
    {
        
    }
    
    public void AssignConfig(EffectConfig.DoublePunchConfig cfg)
    {
        DurationRemaining = EffectConfig.DoublePunchConfig.PunchAnimationTime * 2;
        Config = cfg;
    }
    
    public void Punch()
    {
        if (FightPlayer.IsLocal)
        {
            FightPlayer.SetAnimTrigger("punch"); // Animation can be done locally first...
        }
        if (Network.IsServer)
        {
            // Damage and broadcast animation
            FightPlayer.CallClient_SetAnimTriggerBroadcast("punch");
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

            /*hit = Physics.Raycast(Entity.Position, FightPlayer.GetPunchDirection(),
                EffectConfig.PunchConfig.PunchRange, out rc);*/

            if (rc.Entity != null)
            {
                FightPlayer other = rc.Entity.GetComponent<FightPlayer>();
                
                FightPlayer.DamageInfo info = new FightPlayer.DamageInfo();
                other.TakeDamage(Config.PunchDamage, FightPlayer, info);
            }
        }
        
        //FightPlayer.AddPlayerPunchCollisionFunction(OnPunchCollisionEnter);
    }
}