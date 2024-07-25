using AO;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.SceneObjects.Projectiles;

namespace Assembly.scripts.Effects.ActiveSkills;
public class AbilitySpoonThrow : FightAbility
{
    public override string SkillKey => "SpoonThrow";

    public override Type Effect => typeof(EffectProjectileThrow);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => EffectConfig.ProjectileConfig.SpoonRange;
    public override int MaxTargets => 1;
    
    public override float Cooldown => EffectConfig.ProjectileConfig.SpoonThrowCooldown;
}

public class EffectProjectileThrow : FightEffect
{

    protected EffectConfig.ProjectileConfig Config;
    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => true;

    protected List<Entity> WhiteList;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        WhiteList = new List<Entity>();
        WhiteList.Add(FightPlayer.Entity);
        
        AssignConfig();
        FightPlayer.SetAnimTrigger(Config.ThrowTrigger);
        //FightPlayer.SetMouseIKPosition(AbilityPositionOrDirection);
        DurationRemaining = FightLayer.GetCurrentStateLength();
        FightPlayer.SpineAnimator.OnEvent += OnAnimationEvent;
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.SpineAnimator.OnEvent -= OnAnimationEvent;
    }

    public override void OnAnimationEvent(string eventName)
    {
        base.OnAnimationEvent(eventName);
        if (eventName == "Attack")
        {
            ProjectileThrow();
        }
    }

    public virtual void AssignConfig()
    {
        Config = EffectConfig.ProjectileConfig.GetPlayerSpoonThrowConfig(FightPlayer.CurrentAttack);
    }
    
    public virtual void ProjectileThrow()
    {
        // This function can be overwritten to create different projectile throwing behaviors
        // However, you should try to build the logic of the projectile within itself
        // i.e. inherit the Projectile component and put it on your prefab.

        Entity proj = Game.SpawnProjectile(FightPlayer, Config.ProjectilePrefabKey,
            $"{Config.ProjectilePrefabKey}",
            FightPlayer.Entity.Position, AbilityPositionOrDirection);
        //proj.Position = Entity.Position;
        InitializeProjectile(proj);
    }

    protected virtual void InitializeProjectile(Entity proj)
    {
        // Default Projectile (Spoon)
        Projectile projComp = proj.GetComponent<Projectile>();
        projComp.Speed = Config.Speed;
        projComp.Lifetime = Config.ProjectileLifetime;
        BaseProjectile supplementProjectileComp = proj.GetComponent<BaseProjectile>();
        supplementProjectileComp.InitializeProjectile(FightPlayer, Config.Damage, false);
    }


}