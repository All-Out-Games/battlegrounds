using AO;
using Assembly.scripts.Effects;
using Assembly.scripts.VFX;

namespace Assembly.scripts.SceneObjects.Projectiles;

public class BefuddleProjectile : BaseProjectile
{
    public float ConfusionTime = 3f;
    public float ConfusionIntensity = 125f;

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
            SoundId = SFX.Play(SFXKeys.ProjectileLoopAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity, Loop = true, LoopTimeout = 3f });
        }
        else
        {
            Log.Error("No Animator Found on projectile");
        }
    }

    protected override void DoProjectileEffect(Entity other, bool predicted)
    {

        FightPlayer fp = other.GetComponent<PlayerCollisionChild>()?.Player;
        if (fp != null && fp.Damageable())
        {
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Damage, DamageType.Ranged) with { InterruptLevel = FightPlayer.DamageInfo.StunInterruptLevel };
            info.ReactionInfo.Flinch = false;
            info.SkillKey = SkillConfig.BefuddleConfig.SkillKey;

            var overrideType = fp.TakeDamage(Owner, info);
            bool reachedPlayer = overrideType != FightPlayer.DamageInfo.DamageNumberOverrideType.Dodged &&
                                 overrideType != FightPlayer.DamageInfo.DamageNumberOverrideType.Parry;

            if (overrideType == FightPlayer.DamageInfo.DamageNumberOverrideType.Parry)
            {
                // Reflected! Change owner and send the projectile back.
                Vector2 refDir = Entity.Position - other.Position;
                Reflect(fp, Owner.Alive() ? Owner.GetSkillTree().GetSkillLevel("Befuddle") : 1, refDir);
            }

            if (!Pierce)
            {
                Entity.Destroy();
            }

            if (reachedPlayer)
            {
                fp.AddEffect<EffectConfusion>(Owner, ConfusionTime, confusion => confusion.ConfusionIntensity = ConfusionIntensity);
                FightClubGameManager.Instance.ClientSpawn(VFXPrefabs.BefuddleHitVFX, Vector2.Lerp(other.Position, Entity.Position, 0.5f));
                SFX.Play(SFXKeys.BefuddleHitAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity });
            }
        }
    }

    protected override BaseProjectile Reflect(FightPlayer newOwner, int level, Vector2 direction)
    {
        base.Reflect(newOwner, level, direction);
        if (Owner.Alive() && newOwner.Alive())
        {
            EffectConfig.ProjectileConfig config = EffectConfig.ProjectileConfig.GetPlayerBefuddleConfig(newOwner.CurrentAttack, level);
            Entity proj = Game.SpawnProjectile(newOwner.Entity, config.ProjectilePrefabKey,
                config.ProjectilePrefabKey,
                Entity.Position, direction);
            Projectile projComp = proj.GetComponent<Projectile>();
            projComp.Speed = config.Speed;
            projComp.Lifetime = config.ProjectileLifetime;

            BefuddleProjectile supplementProjectileComp = proj.GetComponent<BefuddleProjectile>();
            supplementProjectileComp.LifeTime = config.ProjectileLifetime;
            supplementProjectileComp.InitializeProjectile(newOwner, config.Damage, false);
            return supplementProjectileComp;
        }

        return null;
    }
}