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
        ProjectileThrow();
        FightPlayer.SetSkillBlockCast(false);
    }

    public virtual void AssignConfig(EffectConfig.ProjectileConfig cfg, string slotKey)
    {
        Config = cfg;
        SlotKey = slotKey;
    }
    
    public virtual void ProjectileThrow()
    {
        // This function can be overwritten to create different projectile throwing behaviors
        // However, you should try to build the logic of the projectile within itself
        // i.e. inherit the Projectile component and put it on your prefab.
        Log.Debug("Projectile Instantiated!");
        Entity proj = AO.Entity.Instantiate(AO.Assets.GetAsset<Prefab>(Config.ProjectilePrefabKey));
        //proj.Position = Entity.Position;
    }

    public override bool IsActiveEffect { get; }
    public override bool BlockAbilityActivation { get; }
    public override bool IsValidTarget { get; }
}