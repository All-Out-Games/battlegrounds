namespace Assembly.scripts.SceneObjects.Projectiles;

using AO;

public class PsyBoltProjectile : BaseProjectile
{
    public float KnockBackStrength = 20f;
    
    private Spine_Animator _animator;
    public override void Start()
    {
        base.Start();
        _animator = Entity.GetComponent<Spine_Animator>();
        if (_animator != null)
        {
            var instance = _animator.SpineInstance;
            instance.SetSkin("psybolt");
            instance.EnableSkin("psybolt");
            instance.SetAnimation("fly_straight", true);
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