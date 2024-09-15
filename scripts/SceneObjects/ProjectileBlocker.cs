using AO;
using Assembly.scripts.SceneObjects.Projectiles;
using Assembly.scripts.VFX;

namespace Assembly.scripts.SceneObjects;


public class ProjectileBlocker : Component
{
    private Collider _collider;
    public override void Awake()
    {
        base.Awake();
        _collider = GetComponent<Collider>();
        if (_collider == null)
        {
            Log.Warn($"Collider is not found on a Projectile Blocker. {Entity.Name} will be destroyed!");
            Entity.Destroy();
        }
    }

    public virtual void DoBlockerEffect(BaseProjectile other)
    {
        other.Entity.Destroy();
        SFX.Play(SFXKeys.SpoonHitAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity });
        FightClubGameManager.Instance.ClientSpawn(VFXPrefabs.HitVFX, Vector2.Lerp(other.Position, Entity.Position, 0.5f),
            entity =>
            {
                SelectionVFX vfx = entity.GetComponent<SelectionVFX>();
                vfx.StartVFX("hit_generic", false);
            }
        );
    }
}