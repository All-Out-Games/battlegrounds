using AO;
namespace Assembly.scripts.Effects.ActiveSkills;

public class EffectSelfDestruct : FightEffect
{
    protected EffectConfig.SelfDestructConfig Config;
    
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => false;

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        AssignConfig(EffectConfig.SelfDestructConfig.GetDefault(FightPlayer.CurrentAttack));
        DurationRemaining = EffectConfig.SelfDestructConfig.ActivationTime;
    }

    public override void OnEffectEnd(bool interrupt)
    {
        KnockingBlast();
    }

    protected void AssignConfig(EffectConfig.SelfDestructConfig cfg)
    {
        Config = cfg;
    }

    private void KnockingBlast()
    {
        if (Network.IsServer)
        {
            Log.Debug($"STOMP! Dmg = {Config.BlastDamage}");
            var cbPlayers = FightClubGameManager.Instance.GetCombatPlayers();
            Vector2 selfPos = FightPlayer.Entity.Position;
            foreach (var entity in cbPlayers)
            {
                if(entity.NetworkId == FightPlayer.Entity.NetworkId) continue;
                
                if (Vector2.Distance(selfPos, entity.Position) < EffectConfig.SelfDestructConfig.BlastRange)
                {
                    FightPlayer other = entity.GetComponent<FightPlayer>();
                    if (other != null)
                    {
                        FightPlayer.DamageInfo info = new FightPlayer.DamageInfo();
                        other.TakeDamage(Config.BlastDamage, FightPlayer, info);
                        
                        Vector2 bumpDir = other.Entity.Position - FightPlayer.Entity.Position;
                        other.AddBumpFrom(FightPlayer, bumpDir * EffectConfig.SelfDestructConfig.BumpStrength, false);
                    }
                }
            }

            FightPlayer.DamageInfo selfDmgInfo = new FightPlayer.DamageInfo() with { Flinch = false};
            FightPlayer.TakeDamage(Config.SelfDamage, FightPlayer, selfDmgInfo);
        }
    }
}