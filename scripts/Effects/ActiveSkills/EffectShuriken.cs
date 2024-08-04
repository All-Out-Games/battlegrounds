using AO;
using Assembly.scripts.SceneObjects.Projectiles;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityShuriken : FightAbility
{
    public override string SkillKey => "Shuriken";

    public override Type Effect => typeof(EffectShuriken);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => EffectConfig.ProjectileConfig.ShurikenRange;
    public override int MaxTargets => 1;
    
    public override float Cooldown => EffectConfig.ProjectileConfig.ShurikenCooldown;
}


public class EffectShuriken : EffectProjectileThrow
{
    public override void AssignConfig()
    {
        Config = EffectConfig.ProjectileConfig.GetPlayerShurikenConfig(FightPlayer.CurrentAttack);
    }

    protected override void InitializeProjectile(Entity proj)
    {
        Projectile projComp = proj.GetComponent<Projectile>();
        projComp.Speed = Config.Speed;
        projComp.Lifetime = Config.ProjectileLifetime;
            
        ShurikenProjectile supplementProjectileComp = proj.GetComponent<ShurikenProjectile>();
        supplementProjectileComp.LifeTime = Config.ProjectileLifetime;
        supplementProjectileComp.InitializeProjectile(FightPlayer, Config.Damage, false);
        supplementProjectileComp.BackDamageMultiplier = EffectConfig.ProjectileConfig.ShurikenBackDamageModifier;
        supplementProjectileComp.Enhanced = FightPlayer.HasSkill("NinjaMastery");
    }
    
}