using AO;
using Assembly.scripts.VFX;

namespace Assembly.scripts.Effects.ActiveSkills;


public class AbilityLeapSlam : FightAbility
{
    public override string SkillKey => "LeapSlam";

    public override Type Effect => typeof(EffectLeapSlam);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => 4f;

    public override float Cooldown =>  EffectConfig.LeapSlamConfig.Cooldown;
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
    
    public override void OnEffectStart()
    {
        base.OnEffectStart();

        FightPlayer.AddBump(Vector2.Zero, true);

        AssignConfig(EffectConfig.LeapSlamConfig.GetDefault(FightPlayer.CurrentAttack));

        _dirPosition = GetDashDirection();
        FightPlayer.SetFacingDirection(_dirPosition.X > 0);
        FightPlayer.SpineAnimator.OnEvent += OnAnimationEvent;
        
        FightPlayer.SetAnimTrigger("leapslam");
        
        DurationRemaining = MainLayer.GetCurrentStateLength();
        FightPlayer.AddDash(_dirPosition * 100f, DurationRemaining);
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.SpineAnimator.OnEvent -= OnAnimationEvent;
    }

    public override void OnAnimationEvent(string evt)
    {
        Log.Debug($"Event {evt}!");
        if (evt == "Attack")
        {
            FightClubGameManager.Instance.ClientSpawn(VFXPrefabKeys.LeapSlamCraterVfxPath, FightPlayer.Entity.Position);
            SlamDamage();
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
        
        var cbPlayers = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(selfPos, _config.SlamRadius);
        foreach (var other in cbPlayers)
        {
            if(other == FightPlayer) continue;
            
            other.TakeDamage(FightPlayer, info);
            Vector2 dir = other.Entity.Position - selfPos;
            other.AddBump(dir.Normalized * _config.BumpStrength, false);
            if (Vector2.Dot(dir, other.GetFacingDirectionAsVector()) > 0)
            {
                other.SetFacingDirection(!other.GetFacingDirection());
            }

            //other.AddEffect<EffectKnockDown>(FightPlayer, EffectConfig.LeapSlamConfig.KnockDownTime + 0.5f);
            other.GetEffectMgr().AddLeapSlamKnockdown(FightPlayer.Entity, EffectConfig.LeapSlamConfig.KnockDownTime + 0.5f);
        }
    }
}

public class EffectKnockDown : FightEffectWithNoFlinch
{
    
    protected override bool PreventMovement => true;
    public override bool BlockAbilityActivation => true;

    protected override int InterruptLevel => FightPlayer.DamageInfo.KnockBackInterruptLevel;

    private bool _gettingup = false;

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        FightPlayer.UnsetAnimTrigger("sentfly_end");
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
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.OnReceiveDamage -= OnDamageEvent;
        FightPlayer.SpineAnimator.OnAnimationEnd -= OnAnimationEnd;
        FightPlayer.SetAnimTrigger("RESET");
        //FightPlayer.SetAnimTrigger("knockdown_end");
    }
}