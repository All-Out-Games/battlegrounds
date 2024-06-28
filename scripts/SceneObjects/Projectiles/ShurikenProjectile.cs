using AO;
using Assembly.scripts.VFX;

namespace Assembly.scripts.SceneObjects.Projectiles;

public class ShurikenProjectile : BaseProjectile
{
    public float BackDamageMultiplier = 1.0f;
    
    private Spine_Animator _animator;
    
    public override void Start()
    {
        base.Start();
        _animator = Entity.GetComponent<Spine_Animator>();
        if (_animator != null)
        {
            var instance = _animator.SpineInstance;
            instance.SetSkin("shuriken");
            instance.EnableSkin("shuriken");
            instance.SetAnimation("spin_loop", true);
        }
        else
        {
            Log.Error("No Animator Found on projectile");
        }
    }
    protected override void DoProjectileEffect(Entity other, bool predicted)
    {
        FightPlayer fp = other.GetComponent<PlayerCollisionChild>()?.Player;
        if (fp is { CurrentHealth: > 0 })
        {
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Damage, DamageType.Ranged);
            if (predicted)
            {
                info.ReactionInfo.Flinch = false;
            }
            
            Vector2 dir = other.Position - Entity.Position;
            if (Vector2.Dot(dir, fp.GetFacingDirection() ? Vector2.Right : Vector2.Left) >= 0)
            {
                info.ReactionInfo.Amount = (int) float.Floor(Damage * BackDamageMultiplier);
            }
            fp.TakeDamage(Owner, info);
            
            if (!Pierce)
            {
                Entity.Destroy();
            }
            FightClubGameManager.Instance.ClientSpawn(VFXPrefabKeys.HitVfxPath, Vector2.Lerp(other.Position, Entity.Position, 0.5f));
        }
    }
}