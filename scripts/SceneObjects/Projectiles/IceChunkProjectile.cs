using Assembly.scripts.VFX;

namespace Assembly.scripts.SceneObjects.Projectiles;
using AO;

public class IceChunkProjectile : BaseProjectile
{
    public override void Awake()
    {
        base.Awake();
        hitFxId = "hit_ice";
        hitSoundId = SFXKeys.IceProjectileHitAudio;

        float chance = Random.Shared.NextFloat();
        Sprite_Renderer rdr = GetComponent<Sprite_Renderer>();
        if (rdr != null)
        {
            if (chance < 0.33)
            {
                rdr.Sprite = Assets.GetAsset<Texture>("projectile/ice_chunk_B.png");
            }
            else if (chance < 0.66)
            {
                rdr.Sprite = Assets.GetAsset<Texture>("projectile/ice_chunk_C.png");
            }
        }

    }

    protected override void DoProjectileEffect(Entity other, bool predicted)
    {

        FightPlayer fp = other.GetComponent<PlayerCollisionChild>()?.Player;
        if (fp != null && fp.Damageable())
        {
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Damage, DamageType.Ranged);
            info.SkillKey = SkillConfig.IceStormNodeConfig.SkillKey;
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
                Reflect(fp, 1, refDir);
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
                SFX.Play(hitSoundId, new SFX.PlaySoundDesc() { EntityToFollow = Entity });
            }

        }
    }

    protected override BaseProjectile Reflect(FightPlayer newOwner, int level, Vector2 direction)
    {
        base.Reflect(newOwner, level, direction);
        if (Owner.Alive() && newOwner.Alive())
        {
            EffectConfig.ProjectileConfig config = EffectConfig.ProjectileConfig.GetPlayerIceChunkConfig(newOwner.CurrentAttack);
            Entity proj = Game.SpawnProjectile(newOwner.Entity, config.ProjectilePrefabKey,
                config.ProjectilePrefabKey,
                Entity.Position, direction);
            Projectile projComp = proj.GetComponent<Projectile>();
            projComp.Speed = config.Speed;
            projComp.Lifetime = config.ProjectileLifetime;

            IceChunkProjectile supplementProjectileComp = proj.GetComponent<IceChunkProjectile>();
            supplementProjectileComp.LifeTime = config.ProjectileLifetime;
            supplementProjectileComp.InitializeProjectile(newOwner, config.Damage, false);
            return supplementProjectileComp;
        }

        return null;
    }
}