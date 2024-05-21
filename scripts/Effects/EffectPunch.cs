using System.Collections;
using AO;


public partial class EffectPunch : FightEffect
{
    private EffectConfig.PunchConfig _config;

    EffectPunch()
    {
        IsActiveEffect = false;
        BlockAbilityActivation = true;
        IsValidTarget = false;
    }
    
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        Punch();
        FightPlayer.SetSkillBlockCast(true);
    }

    public override void OnEffectEnd(bool interrupt)
    {
        FightPlayer.SetSkillBlockCast(false);
    }
    
    public void AssignConfig(EffectConfig.PunchConfig cfg)
    {
        _config = cfg;
    }

    [ClientRpc]
    public void Punch()
    {
        FightPlayer.SetAnimTrigger("punch");
        if (Network.IsServer)
        {
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
                other.TakeDamage(_config.PunchDamage);
            }
            Log.Debug($"Shin: Falcon Punch! Dmg = {_config.PunchDamage}");
        }
        
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