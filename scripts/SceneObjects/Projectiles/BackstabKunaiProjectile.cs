using AO;
using Assembly.scripts.Effects.ActiveSkills;

namespace Assembly.scripts.SceneObjects.Projectiles;

public partial class BackstabKunaiProjectile : BaseProjectile
{
    private Spine_Animator _animator;
    public override void Awake()
    {
        base.Awake();
        _animator = Entity.GetComponent<Spine_Animator>();
        if (_animator != null)
        {
            var instance = _animator.SpineInstance;
            instance.SetSkin("kunai");
            instance.EnableSkin("kunai");
            instance.SetAnimation("fly_straight", true);
            SoundId = SFX.Play(SFXKeys.ShurikenLoopAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity, Loop = true,LoopTimeout = 3f});
        }
        else
        {
            Log.Error("No Animator Found on projectile");
        }
    }
    protected override void DoProjectileEffect(Entity other, bool predicted)
    {
        FightPlayer fp = other.GetComponent<PlayerCollisionChild>()?.Player;
        if (fp.Alive() && fp.Damageable())
        {
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(1, DamageType.Ranged) with {InterruptLevel = FightPlayer.DamageInfo.StunInterruptLevel};
            info.ReactionInfo.Flinch = false;
            info.SkillKey = SkillConfig.BackstabConfig.SkillKey;
            
            var overrideType = fp.TakeDamage(Owner, info);
            bool reachedPlayer = overrideType != FightPlayer.DamageInfo.DamageNumberOverrideType.Dodged &&
                                 overrideType != FightPlayer.DamageInfo.DamageNumberOverrideType.Parry;
            
            if (overrideType == FightPlayer.DamageInfo.DamageNumberOverrideType.Parry)
            {
                // Reflected! Change owner and send the projectile back.
                Vector2 refDir = Entity.Position - other.Position;
                Reflect(fp, Owner.Alive()? Owner.GetSkillTree().GetSkillLevel("Backstab") : 1, refDir);
            }
            
            if (Network.IsServer && reachedPlayer)
            {
                CallClient_BackstabPlayer(Owner, fp);
            }

            if (!Pierce)
            {
                Entity.Destroy();
            }
        }
    }

    [ClientRpc]
    public static void BackstabPlayer(FightPlayer shooter, FightPlayer target)
    {
        target.AddEffect<EffectBackstab>(shooter, EffectConfig.BackStabConfig.BackstabTime);
        shooter.AddEffect<EffectBackstabCaster>(target, EffectConfig.BackStabConfig.BackstabTime);
    }
    
    protected override BaseProjectile Reflect(FightPlayer newOwner, int level, Vector2 direction)
    {
        base.Reflect(newOwner, level, direction);
        if (Owner.Alive() && newOwner.Alive())
        {
            EffectConfig.ProjectileConfig config = EffectConfig.BackStabConfig.GetKunaiConfig();
            Entity proj = Game.SpawnProjectile(newOwner, config.ProjectilePrefabKey,
                config.ProjectilePrefabKey,
                Entity.Position, direction);
            Projectile projComp = proj.GetComponent<Projectile>();
            projComp.Speed = config.Speed;
            projComp.Lifetime = config.ProjectileLifetime;
            
            BackstabKunaiProjectile supplementProjectileComp = proj.GetComponent<BackstabKunaiProjectile>();
            supplementProjectileComp.LifeTime = config.ProjectileLifetime;
            supplementProjectileComp.InitializeProjectile(newOwner, config.Damage, false);
            return supplementProjectileComp;
        }

        return null;
    }
}