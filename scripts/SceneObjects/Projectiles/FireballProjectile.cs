using Assembly.scripts.Effects;

namespace Assembly.scripts.SceneObjects.Projectiles;
using AO;
using Assembly.scripts.VFX;

public class FireballProjectile : BaseProjectile
{
    public float BurnTime = 2.0f;
    private Spine_Animator _animator;
    public override void Awake()
    {
        base.Awake();
        _animator = Entity.GetComponent<Spine_Animator>();
        if (_animator != null)
        {
            _animator.Awaken();
            var instance = _animator.SpineInstance;
            instance.SetAnimation("flying_loop", true);
            SoundId = SFX.Play(SFXKeys.FireballLoopAudio, new SFX.PlaySoundDesc() {EntityToFollow = Entity, Loop = true, LoopTimeout = 3f});
            _animator.OnAnimationEnd += evt =>
            {
                if (evt == "explode")
                {
                    Entity.Destroy();
                }
            };
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
            info.SkillKey = SkillConfig.FireballNodeConfig.SkillKey;
            info.SpecialDeathAnimation = true;
            
            // For Fireball Lv. 5, we shoot 3 fireballs
            // That might be too OP if all of them hit the same player.
            // Solution: If a player is burning, and the burn just started (meaning they just got hit by a fireball)
            // We reduce the fireball's damage to 1
            EffectBurn eb = fp.GetEffect<EffectBurn>();
            if (eb.Alive() && eb.ElapsedTime < 0.3f) info.ReactionInfo.Amount = 1;
            
            var overrideType = fp.TakeDamage(Owner, info);
            var reachedPlayer = overrideType != FightPlayer.DamageInfo.DamageNumberOverrideType.Dodged &&
                                overrideType != FightPlayer.DamageInfo.DamageNumberOverrideType.Parry;
            if (!Pierce)
            {
                _animator.SpineInstance.SetAnimation("explode", false);
                ModifySpeed(0.1f);
            }
            
            if (overrideType == FightPlayer.DamageInfo.DamageNumberOverrideType.Parry)
            {
                // Reflected! Change owner and send the projectile back.
                Vector2 refDir = Entity.Position - other.Position;
                Reflect(fp, Owner.Alive()? Owner.GetSkillTree().GetSkillLevel("Fireball") : 1, refDir);
            }

            if (reachedPlayer)
            {
                SFX.Play(SFXKeys.FireballHitAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity });
                EffectBurn.AddOrStackBurn(fp, Owner.Entity, BurnTime, EffectConfig.ProjectileConfig.FireballBurnDamageBase);
            }

        }
    }

    protected override BaseProjectile Reflect(FightPlayer newOwner, int level, Vector2 direction)
    {
        base.Reflect(newOwner, level, direction);
        if (Owner.Alive() && newOwner.Alive())
        {
            EffectConfig.ProjectileConfig config = EffectConfig.ProjectileConfig.GetPlayerFireballConfig(newOwner.CurrentAttack, level);
            Entity proj = Game.SpawnProjectile(newOwner, config.ProjectilePrefabKey,
                config.ProjectilePrefabKey,
                Entity.Position, direction);
            Projectile projComp = proj.GetComponent<Projectile>();
            projComp.Speed = config.Speed;
            projComp.Lifetime = config.ProjectileLifetime;
            
            FireballProjectile supplementProjectileComp = proj.GetComponent<FireballProjectile>();
            supplementProjectileComp.hitFxId = hitFxId;
            supplementProjectileComp.hitSoundId = hitSoundId;
            supplementProjectileComp.LifeTime = config.ProjectileLifetime;
            supplementProjectileComp.InitializeProjectile(newOwner, config.Damage, Pierce);
            supplementProjectileComp.BurnTime = BurnTime;
            return supplementProjectileComp;
        }

        return null;
    }
}