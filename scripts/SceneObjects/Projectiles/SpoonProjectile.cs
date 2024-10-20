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
            fp.TakeDamage(Owner, info);
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
            SFX.Play(SFXKeys.SpoonHitAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity });

        }
    }
}