using AO;
using Assembly.scripts.Effects;
using Assembly.scripts.VFX;

namespace Assembly.scripts.SceneObjects.Projectiles;

public class BefuddleProjectile : BaseProjectile
{
    public float ConfusionTime = 3f;
    public float ConfusionIntensity = 125f;

    private Spine_Animator _animator;

    public override void Start()
    {
        base.Start();
        _animator = Entity.GetComponent<Spine_Animator>();
        if (_animator != null)
        {
            var instance = _animator.SpineInstance;
            instance.SetAnimation("flying_loop", true);
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
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Damage, DamageType.Ranged) with {InterruptLevel = FightPlayer.DamageInfo.StunInterruptLevel};
            info.ReactionInfo.Flinch = false;
            info.SkillKey = SkillConfig.BefuddleConfig.SkillKey;
            fp.TakeDamage(Owner, info);
            fp.AddEffect<EffectConfusion>(Owner, ConfusionTime, confusion => confusion.ConfusionIntensity = ConfusionIntensity);
            if (!Pierce)
            {
                Entity.Destroy();
            }
            FightClubGameManager.Instance.ClientSpawn(VFXPrefabs.BefuddleHitVFX, Vector2.Lerp(other.Position, Entity.Position, 0.5f));
            SFX.Play(SFXKeys.BefuddleHitAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity });
        }
    }
}