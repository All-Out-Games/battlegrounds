using AO;
using Assembly.scripts.Effects.ActiveSkills;

namespace Assembly.scripts.SceneObjects.Projectiles;

public class BackstabKunaiProjectile : BaseProjectile
{
    private Spine_Animator _animator;
    public override void Start()
    {
        base.Start();
        _animator = Entity.GetComponent<Spine_Animator>();
        if (_animator != null)
        {
            var instance = _animator.SpineInstance;
            instance.SetSkin("kunai");
            instance.EnableSkin("kunai");
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
            fp.AddEffect<EffectBackstab>(Owner, EffectConfig.BackStabConfig.BackstabTime);
            Owner.AddEffect<EffectBackstabCaster>(fp, EffectConfig.BackStabConfig.BackstabTime);
            if (!Pierce)
            {
                Entity.Destroy();
            }
        }
    }
}