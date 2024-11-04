using AO;
using Assembly.scripts.SceneObjects;
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
    
    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        int lv = fp.GetSkillTree().GetSkillLevel("Psybolt");
        float cd = EffectConfig.ProjectileConfig.PsyboltCooldown;
        if (lv > 2)
        {
            cd -= 1;
        }

        return cd;
    }
}

public class EffectPsybolt : EffectProjectileThrow
{
    public override void PlayThrowSound()
    {
        SFX.Play(SFXKeys.PsyboltShootAudio, DefaultSoundDesc);
    }

    public override void AssignConfig()
    {
        Config = EffectConfig.ProjectileConfig.GetPlayerPsyboltConfig(FightPlayer.CurrentAttack, FightPlayer.GetSkillTree().GetSkillLevel("Psybolt"));
    }
    
    public override void ProjectileThrow()
    {
        // This function can be overwritten to create different projectile throwing behaviors
        // However, you should try to build the logic of the projectile within itself
        // i.e. inherit the Projectile component and put it on your prefab.

        Entity proj = Game.SpawnProjectile(FightPlayer, Config.ProjectilePrefabKey,
            $"{Config.ProjectilePrefabKey}",
            FightPlayer.Entity.Position + Vector2.Up, AbilityDirection);
        //proj.Position = Entity.Position;
        InitializeProjectile(proj);
    }

    protected override void InitializeProjectile(Entity proj)
    {
        Projectile projComp = proj.GetComponent<Projectile>();
        projComp.Speed = Config.Speed;
        projComp.Lifetime = Config.ProjectileLifetime;
        
        PsyBoltProjectile supplementProjectileComp = proj.GetComponent<PsyBoltProjectile>();
        supplementProjectileComp.LifeTime = Config.ProjectileLifetime;
        supplementProjectileComp.InitializeProjectile(FightPlayer, Config.Damage, false);
        supplementProjectileComp.KnockBackStrength = EffectConfig.ProjectileConfig.PsyboltKnockbackStrength + (FightPlayer.GetSkillTree().GetSkillLevel("Psybolt") > 4 ? 40 : 0);

        if (FightPlayer.HasSkill("Psychic"))
        {
            proj.LocalScale *= 1.25f;
        }
    }
    
}