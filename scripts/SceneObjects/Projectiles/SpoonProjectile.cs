using AO;

namespace Assembly.scripts.SceneObjects.Projectiles;

public class SpoonProjectile : BaseProjectile
{
    protected override void DoProjectileEffect(Entity other, bool predicted)
    {
        
        FightPlayer fp = other.GetComponent<PlayerCollisionChild>()?.Player;
        if (fp != null)
        {
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Damage, DamageType.Ranged);
            fp.TakeDamage(Owner, info);
            if (!Pierce)
            {
                Entity.Destroy();
            }
        
        }
    }
}