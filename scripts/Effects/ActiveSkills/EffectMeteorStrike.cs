using Assembly.scripts.SceneObjects;
using Assembly.scripts.SceneObjects.TriggersAndInteractions;

namespace Assembly.scripts.Effects.ActiveSkills;
using AO;

public class AbilityMeteorStrike : FightAbility
{
    public override string SkillKey => "MeteorStrike";
    public override Type Effect => typeof(EffectMeteorStrike);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        return fp.GetSkillTree().GetSkillLevel("MeteorStrike") > 2
            ? EffectConfig.MeteorStrikeConfig.Cooldown - 4
            : EffectConfig.MeteorStrikeConfig.Cooldown;
    }
}

public class EffectMeteorStrike : FightEffectWithImmunity
{
    public override bool BlockAbilityActivation => true;
    public override bool IsActiveEffect => true;
    protected override string InvincibilityReason => "Meteor";

    private EffectConfig.MeteorStrikeConfig _config;

    private bool _beginFalling = false;

    private Entity _spineEntity;

    private float _curYOffset;
    private float _originalY;
    
    private CameraControl _localControl;
    private float _defaultZoom;

    private ReticleObject _meteorReticle;
    private string _meteorReticlePrefabPath = "MeteorReticle.prefab";
    private string _meteorCraterPrefabPath = "MeteorCrater.prefab";

    public override float SpeedModifier => _beginFalling ? _config.FallSpeedMultiplier : 0.25f;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        
        FightPlayer.RegisterSpeedModify(this);
        _defaultZoom = CameraControl.GetCurrent().Zoom;
        // Effect has invincibility but also temporarily puts player out of combat.
        FightPlayer.PlayerStatus = PlayerStatus.TemporarilyOutCombat;
        FightPlayer.SetAnimTrigger("meteor_start", true);
        _spineEntity = FightPlayer.SpineAnimator.Entity; // This can be used to offset height
        _config = EffectConfig.MeteorStrikeConfig.GetDefault(FightPlayer.CurrentAttack,
            FightPlayer.GetSkillTree().GetSkillLevel("MeteorStrike"));

        if (!isDropIn)
        {
            DurationRemaining = 0.1f + EffectConfig.MeteorStrikeConfig.JumpDuration +
                                EffectConfig.MeteorStrikeConfig.FallDuration;
            SFX.Play(SFXKeys.MeteorStartAudio, DefaultSoundDesc);
        }

        if (FightPlayer.IsLocal)
        {
            // Camera
            _localControl = CameraControl.Create(2);
            _localControl.Position = FightPlayer.Position;
        }
        
        // Reticle
        FightClubGameManager.Instance.ClientSpawn(_meteorReticlePrefabPath,FightPlayer.Position
            ,
            entity =>
            {
                _meteorReticle = entity.GetComponent<ReticleObject>();
                entity.SetParent(FightPlayer.Entity, true);
                _meteorReticle.PlayReticleLerpAnimation(Vector2.Zero, new Vector2(5,5), EffectConfig.MeteorStrikeConfig.JumpDuration);
            }
        );

        
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if (Util.OneTime(ElapsedTime > EffectConfig.MeteorStrikeConfig.JumpDuration, ref _beginFalling))
        {
            _curYOffset = _spineEntity.LocalPosition.Y + 10f;
            FightPlayer.SetAnimTrigger("meteor_loop");
            _originalY = _spineEntity.LocalPosition.Y;
            _spineEntity.LocalPosition = _spineEntity.LocalPosition with { Y = _curYOffset + _originalY };
            DurationRemaining = EffectConfig.MeteorStrikeConfig.FallDuration;
            SoundId = SFX.Play(SFXKeys.MeteorLoopAudio,
                DefaultSoundDesc with { Loop = true, LoopTimeout = DurationRemaining+1f });
        }

        float prog10 = 0;
        if (_beginFalling)
        {
            prog10 = DurationRemaining / EffectConfig.MeteorStrikeConfig.FallDuration;
            //Log.Info($"{prog10}");
            _curYOffset = Util.Lerp(0, 10f, prog10);
            FightPlayer.SpineAnimator.DepthOffset = -_curYOffset;
            _spineEntity.LocalPosition = _spineEntity.LocalPosition with { Y = _curYOffset + _originalY };
        }
        
