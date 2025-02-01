using Assembly.scripts.SceneObjects;
using Assembly.scripts.SceneObjects.Projectiles;
using Assembly.scripts.VFX;

namespace Assembly.scripts.Effects.ActiveSkills;
using AO;

public class AbilityIceStorm : FightAbility
{
    public override string SkillKey => "IceStorm";

    public override Type Effect => typeof(EffectIceStorm);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    
    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        float cd = EffectConfig.IceStormConfig.Cooldown;
        int lv = fp.GetSkillTree().GetSkillLevel("IceStorm");
        if (lv > 3)
        {
            cd -= 2;
        }

        return cd;
    }
}

public class EffectIceStorm : FightEffectWithNoFlinch
{
    public override bool IsActiveEffect => true;

    public override bool BlockAbilityActivation => true;

    public override float SpeedModifier => _endAnimationPlayed ? 0.15f : 1f;

    private bool _endAnimationPlayed;

    private EffectConfig.IceStormConfig _config;
    private EffectConfig.ProjectileConfig _chunkConfig;
    
    protected float NextDmgTick = 0.5f;
    protected bool Ticked = false;
    protected bool LoopAudioPlayed = false;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        _config = EffectConfig.IceStormConfig.GetDefault(FightPlayer.CurrentAttack, FightPlayer.GetSkillTree().GetSkillLevel("IceStorm"));
        _chunkConfig = EffectConfig.ProjectileConfig.GetPlayerIceChunkConfig(_config.ProjectileDamage);
        if (!isDropIn)
        {
            SFX.Play(SFXKeys.IceStormStartAudio, DefaultSoundDesc);
            DurationRemaining = EffectConfig.IceStormConfig.EndAnimationTime + 1f;
        }
        FightPlayer.SetAnimTrigger("ice_storm_start");
        FightPlayer.RegisterSpeedModify(this);
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if(Util.OneTime(ElapsedTime > EffectConfig.IceStormConfig.EndAnimationTime, ref _endAnimationPlayed))
        {
            FightPlayer.SetAnimTrigger("ice_storm_end");
            SFX.Play(SFXKeys.IceStormEndAudio, DefaultSoundDesc);
            SFX.FadeOutAndStop(SoundId, 1);
        }
        
        if (Util.OneTime(ElapsedTime > NextDmgTick, ref Ticked))
        {
            if (!LoopAudioPlayed)
            {
                SoundId = SFX.Play(SFXKeys.IceStormLoopAudio,
                    DefaultSoundDesc with { Loop = true, LoopTimeout = DurationRemaining });
            }
            NextDmgTick += 1;
            Ticked = false;
            LoopAudioPlayed = true;
            if (!_endAnimationPlayed)
            {
                IceAttack();
            }
            
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.RemoveSpeedModify(this);
        SFX.Stop(SoundId);
    }

    private void IceAttack()
    {
        // Part I: Small AoE with slow and ice damage
        Vector2 selfPos = FightPlayer.Entity.Position;

        FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.Damage, DamageType.AOE);
        info.SkillKey = SkillConfig.IceStormNodeConfig.SkillKey;
        info.CrateImmediateDestroy = true;
        
        var damageables = FightClubGameManager.Instance.OverlapCircleForDamageables(selfPos, EffectConfig.IceStormConfig.AoeRange, Player);

        bool hit = false;
        foreach (var dmg in damageables)
        {
            if(!dmg.Damageable()) continue;
            
            dmg.TakeDamage(FightPlayer, info);

            if (dmg is PlayerCollisionChild fp)
            {
                var other = fp.Player;

                //other.AddEffect<EffectKnockDown>(FightPlayer, EffectConfig.LeapSlamConfig.KnockDownTime + 0.5f);
                other.AddEffect<EffectMovementSpeedChange>(FightPlayer, 1.1f, change => change.SpdModifier = EffectConfig.IceStormConfig.PlayerSpeedMultiplier);
                hit = true;
                FightClubGameManager.Instance.ClientSpawn(VFXPrefabs.HitVFX, other.Position with{ Y = other.Position.Y + 0.2f},
                    entity =>
                    {
                        SelectionVFX vfx = entity.GetComponent<SelectionVFX>();
                        vfx.StartVFX("hit_ice", false);
                    }
                );
            }
        }

        if (hit)
        {
            SFX.Play(SFXKeys.IceHitAudio, DefaultSoundDesc);
        }
        // Part II: Ice chunks projectile - they are essentially spoons with a skin

        float range = EffectConfig.ProjectileConfig.SpoonRange;
        var targetPlayers = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(selfPos, range, Player);

        for (int i = 0; i < _config.LockTarget; i++)
        {
            Vector2 chunkStartDir, chunkStartPos;
            
            if (i < targetPlayers.Count && targetPlayers[i].Alive())
            {
                chunkStartDir = (targetPlayers[i].Position - selfPos).Normalized;
            }
            else
            {
                chunkStartDir = new Vector2(Random.Shared.NextFloat()-0.5f, Random.Shared.NextFloat()-0.5f).Normalized;
            }
            chunkStartPos = selfPos + chunkStartDir * range;

            Entity proj = Game.SpawnProjectile(FightPlayer, _chunkConfig.ProjectilePrefabKey,
                $"{_chunkConfig.ProjectilePrefabKey}",
                chunkStartPos, -chunkStartDir);
                
            Projectile projComp = proj.GetComponent<Projectile>();
            projComp.Speed = _chunkConfig.Speed;
            projComp.Lifetime = _chunkConfig.ProjectileLifetime;
            BaseProjectile supplementProjectileComp = proj.GetComponent<BaseProjectile>();
            supplementProjectileComp.LifeTime = _chunkConfig.ProjectileLifetime;
            supplementProjectileComp.InitializeProjectile(FightPlayer, _chunkConfig.Damage, false);
            
        }
    }
}