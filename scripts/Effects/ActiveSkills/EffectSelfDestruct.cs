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
    }

    public override void OnEffectEnd(bool interrupt)
    {
        KnockingBlast();
    }

    protected void AssignConfig(EffectConfig.SelfDestructConfig cfg)
    {
        Config = cfg;
        DurationRemaining = EffectConfig.SelfDestructConfig.ActivationTime;
    }

    private void KnockingBlast()
    {
        if (Network.IsServer)
        {
            Log.Debug($"STOMP! Dmg = {Config.BlastDamage}");
            Vector2 selfPos = FightPlayer.Entity.Position;
            var cbPlayers = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(selfPos, EffectConfig.SelfDestructConfig.BlastRange);
            
            foreach (var fp in cbPlayers)
            {
                if (fp.Entity.NetworkId == FightPlayer.Entity.NetworkId)
                {
                    // Self damage
                    FightPlayer.DamageInfo selfDmgInfo = new FightPlayer.DamageInfo() with { Flinch = false};
                    FightPlayer.TakeDamage(Config.SelfDamage, FightPlayer, selfDmgInfo);
                }
                else
                {
                    FightPlayer.DamageInfo info = new FightPlayer.DamageInfo() { DmgType = DamageType.AOE};
                    fp.TakeDamage(Config.BlastDamage, FightPlayer, info);
                        
                    Vector2 bumpDir = fp.Entity.Position - selfPos;
                    fp.AddBumpFrom(FightPlayer, bumpDir * EffectConfig.SelfDestructConfig.BumpStrength, false);
                }
            }
        }
    }
}