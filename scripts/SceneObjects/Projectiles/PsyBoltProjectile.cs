using Assembly.scripts.VFX;

namespace Assembly.scripts.SceneObjects.Projectiles;

using AO;

public class PsyBoltProjectile : BaseProjectile
{
    public float KnockBackStrength = 20f;
    
    private Spine_Animator _animator;
    public override void Awake()
    {
        base.Awake();
        _animator = Entity.GetComponent<Spine_Animator>();
        if (_animator != null)
        {
            _animator.Awaken();
            var instance = _animator.SpineInstance;
            instance.SetSkin("psybolt");
            instance.EnableSkin("psybolt");
            instance.SetAnimation("fly_straight", true);
            SoundId = SFX.Play(SFXKeys.PsyboltLoopAudio, new SFX.PlaySoundDesc() {EntityToFollow = Entity, Loop = true, LoopTimeout = 3f});
            
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
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Damage, DamageType.Ranged) with {InterruptLevel = 2000};
            info.SkillKey = SkillConfig.PsyboltConfig.SkillKey;
            fp.TakeDamage(Owner, info);
            Vector2 dir = other.Position - Entity.Position;
            fp.AddBumpFrom(Owner, dir * KnockBackStrength, false);
            //fp.AddBumpFrom(Owner, EngineProjectile.Direction, false);
            
            if (!Pierce)
            {
                Entity.Destroy();
            }
            
            FightClubGameManager.Instance.ClientSpawn(VFXPrefabKeys.HitVfxPath, Vector2.Lerp(other.Position, Entity.Position, 0.5f),
                entity =>
                {
                    SelectionVFX vfx = entity.GetComponent<SelectionVFX>();
                    vfx.StartVFX("hit_psybolt", false);
                    SFX.Play(SFXKeys.PsyboltHitAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity});
                }
            );
        }
    }
    
}