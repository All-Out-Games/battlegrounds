using AO;


public class EffectProjectileThrow : FightEffect
{

    protected EffectConfig.ProjectileConfig Config;
    
    EffectProjectileThrow()
    {
        IsActiveEffect = false;
        BlockAbilityActivation = true;
        IsValidTarget = true;
    }
    
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        FightPlayer.SetSkillBlockCast(true);
    }

    public override void OnEffectEnd(bool interrupt)
    {
        //Log.Debug($"Projectile Prefab Key {Config.ProjectilePrefabKey}");
        if (Network.IsServer)
        {
            ProjectileThrow();
        }
        FightPlayer.SetSkillBlockCast(false);
    }

    public virtual void AssignConfig(EffectConfig.ProjectileConfig cfg, string slotKey)
    {
        Config = cfg;
    }
    
    public virtual void ProjectileThrow()
    {
        // This function can be overwritten to create different projectile throwing behaviors
        // However, you should try to build the logic of the projectile within itself
        // i.e. inherit the Projectile component and put it on your prefab.

        //Entity proj = AO.Assets.GetAsset<Prefab>(Config.ProjectilePrefabKey).Instantiate();

        Entity proj = Game.SpawnProjectile(FightPlayer, Config.ProjectilePrefabKey,
            $"{FightPlayer.Id}_Spoon",
            FightPlayer.Entity.Position, Vector2.Left);
        //proj.Position = Entity.Position;
        Projectile projComp = proj.GetComponent<Projectile>();
        projComp.Speed = Config.Speed;
        projComp.Lifetime = Config.ProjectileLifetime;
        projComp.OnHit = (other, predicted) =>
        {
            Log.Debug($"Hit {other.Name} !");
            FightPlayer player = other.GetComponent<FightPlayer>();
            if (player != null)
            {
                player.TakeDamage(Config.Damage, FightPlayer);
                proj.Destroy();
            }
        };
    }

    public override bool IsActiveEffect { get; }
    public override bool BlockAbilityActivation { get; }
    public override bool IsValidTarget { get; }
}