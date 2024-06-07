using System.Collections;

namespace Assembly.scripts.Effects.ActiveSkills;
using AO;

public class EffectGroundStomp : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => true;

    protected EffectConfig.GroundStompConfig Config;
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        AssignConfig(EffectConfig.GetPlayerGroundStompConfig(FightPlayer.CurrentAttack));

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
        if (FightPlayer.IsLocal)
        {
            // TODO: Get some real animation
            FightPlayer.SetAnimTrigger("fart"); // Animation can be done locally first...
        }
        if (Network.IsServer)
        {
            // Damage and broadcast animation
            FightPlayer.CallClient_SetAnimTriggerBroadcast("punch");
            Coroutine.Start(Entity, DelayActiveStompHitbox(EffectConfig.GroundStompConfig.StompActivationTime));
        }
    }
    
    IEnumerator DelayActiveStompHitbox(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (Network.IsServer)
        {
            Log.Debug($"STOMP! Dmg = {Config.StompDamage}");
            var cbPlayers = FightClubGameManager.Instance.GetCombatPlayers();
            Vector2 selfPos = FightPlayer.Entity.Position;
            foreach (var entity in cbPlayers)
            {
                if(entity.NetworkId == FightPlayer.Entity.NetworkId) continue;
                
                if (Vector2.Distance(selfPos, entity.Position) < Config.StompRadius)
                {
                    FightPlayer other = entity.GetComponent<FightPlayer>();
                    if (other != null)
                    {
                        FightPlayer.DamageReactionInfo info = new FightPlayer.DamageReactionInfo();
                        other.TakeDamage(Config.StompDamage, FightPlayer, info);
                    }
                }
            }
        }
        
        //FightPlayer.AddPlayerPunchCollisionFunction(OnPunchCollisionEnter);
    }
    
}