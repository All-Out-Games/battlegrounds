namespace Assembly.scripts.SceneObjects.Projectiles;
using AO;

public class IllusionWaveProjectile : BaseProjectile
{

    private Spine_Animator _animator;

    public override bool Blockable => false;
    public override bool Pierce => true;

    public override void Awake()
    {
        base.Awake();
        _animator = Entity.GetComponent<Spine_Animator>();
        if (_animator != null)
        {
            _animator.Awaken();
            var instance = _animator.SpineInstance;
            instance.SetAnimation("loop", true);
            SoundId = SFX.Play(SFXKeys.IllusionSlashAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity, Loop = false});
        }
        else
        {
            Log.Error("No Animator Found on projectile");
        }
    }
    protected override void DoProjectileEffect(Entity other, bool predicted)
    {
        DamageableObject fp = other.GetComponent<DamageableObject>();
        if (fp != null && fp.Damageable())
        {
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Damage, DamageType.Ranged);

            info.SkillKey = SkillConfig.BladeFrenzyConfig.SkillKey;
            info.CrateImmediateDestroy = true;
            
            fp.TakeDamage(Owner, info);
            if (!Pierce)
            {
                Entity.Destroy();
            }
            
        }
    }
}