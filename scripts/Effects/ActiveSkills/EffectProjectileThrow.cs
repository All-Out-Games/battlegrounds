using AO;


public class EffectProjectileThrow : FightEffect
{

    protected EffectConfig.ProjectileConfig Config;
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => true;

    protected List<Entity> WhiteList;

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        WhiteList = new List<Entity>();
        WhiteList.Add(FightPlayer.Entity);
        
        AssignConfig(EffectConfig.GetPlayerSpoonThrowConfig(FightPlayer.CurrentAttack));
        DurationRemaining = Config.ThrowAnimationLength;
    }

    public override void OnEffectEnd(bool interrupt)
    {
        //Log.Debug($"Projectile Prefab Key {Config.ProjectilePrefabKey}");
        ProjectileThrow();
    }

    public virtual void AssignConfig(EffectConfig.ProjectileConfig cfg)
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
            FightPlayer.Entity.Position, AbilityPositionOrDirection);
        //proj.Position = Entity.Position;
        Projectile projComp = proj.GetComponent<Projectile>();
        projComp.Speed = Config.Speed;
        projComp.Lifetime = Config.ProjectileLifetime;
        
        projComp.OnHit = (other, predicted) =>
        {
            if (WhiteList.Contains(other))
            {
                return;
            }
            Log.Debug($"Hit {other.Name} !");
            FightPlayer player = other.GetComponent<FightPlayer>();
            if (player != null)
            {
                // TODO： check player status here (do not damage spectators) Player status seems to be synced incorrectly
                WhiteList.Add(player.Entity);
                FightPlayer.DamageReactionInfo info = new FightPlayer.DamageReactionInfo();
                player.TakeDamage(Config.Damage, FightPlayer, info);
                proj.Destroy();
            }
        };
    }


}