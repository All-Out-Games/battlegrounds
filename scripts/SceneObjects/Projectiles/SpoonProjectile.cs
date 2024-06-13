using AO;

namespace Assembly.scripts.SceneObjects.Projectiles;

public class SpoonProjectile : BaseProjectile
{
    protected override void DoProjectileEffect(Entity other, bool predicted)
    {
        
        FightPlayer fp = other.GetComponent<PlayerCollisionChild>()?.Player;
        if (fp != null)
        {
            if (Network.IsServer)
            {
                FightPlayer.DamageInfo info = new FightPlayer.DamageInfo() { DmgType = DamageType.Ranged };
                fp.TakeDamage(Damage, Owner, info);
            }
            if (!Pierce)
            {
                Log.Debug("Projectile Destroy");
                Entity.Destroy();
            }
        
        }
    }
}