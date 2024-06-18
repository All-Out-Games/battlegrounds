using AO;
using Assembly.scripts.Effects;

namespace Assembly.scripts.SceneObjects.Projectiles;

public class BefuddleProjectile : BaseProjectile
{
    public float ConfusionTime = 3f;
    public float ConfusionIntensity = 125f;
    protected override void DoProjectileEffect(Entity other, bool predicted)
    {
        
        FightPlayer fp = other.GetComponent<PlayerCollisionChild>()?.Player;
        if (fp != null)
        {
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Damage, DamageType.Ranged);
            info.ReactionInfo.Flinch = false;
            fp.TakeDamage(Owner, info);
            fp.AddEffect<EffectConfusion>(Owner, ConfusionTime, confusion => confusion.ConfusionIntensity = ConfusionIntensity);
            if (!Pierce)
            {
                Entity.Destroy();
            }
        
        }
    }
}