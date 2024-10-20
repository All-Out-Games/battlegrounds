using AO;
using Assembly.scripts.VFX;

namespace Assembly.scripts.SceneObjects.Projectiles;

public class ShurikenProjectile : BaseProjectile
{
    public float BackDamageMultiplier = 1.0f;
    public bool Enhanced = false;
    
    private Spine_Animator _animator;
    
    public override void Awake()
    {
        base.Awake();
        _animator = Entity.GetComponent<Spine_Animator>();
        if (_animator != null)
        {
            _animator.Awaken();
            var instance = _animator.SpineInstance;
            instance.SetSkin("shuriken");
            instance.EnableSkin("shuriken");
            instance.SetAnimation("spin_loop", true);
            SoundId = SFX.Play(SFXKeys.ShurikenLoopAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity, Loop = true, LoopTimeout = 3f});
            Log.Warn($"Sound Start ID = {SoundId}");
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
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Damage, DamageType.Ranged);
            Vector2 dir = other.Position - Entity.Position;
            if (Vector2.Dot(dir, fp.GetFacingDirection() ? Vector2.Right : Vector2.Left) >= 0)
            {
                info.ReactionInfo.Amount = (int) float.Floor(Damage * BackDamageMultiplier);
                info.DamageNumberColor = GlobalData.CritNumberColor; // Orange
            }

            info.SkillKey = SkillConfig.ShurikenConfig.SkillKey;
            fp.TakeDamage(Owner, info);
            
            if (Enhanced) // bounce
            {
                EffectConfig.ProjectileConfig config = EffectConfig.ProjectileConfig.GetPlayerShurikenConfig(Owner.CurrentAttack, 1);
                Vector2 bounceDir = Vector2.Rotate(dir.Normalized, 1.57f, Vector2.Zero).Normalized;
                
                Entity proj = Game.SpawnProjectile(Owner, config.ProjectilePrefabKey,
                    config.ProjectilePrefabKey,
                    Entity.Position, bounceDir);
                Projectile projComp = proj.GetComponent<Projectile>();
                projComp.Speed = config.Speed;
                projComp.Lifetime = config.ProjectileLifetime;
            
                ShurikenProjectile supplementProjectileComp = proj.GetComponent<ShurikenProjectile>();
                supplementProjectileComp.LifeTime = config.ProjectileLifetime;
                supplementProjectileComp.InitializeProjectile(Owner, config.Damage, false);
                supplementProjectileComp.BackDamageMultiplier = EffectConfig.ProjectileConfig.ShurikenBackDamageModifier;
                supplementProjectileComp.AddIgnoredPlayer(fp); // Don't hit the same player again
                
            }
            
            if (!Pierce)
            {
                Entity.Destroy();
            }

            FightClubGameManager.Instance.ClientSpawn(VFXPrefabs.HitVFX, Vector2.Lerp(other.Position, Entity.Position, 0.5f),
                entity =>
                {
                    SelectionVFX vfx = entity.GetComponent<SelectionVFX>();
                    vfx.StartVFX("hit_generic", false);
                }
            );
            SFX.Play(SFXKeys.ShurikenHitAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity });
        }
    }
}