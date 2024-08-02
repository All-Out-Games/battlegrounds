using AO;
using Assembly.scripts.SceneObjects.Projectiles;

namespace Assembly.scripts.Effects.ActiveSkills;


public class AbilityPsybolt : FightAbility
{
    public override string SkillKey => "Psybolt";

    public override Type Effect => typeof(EffectPsybolt);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => EffectConfig.ProjectileConfig.PsyboltRange;
    public override int MaxTargets => 1;
    
    public override float Cooldown => EffectConfig.ProjectileConfig.PsyboltCooldown;
}

public class EffectPsybolt : EffectProjectileThrow
{

    public override void AssignConfig()
    {
        Config = EffectConfig.ProjectileConfig.GetPlayerPsyboltConfig(FightPlayer.CurrentAttack);
    }

    protected override void InitializeProjectile(Entity proj)
    {
        Projectile projComp = proj.GetComponent<Projectile>();
        projComp.Speed = Config.Speed;
        projComp.Lifetime = Config.ProjectileLifetime;
        
        PsyBoltProjectile supplementProjectileComp = proj.GetComponent<PsyBoltProjectile>();
        supplementProjectileComp.LifeTime = Config.ProjectileLifetime;
        supplementProjectileComp.InitializeProjectile(FightPlayer, Config.Damage, false);
        supplementProjectileComp.KnockBackStrength = EffectConfig.ProjectileConfig.PsyboltKnockbackStrength;
    }
    
}