using AO;
using Assembly.scripts.Effects.ActiveSkills;

namespace Assembly.scripts.SceneObjects.Projectiles;

public partial class BackstabKunaiProjectile : BaseProjectile
{
    private Spine_Animator _animator;
    public override void Awake()
    {
        base.Awake();
        _animator = Entity.GetComponent<Spine_Animator>();
        if (_animator != null)
        {
            var instance = _animator.SpineInstance;
            instance.SetSkin("kunai");
            instance.EnableSkin("kunai");
            instance.SetAnimation("fly_straight", true);
            SoundId = SFX.Play(SFXKeys.ShurikenLoopAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity, Loop = true,LoopTimeout = 3f});
        }
        else
        {
            Log.Error("No Animator Found on projectile");
        }
    }
    protected override void DoProjectileEffect(Entity other, bool predicted)
    {
        FightPlayer fp = other.GetComponent<PlayerCollisionChild>()?.Player;
        if (fp is { CurrentHealth: > 0 } && fp.Damageable())
        {
            if (Network.IsServer)
            {
                CallClient_BackstabPlayer(Owner, fp);
            }

            if (!Pierce)
            {
                Entity.Destroy();
            }
        }
    }

    [ClientRpc]
    public static void BackstabPlayer(FightPlayer shooter, FightPlayer target)
    {
        target.AddEffect<EffectBackstab>(shooter, EffectConfig.BackStabConfig.BackstabTime);
        shooter.AddEffect<EffectBackstabCaster>(target, EffectConfig.BackStabConfig.BackstabTime);
    }
}