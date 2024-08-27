using AO;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.VFX;

namespace Assembly.scripts.Effects.ActiveSkills;


public class AbilityLeapSlam : FightAbility
{
    public override string SkillKey => "LeapSlam";

    public override Type Effect => typeof(EffectLeapSlam);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => 4f;

    public override float Cooldown =>  GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        float cd = EffectConfig.LeapSlamConfig.Cooldown;
        int lv = fp.GetSkillTree().GetSkillLevel("LeapSlam");
        if (lv > 2)
        {
            cd -= 1;
        }

        if (lv > 3)
        {
            cd -= 1;
        }

        return cd;
    }
}

public class EffectLeapSlam : FightEffectWithImmunity
{
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => false;

    protected override bool PreventMovement => true;

    protected override string InvincibilityReason => "LeapSlam";

    private EffectConfig.LeapSlamConfig _config;
    private Vector2 _dirPosition;
    
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);

        FightPlayer.AddBump(Vector2.Zero, true);

        AssignConfig(EffectConfig.LeapSlamConfig.GetDefault(FightPlayer.CurrentAttack, FightPlayer.GetSkillTree().GetSkillLevel("LeapSlam")));

        _dirPosition = GetDashDirection();
        FightPlayer.SetFacingDirection(_dirPosition.X > 0);
        FightPlayer.SpineAnimator.OnEvent += OnAnimationEvent;
        
        FightPlayer.SetAnimTrigger("leapslam");
        
        DurationRemaining = MainLayer.GetCurrentStateLength();
        FightPlayer.AddDash(_dirPosition * EffectConfig.LeapSlamConfig.LeapMomentum, DurationRemaining);
        SoundId = SFX.Play(SFXKeys.LeapSlamAudio, DefaultSoundDesc);
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.SetFacingDirection(_dirPosition.X > 0);
        FightPlayer.SpineAnimator.OnEvent -= OnAnimationEvent;
    }

    public override void OnAnimationEvent(string evt)
    {
        if (evt == "Attack")
        {
            FightClubGameManager.Instance.ClientSpawn(VFXPrefabKeys.LeapSlamCraterVfxPath, FightPlayer.Entity.Position, entity => entity.LocalScale *= _config.SlamAreaMultiplier);
            SlamDamage();
            FightPlayer.SetFacingDirection(_dirPosition.X > 0);
        }
    }

    public void AssignConfig(EffectConfig.LeapSlamConfig cfg)
    {
        _config = cfg;
    }

    protected Vector2 GetDashDirection()
    {
        return AbilityPositionOrDirection;
    }
    
    public void SlamDamage()
    {
        //Log.Debug($"SLAM!");
        Vector2 selfPos = FightPlayer.Entity.Position;
        FightPlayer.AddDash(Vector2.Zero, 0); // Remove Dash
        FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.SlamDamage, DamageType.AOE, FightPlayer.DamageInfo.KnockBackInterruptLevel);
        info.SkillKey = SkillConfig.LeapSlamConfig.SkillKey;
        var cbPlayers = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(selfPos, EffectConfig.LeapSlamConfig.SlamRadius * _config.SlamAreaMultiplier, Player);
        bool hit = false;
        foreach (var other in cbPlayers)
        {
            if(other == FightPlayer || !other.Damageable()) continue;
            
            other.TakeDamage(FightPlayer, info);
            Vector2 dir = other.Entity.Position - selfPos;
            other.AddBump(dir.Normalized * _config.BumpStrength, false);
            if (Vector2.Dot(dir, other.GetFacingDirectionAsVector()) > 0)
            {
                other.SetFacingDirection(!other.GetFacingDirection());
            }

            //other.AddEffect<EffectKnockDown>(FightPlayer, EffectConfig.LeapSlamConfig.KnockDownTime + 0.5f);
            other.GetEffectMgr().AddLeapSlamKnockdown(FightPlayer.Entity, EffectConfig.LeapSlamConfig.KnockDownTime + 0.5f);
            hit = true;
        }

        if (hit)
        {
            SFX.Play(SFXKeys.LeapSlamKnockAudio, DefaultSoundDesc);
        }
    }
}

public class EffectKnockDown : FightEffectWithNoFlinch
{
    
    protected override bool PreventMovement => true;

    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;

    protected override int InterruptLevel => FightPlayer.DamageInfo.KnockBackInterruptLevel;

    private bool _gettingup = false;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.SetAnimTrigger("sentfly");
        FightPlayer.SpineAnimator.OnAnimationEnd += OnAnimationEnd;
        FightPlayer.OnReceiveDamage += OnDamageEvent;
    }
    
    public void OnAnimationEnd(string ani)
    {
        if (ani == "BAT_003/sent_flying_land")
        {
            FightPlayer.RemoveEffect<EffectKnockDown>(false);
        }

    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if (Util.OneTime(ElapsedTime > EffectConfig.LeapSlamConfig.KnockDownTime, ref _gettingup))
        {
            FightPlayer.SetAnimTrigger("sentfly_end");
            DurationRemaining = MainLayer.GetCurrentStateLength();
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.OnReceiveDamage -= OnDamageEvent;
        FightPlayer.SpineAnimator.OnAnimationEnd -= OnAnimationEnd;
        FightPlayer.SetAnimTrigger("RESET", NeedALReset());
    }
}