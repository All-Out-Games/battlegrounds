using AO;
namespace Assembly.scripts.Effects.ActiveSkills;



public class AbilitySelfDestruct : FightAbility
{
    public override string SkillKey => "SelfDestruct";
    public override Type Effect => typeof(EffectSelfDestruct);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    public override float Cooldown => EffectConfig.SelfDestructConfig.Cooldown;
}

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
        Vector2 selfPos = FightPlayer.Entity.Position;
        var cbPlayers = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(selfPos, EffectConfig.SelfDestructConfig.BlastRange);
            
        foreach (var fp in cbPlayers)
        {
            if (fp.Entity.NetworkId == FightPlayer.Entity.NetworkId)
            {
                // Self damage
                FightPlayer.DamageInfo selfDmgInfo = FightPlayer.DamageInfo.CreateSelfDamageInfo(Config.SelfDamage);
                FightPlayer.TakeDamage(FightPlayer, selfDmgInfo);
            }
            else
            {
                FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Config.BlastDamage, DamageType.AOE);
                fp.TakeDamage(FightPlayer, info);
                        
                Vector2 bumpDir = fp.Entity.Position - selfPos;
                fp.AddBumpFrom(FightPlayer, bumpDir * EffectConfig.SelfDestructConfig.BumpStrength, false);
            }
        }
    }
}