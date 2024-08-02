using AO;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.SceneObjects.Projectiles;
namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityBefuddle : FightAbility
{
    public override string SkillKey => "Befuddle";

    public override Type Effect => typeof(EffectBefuddle);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => EffectConfig.ProjectileConfig.BefuddleRange;
    public override int MaxTargets => 1;
    
    public override float Cooldown => EffectConfig.ProjectileConfig.BefuddleCooldown;
}

public class EffectBefuddle : EffectProjectileThrow
{
    public override void PlayThrowSound()
    {
        SFX.Play(SFXKeys.BefuddleThrowAudio, DefaultSoundDesc);
    }

    public override void AssignConfig()
    {
        Config = EffectConfig.ProjectileConfig.GetPlayerBefuddleConfig(FightPlayer.CurrentAttack);
    }
    

    protected override void InitializeProjectile(Entity proj)
    {
        Projectile projComp = proj.GetComponent<Projectile>();
        projComp.Speed = Config.Speed;
        projComp.Lifetime = Config.ProjectileLifetime;
        
        BefuddleProjectile supplementProjectileComp = proj.GetComponent<BefuddleProjectile>();
        supplementProjectileComp.LifeTime = Config.ProjectileLifetime;
        supplementProjectileComp.InitializeProjectile(FightPlayer, Config.Damage, false);
        supplementProjectileComp.ConfusionTime = EffectConfig.ProjectileConfig.BefuddleConfusionTime;
        supplementProjectileComp.ConfusionIntensity = EffectConfig.ProjectileConfig.BefuddleConfusionIntensity;
    }
}