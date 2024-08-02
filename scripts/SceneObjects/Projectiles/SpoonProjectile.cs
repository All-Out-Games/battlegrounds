using AO;
using Assembly.scripts.VFX;

namespace Assembly.scripts.SceneObjects.Projectiles;

public class SpoonProjectile : BaseProjectile
{
    private Spine_Animator _animator;
    public override void Start()
    {
        base.Start();
        _animator = Entity.GetComponent<Spine_Animator>();
        if (_animator != null)
        {
            var instance = _animator.SpineInstance;
            instance.SetSkin("spoon");
            instance.EnableSkin("spoon");
            instance.SetAnimation("fly_straight", true);
            SoundId = SFX.Play(SFXKeys.ProjectileLoopAudio, new SFX.PlaySoundDesc() {EntityToFollow = Entity, Loop = true});
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
        
        }
    }
}