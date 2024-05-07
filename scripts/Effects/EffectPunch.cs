using System.Collections;
using AO;


public partial class EffectPunch : FightEffect
{
    private AbilityConfig.PunchConfig _config;

    EffectPunch()
    {
        IsActiveEffect = false;
        BlockAbilityActivation = true;
        IsValidTarget = false;
    }
    
    public override void OnEffectStart()
    {
        base.OnEffectStart();

        //Coroutine.Start(FightPlayer.Entity, DelayActivePunchHitbox(AbilityConfig.PunchConfig.PunchActivationTime));
        
        Punch();
    }

    public override void OnEffectEnd(bool interrupt)
    {
        
    }
    
    public void AssignConfig(AbilityConfig.PunchConfig cfg)
    {
        _config = cfg;
    }

    [ClientRpc]
    public void Punch()
    {
        FightPlayer.SetAnimTrigger("punch");
        if (Network.IsServer)
        {
            Coroutine.Start(Entity, DelayActivePunchHitbox(AbilityConfig.PunchConfig.PunchActivationTime));
        }
    }

    IEnumerator DelayActivePunchHitbox(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        Physics.RaycastHit rc;
        var hit = Physics.RaycastWithWhitelist(Entity.Position, FightPlayer.GetFacingDirection(),
            AbilityConfig.PunchConfig.PunchRange, FightClubGameManager.Instance.GetCombatPlayers().ToArray(), out rc);

        if (rc.Entity != null)
        {
            FightPlayer other = rc.Entity.GetComponent<FightPlayer>();
            other.TakeDamage(_config.PunchDamage);
        }
        Log.Debug("Falcon Punch!");
        //FightPlayer.AddPlayerPunchCollisionFunction(OnPunchCollisionEnter);
    }

    protected void OnPunchCollisionEnter(Entity other)
    {
        // NOT IN USE; Collider's on enter function will not function properly when activated without moving
        FightPlayer otherPlayer = other.GetComponent<FightPlayer>();
        if (otherPlayer != null)
        {
            // TODO: Whitelist the damaged player and check; Each punch should damage a player only once
            if (Network.IsServer)
            {
                otherPlayer.TakeDamage(_config.PunchDamage);
            }

        }
    }

    public override bool IsActiveEffect { get; }
    public override bool BlockAbilityActivation { get; }
    public override bool IsValidTarget { get; }
}