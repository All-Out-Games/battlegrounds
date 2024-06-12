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
        
        DoublePunch();
    }

    public override void OnEffectEnd(bool interrupt)
    {
        
    }
    
    public void AssignConfig(EffectConfig.DoublePunchConfig cfg)
    {
        DurationRemaining = EffectConfig.DoublePunchConfig.PunchAnimationTime * 2;
        Config = cfg;
    }
    
    public void DoublePunch()
    {
        // The first punch stuns the enemy if hit. The second punch knock them back
        if (FightPlayer.IsLocal)
        {
            FightPlayer.SetAnimTrigger("punch"); // Animation can be done locally first...
        }
        if (Network.IsServer)
        {
            // Damage and broadcast animation
            FightPlayer.CallClient_SetAnimTriggerBroadcast("punch");
            Coroutine.Start(Entity, DelayActivePunchHitbox(EffectConfig.DoublePunchConfig.PunchActivationTime));
        }
        // Run a delayed second punch on both client/server
        Coroutine.Start(Entity, SecondPunch(EffectConfig.DoublePunchConfig.PunchAnimationTime));
    }

    IEnumerator DelayActivePunchHitbox(float delayTime, int punchType = 0)
    {
        yield return new WaitForSeconds(delayTime);
        if (Network.IsServer)
        {
            Physics.RaycastHit rc;
            var hit = Physics.RaycastWithWhitelist(Entity.Position, FightPlayer.GetPunchDirection(),
                EffectConfig.DoublePunchConfig.PunchRange, FightClubGameManager.Instance.GetCombatPlayers().ToArray(), out rc);

            /*hit = Physics.Raycast(Entity.Position, FightPlayer.GetPunchDirection(),
                EffectConfig.PunchConfig.PunchRange, out rc);*/

            if (hit && rc.Entity != null)
            {
                FightPlayer other = rc.Entity.GetComponent<FightPlayer>();
                
                FightPlayer.DamageInfo info = new FightPlayer.DamageInfo();

                if (punchType == 0)
                {
                    // Stunning Punch
                    other.TakeDamage(Config.PunchDamage, FightPlayer, info);
                    other.GetEffectMgr().CallClient_AddStun(FightPlayer.Entity.NetworkId, EffectConfig.DoublePunchConfig.PunchAnimationTime);
                }
                else
                {
                    // Bumping Punch
                    other.TakeDamage(Config.PunchDamage, FightPlayer, info);
                    Vector2 bumpDir = other.Entity.Position - FightPlayer.Entity.Position;
                    other.AddBumpFrom(FightPlayer, bumpDir * Config.BumpStrength, false);
                }
                
            }
            
        }
        
        //FightPlayer.AddPlayerPunchCollisionFunction(OnPunchCollisionEnter);
    }

    IEnumerator SecondPunch(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (FightPlayer.IsLocal)
        {
            FightPlayer.SetAnimTrigger("punch"); // Animation can be done locally first...
        }
        if (Network.IsServer)
        {
            FightPlayer.CallClient_SetAnimTriggerBroadcast("punch");
            Coroutine.Start(Entity, DelayActivePunchHitbox(EffectConfig.DoublePunchConfig.PunchActivationTime, 1));
        }
    }
}