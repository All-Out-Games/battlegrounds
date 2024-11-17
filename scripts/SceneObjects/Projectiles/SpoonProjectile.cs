using AO;
using Assembly.scripts.VFX;

namespace Assembly.scripts.SceneObjects.Projectiles;

public class SpoonProjectile : BaseProjectile
{
    private Spine_Animator _animator;
    public override void Awake()
    {
        base.Awake();
        _animator = Entity.GetComponent<Spine_Animator>();
        if (_animator != null)
        {
            _animator.Awaken();
            var instance = _animator.SpineInstance;
            instance.SetSkin("spoon");
            instance.EnableSkin("spoon");
            instance.SetAnimation("fly_straight", true);
            SoundId = SFX.Play(SFXKeys.ProjectileLoopAudio, new SFX.PlaySoundDesc() {EntityToFollow = Entity, Loop = true, LoopTimeout = 3f});
        }
        else
        {
            Log.Error("No Animator Found on projectile");
            Entity.Destroy();
        }
    }

    protected override void DoProjectileEffect(Entity other, bool predicted)
    {
        
        FightPlayer fp = other.GetComponent<PlayerCollisionChild>()?.Player;
        if (fp != null && fp.Damageable())
        {
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Damage, DamageType.Ranged);
            info.SkillKey = SkillConfig.SpoonThrowConfig.SkillKey;
            var overrideType = fp.TakeDamage(Owner, info);
            var reachedPlayer = overrideType != FightPlayer.DamageInfo.DamageNumberOverrideType.Dodged &&
                                overrideType != FightPlayer.DamageInfo.DamageNumberOverrideType.Parry;
            if (!Pierce)
            {
                Entity.Destroy();
            }
            
            if (overrideType == FightPlayer.DamageInfo.DamageNumberOverrideType.Parry)
            {
                // Reflected! Change owner and send the projectile back.
                Vector2 refDir = Entity.Position - other.Position;
                Reflect(fp, Owner.Alive()? Owner.GetSkillTree().GetSkillLevel("SpoonThrow") : 1, refDir);
            }

            if (reachedPlayer)
            {
                FightClubGameManager.Instance.ClientSpawn(VFXPrefabs.HitVFX, Vector2.Lerp(other.Position, Entity.Position, 0.5f),
                    entity =>
                    {
                        SelectionVFX vfx = entity.GetComponent<SelectionVFX>();
                        vfx.StartVFX(hitFxId, false);
                    }
                );
                SFX.Play(SFXKeys.SpoonHitAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity });
            }

        }
    }
    
    protected override BaseProjectile Reflect(FightPlayer newOwner, int level, Vector2 direction)
    {
        base.Reflect(newOwner, level, direction);
        if (Owner.Alive() && newOwner.Alive())
        {
            EffectConfig.ProjectileConfig config = EffectConfig.ProjectileConfig.GetPlayerSpoonThrowConfig(newOwner.CurrentAttack, level);
            Entity proj = Game.SpawnProjectile(newOwner, config.ProjectilePrefabKey,
                config.ProjectilePrefabKey,
                Entity.Position, direction);
            Projectile projComp = proj.GetComponent<Projectile>();
            projComp.Speed = config.Speed;
            projComp.Lifetime = config.ProjectileLifetime;
            
            SpoonProjectile supplementProjectileComp = proj.GetComponent<SpoonProjectile>();
            supplementProjectileComp.LifeTime = config.ProjectileLifetime;
            supplementProjectileComp.InitializeProjectile(newOwner, config.Damage, false);
            return supplementProjectileComp;
        }

        return null;
    }
}