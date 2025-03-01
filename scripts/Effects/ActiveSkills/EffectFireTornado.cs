using Assembly.scripts.SceneObjects;
using Assembly.scripts.SceneObjects.Projectiles;

namespace Assembly.scripts.Effects.ActiveSkills;
using AO;

public class AbilityFireTornado : FightAbility
{
    public override string SkillKey => "FireTornado";

    public override Type Effect => typeof(EffectFireTornado);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => 5f;

    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        return fp.GetSkillTree().GetSkillLevel("FireTornado") > 1 ? EffectConfig.FlashOfSteelConfig.Cooldown - 1 : EffectConfig.FlashOfSteelConfig.Cooldown;
    }
}

public class EffectFireTornado : FightEffect
{
    public override bool IsActiveEffect => true;
    
    public override bool BlockAbilityActivation => true;

    private bool _launched;

    private EffectConfig.ProjectileConfig Config;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        Config = EffectConfig.FireTornadoConfig.GetTornadoCfg(FightPlayer.CurrentAttack, FightPlayer.GetSkillTree().GetSkillLevel("FireTornado"));
        if (!isDropIn)
        {
            DurationRemaining = 0.4f;
        }
        FightPlayer.SetAnimTrigger("summon_thunder", true);
        SFX.Play(SFXKeys.FireTornadoStartAudio, DefaultSoundDesc with{Volume = 0.65f});
        FightPlayer.SetFacingDirection(AbilityDirection.X > 0);
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if (Util.OneTime(ElapsedTime > 0.15f, ref _launched))
        {
            ProjectileThrow();
        }
    }
    
    private void ProjectileThrow()
    {

        Entity proj = Game.SpawnProjectile(FightPlayer, Config.ProjectilePrefabKey,
            $"{Config.ProjectilePrefabKey}",
            Position + AbilityDirection, AbilityDirection);
        //proj.Position = Entity.Position;
        InitializeProjectile(proj);
    }

    private void InitializeProjectile(Entity proj)
    {
        Projectile projComp = proj.GetComponent<Projectile>();
        projComp.Speed = Config.Speed;
        projComp.Lifetime = Config.ProjectileLifetime;
        FireTornadoProjectile supplementProjectileComp = proj.GetComponent<FireTornadoProjectile>();
        supplementProjectileComp.LifeTime = Config.ProjectileLifetime;
        supplementProjectileComp.InitializeProjectile(FightPlayer, Config.Damage, true);
        supplementProjectileComp.ProjectileLevel = Config.ProjectileLevel;
        proj.LocalRotation = 0; // This is one rare case that we don't want the projectile rotated based on emission direction
    }
}