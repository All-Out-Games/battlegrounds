using System.Collections;

namespace Assembly.scripts.Effects.ActiveSkills;
using AO;

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
        if (FightPlayer.IsLocal)
        {
            // TODO: Get some real animation
            FightPlayer.SetAnimTrigger("fart"); // Animation can be done locally first...
        }
        if (Network.IsServer)
        {
            // Damage and broadcast animation
            FightPlayer.CallClient_SetAnimTriggerBroadcast("fart");
            Coroutine.Start(Entity, DelayActiveStompHitbox(EffectConfig.GroundStompConfig.StompActivationTime));
        }
    }
    
    IEnumerator DelayActiveStompHitbox(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (Network.IsServer)
        {
            //Log.Debug($"STOMP! Dmg = {Config.StompDamage}");
            Vector2 selfPos = FightPlayer.Entity.Position;

            var cbPlayers = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(selfPos, Config.StompRadius);
            foreach (var other in cbPlayers)
            {
                FightPlayer.DamageInfo info = new FightPlayer.DamageInfo() { DmgType = DamageType.AOE};
                if(other.Entity.NetworkId == FightPlayer.Entity.NetworkId) continue;
                
                other.TakeDamage(Config.StompDamage, FightPlayer, info);
            }
        }
        
        //FightPlayer.AddPlayerPunchCollisionFunction(OnPunchCollisionEnter);
    }
    
}