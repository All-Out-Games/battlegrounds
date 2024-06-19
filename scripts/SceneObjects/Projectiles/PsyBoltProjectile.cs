namespace Assembly.scripts.SceneObjects.Projectiles;

using AO;

public class PsyBoltProjectile : BaseProjectile
{
    public float KnockBackStrength = 20f;
    
    protected override void DoProjectileEffect(Entity other, bool predicted)
    {
        
        FightPlayer fp = other.GetComponent<PlayerCollisionChild>()?.Player;
        if (fp != null)
        {
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Damage, DamageType.Ranged) with {InterruptLevel = 2000};
            fp.TakeDamage(Owner, info);
            Vector2 dir = other.Position - Entity.Position;
            fp.AddBumpFrom(Owner, dir * KnockBackStrength, false);
            //fp.AddBumpFrom(Owner, EngineProjectile.Direction, false);
            
            if (!Pierce)
            {
                Entity.Destroy();
            }
        
        }
    }
}