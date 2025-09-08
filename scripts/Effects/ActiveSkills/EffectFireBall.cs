using AO;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.SceneObjects.Projectiles;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityFireball : FightAbility
{
    public override string SkillKey => "Fireball";

    public override Type Effect => typeof(EffectFireBall);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => EffectConfig.ProjectileConfig.FireballRange;
    public override int MaxTargets => 1;

    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        int lv = int.Min(4, fp.GetSkillTree().GetSkillLevel("Fireball"));
        return lv > 2 ? EffectConfig.ProjectileConfig.FireballCooldown - 1 : EffectConfig.ProjectileConfig.FireballCooldown;
    }
}

public class EffectFireBall : EffectProjectileThrow
{
    public override void PlayThrowSound()
    {
        SFX.Play(SFXKeys.BefuddleThrowAudio, DefaultSoundDesc);
    }

    public override void AssignConfig()
    {
        Config = EffectConfig.ProjectileConfig.GetPlayerFireballConfig(FightPlayer.CurrentAttack, FightPlayer.GetSkillTree().GetSkillLevel("Fireball"));
    }


    protected override void InitializeProjectile(Entity proj)
    {
        Projectile projComp = proj.GetComponent<Projectile>();
        projComp.Speed = Config.Speed;
        projComp.Lifetime = Config.ProjectileLifetime;

        FireballProjectile supplementProjectileComp = proj.GetComponent<FireballProjectile>();
        supplementProjectileComp.LifeTime = Config.ProjectileLifetime;
        supplementProjectileComp.hitFxId = "hit_fire";
        supplementProjectileComp.hitSoundId = SFXKeys.FireballHitAudio;
        supplementProjectileComp.InitializeProjectile(FightPlayer, Config.Damage, false);
        supplementProjectileComp.BurnTime = EffectConfig.ProjectileConfig.FireballBurnTime + 0.1f;
        if (Config.ProjectileLevel > 3) supplementProjectileComp.BurnTime += 1;
    }

    public override void ProjectileThrow()
    {
        if (Config.ProjectileLevel > 4)
        {
            Entity proj = Game.SpawnProjectile(FightPlayer.Entity, Config.ProjectilePrefabKey,
                $"{Config.ProjectilePrefabKey}",
                FightPlayer.Entity.Position, AbilityDirection);
            //proj.Position = Entity.Position;
            InitializeProjectile(proj);

            Vector2 additionalDir1 = Vector2.Rotate(AbilityDirection, 0.72f, Vector2.Zero);
            Vector2 additionalDir2 = Vector2.Rotate(AbilityDirection, -0.72f, Vector2.Zero);

            Entity proj1 = Game.SpawnProjectile(FightPlayer.Entity, Config.ProjectilePrefabKey,
                $"{Config.ProjectilePrefabKey}",
                FightPlayer.Entity.Position, additionalDir1);

            Entity proj2 = Game.SpawnProjectile(FightPlayer.Entity, Config.ProjectilePrefabKey,
                $"{Config.ProjectilePrefabKey}",
                FightPlayer.Entity.Position, additionalDir2);

            InitializeProjectile(proj1);
            InitializeProjectile(proj2);
        }
        else
        {
            base.ProjectileThrow();
        }

    }
}