        // Camera
        if (!FightPlayer.Alive() && _localControl != null)
        {
            _localControl.Destroy();
            return;
        }

        if (_localControl != null)
        {
            _localControl.Position = Position;
            float zoomPhase = 0;
            
            if (_beginFalling)
            {
                // During fall phase: Remove zoom until default
                // Directly reuse the prog10 up there (1-0)
                _localControl.Zoom = Util.Lerp(_defaultZoom, 2.3f, prog10);
            }
            else
            {
                // Jump phase: Zoom out to give a clear view
                zoomPhase = ElapsedTime / EffectConfig.MeteorStrikeConfig.JumpDuration; // (0-1 in this phase)
                _localControl.Zoom = Util.Lerp(_defaultZoom, 2.3f, zoomPhase);
            }
        }

    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.EnterCombatFromSpectator();
        FightPlayer.RemoveSpeedModify(this);
        
        if (!interrupt)
        {
            FightPlayer.AddEffect<EffectMeteorEnd>(FightPlayer, 1.2f);
        }

        if (FightPlayer.IsLocal)
        {
            _localControl?.Destroy();
            _localControl = null;
            FightPlayer.GetCameraInterface().Shake(0.5f, 1f);
        }

        if (_meteorReticle.Alive())
        {
            _meteorReticle.Entity.Destroy();
        }
        
        FightClubGameManager.Instance.ClientSpawn(_meteorCraterPrefabPath, Position, entity =>
        {
            entity.Scale *= _config.SizeMultiplier;
        });
        
        // Damage (it's basically a super beefed up leap slam)
        SlamDamage();
        
        // Audio
        SFX.FadeOutAndStop(SoundId, 1f);
        SFX.Play(SFXKeys.MeteorLandAudio, DefaultSoundDesc);
        
        FightPlayer.SpineAnimator.DepthOffset = 0;
    }
    
    public void SlamDamage()
    {
        //Log.Debug($"SLAM!");
        Vector2 selfPos = FightPlayer.Entity.Position;
        FightPlayer.AddDash(Vector2.Zero, 0); // Remove Dash
        FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.Damage, DamageType.None, FightPlayer.DamageInfo.KnockBackInterruptLevel); // Not blockable
        info.SkillKey = SkillConfig.MeteorStrikeNodeConfig.SkillKey;
        info.SpecialDeathAnimation = true;
        info.CrateImmediateDestroy = true;
        info.ReactionInfo.Flinch = false;
        
        var damageables = FightClubGameManager.Instance.OverlapCircleForDamageables(selfPos, EffectConfig.MeteorStrikeConfig.BaseRadius * _config.SizeMultiplier, Player);

        foreach (var dmg in damageables)
        {
            if(!dmg.Damageable()) continue;
            
            dmg.TakeDamage(FightPlayer, info);

            if (dmg is PlayerCollisionChild fp)
            {
                var other = fp.Player;
                Vector2 dir = other.Entity.Position - selfPos;
                other.AddBump(dir.Normalized * EffectConfig.MeteorStrikeConfig.BumpStrength, false);
                if (Vector2.Dot(dir, other.GetFacingDirectionAsVector()) > 0)
                {
                    other.SetFacingDirection(!other.GetFacingDirection());
                }
                
                other.GetEffectMgr().AddLeapSlamKnockdown(FightPlayer.Entity, 2f, EffectConfig.LeapSlamConfig.KnockDownTime);
            }
        }
        
        
    }

}



public class EffectMeteorEnd : FightEffect
{
    // This is our spectator mode.
    // Player can roam around and get XP, and they will have an ability that can be used to immediately spawn into the arena
    public override bool IsActiveEffect => true;

    protected override bool PreventMovement => true;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.SetAnimTrigger("meteor_land");
        DurationRemaining = 1.2f;
        // SFX.Play(SFXKeys.SpectralSpawnAudio, DefaultSoundDesc);
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        var trapLst = Scene.Components<BearTrap>();
        foreach (var trap in trapLst)
        {
            trap.RemoveEntityFromWhiteList(FightPlayer.Entity); // We need to do this because the player ignore all collisions during Meteor Crash but they might have contacted a trap
            // which will add them to a whitelist and invalidate the trap for them.
        }
    }
}

