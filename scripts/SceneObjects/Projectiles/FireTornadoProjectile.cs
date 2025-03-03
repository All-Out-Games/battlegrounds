using Assembly.scripts.VFX;

namespace Assembly.scripts.SceneObjects.Projectiles;
using AO;

public class FireTornadoProjectile : BaseProjectile
{
    private Spine_Animator _animator;

    public override bool Blockable => false;
    public override bool Pierce => true;

    public int ProjectileLevel = 1;

    private bool _faded;
    private bool _tracked;
    
    protected float NextTrackingTick = 0.2f;

    public override void Awake()
    {
        base.Awake();
        _animator = Entity.GetComponent<Spine_Animator>();
        if (_animator != null)
        {
            _animator.Awaken();
            var instance = _animator.SpineInstance;
            instance.SetAnimation("idle", true);
            SoundId = SFX.Play(SFXKeys.FireTornadoLoopAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity, Loop = true, LoopTimeout = 3f, Volume = 0.35f, RangeMultiplier = 1.25f});
        }
        else
        {
            Log.Error("No Animator Found on projectile");
        }
    }

    public override void Update()
    {
        base.Update();
        
        // Fading
        if (Network.IsClient)
        {
            Entity.LocalRotation = 0;
            if (Util.OneTime(TimeElapsed + 0.5 > LifeTime, ref _faded))
            {
                SFX.FadeOutAndStop(SoundId, 0.5f);
                var f = GetComponent<FadeAfterStart>();
                if (f.Alive())
                {
                    f.FadeImmediately(0.1f, 0.75f);
                }
            
            }
        }
        
        // Tracking
        if (!_faded)
        {
            if (Util.OneTime(TimeElapsed > NextTrackingTick, ref _tracked))
            {
                NextTrackingTick += 0.2f;
                _tracked = false;
                var target = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(Entity.Position, EffectConfig.FireTornadoConfig.TrackRange, Owner);
                for (int i = 0; i < target.Count; i++)
                {
                    if (target[i].Entity.Alive() && !WhiteList.Contains(target[i].Entity))
                    {
                        //CallClient_TrackDir(target[i].Position);
                        Vector2 dir = target[i].Position - Entity.Position;
                        SteerProjectileTo(dir.Normalized);
                        break;
                    }
                }
            }
        }
    }

    protected override void DoProjectileEffect(Entity other, bool predicted)
    {
        DamageableObject fp = other.GetComponent<DamageableObject>();
        if (fp.Alive() && fp.Damageable())
        {
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Damage, DamageType.Ranged);

            info.SkillKey = SkillConfig.FireTornadoNodeConfig.SkillKey;
            info.CrateImmediateDestroy = true;
            info.SpecialDeathAnimation = true;
            
            fp.TakeDamage(Owner, info);
            if (!Pierce)
            {
                Entity.Destroy();
            }

            if (fp is PlayerCollisionChild fdb)
            {
                var fpp = fdb.Player;
                var mgr = fdb.Player.GetEffectMgr();
                float burnDuration = EffectConfig.FireTornadoConfig.BurnDuration;
                if (ProjectileLevel > 2) burnDuration += 2;
                    
                mgr.AddBurn(Owner.Entity, burnDuration, EffectConfig.FireTornadoConfig.BurnDamage);

                if (ProjectileLevel > 4) // Knockback when fully maxed out
                {
                    Vector2 dir = GetComponent<Rigidbody>().Velocity.Normalized;
                    fpp.AddBump(dir * 175f, false);
                    if (Vector2.Dot(dir, fpp.GetFacingDirectionAsVector()) > 0)
                    {
                        fpp.SetFacingDirection(!fpp.GetFacingDirection());
                    }

                    //other.AddEffect<EffectKnockDown>(FightPlayer, EffectConfig.LeapSlamConfig.KnockDownTime + 0.5f);
                    mgr.AddLeapSlamKnockdown(Owner.Entity, EffectConfig.LeapSlamConfig.KnockDownTime + 0.5f, EffectConfig.LeapSlamConfig.KnockDownTime);
                }
            }
        }
    }
    
    public void SteerProjectileTo(Vector2 dir)
    {
        //Log.Warn($"Tracking - pos {dir.X}, {dir.Y}");
        if (!Entity.Alive())
        {
            return;
        }
        float tStrength = EffectConfig.FireTornadoConfig.TrackAggressiveness;
        if (ProjectileLevel > 3)
        {
            tStrength *= 2;
        }
        Rigidbody rbd = GetComponent<Rigidbody>();
        float speed = rbd.Velocity.Length;
        rbd.Velocity = (rbd.Velocity + dir * tStrength).Normalized * speed;
    }
}