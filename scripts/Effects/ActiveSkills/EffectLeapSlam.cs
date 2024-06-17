using AO;

namespace Assembly.scripts.Effects.ActiveSkills;


public class AbilityLeapSlam : FightAbility
{
    public override string SkillKey => "LeapSlam";

    public override Type Effect => typeof(EffectLeapSlam);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.CircleAOE;
    public override float MaxDistance => 4f;

    public override float Cooldown => 1; //EffectConfig.LeapSlamConfig.Cooldown;
}

public class EffectLeapSlam : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => false;

    private EffectConfig.LeapSlamConfig _config;
    private Vector2 _dirPosition;
    private Vector2 _originPosition;
    
    private bool _slammed;
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        FightPlayer.AddBump(Vector2.Zero, true);
        _originPosition = FightPlayer.Entity.Position;
        
        AssignConfig(EffectConfig.LeapSlamConfig.GetDefault(FightPlayer.CurrentAttack));
        DurationRemaining = _config.DashDuration + _config.SlamDuration;

        _dirPosition = GetDashDirection();
        FightPlayer.SetFacingDirection(_dirPosition.X > 0);
        _dirPosition += FightPlayer.Entity.Position;

        // The player is invincible and not allowed to input movement during the dash
        FightPlayer.GetEffectMgr().AddEffect<EffectNoMovementWithInvincibility>(FightPlayer, DurationRemaining);
    }

    public override void OnEffectEnd(bool interrupt)
    {
        FightPlayer.GetEffectMgr().RemoveEffect<EffectNoMovementWithInvincibility>(false);
        FightPlayer.AddDash(Vector2.Zero, 0); // Remove Dash
    }

    public override void OnEffectUpdate()
    {

        FightPlayer.Entity.Position = Vector2.Lerp(_originPosition, _dirPosition, DurationProgress01);
        if (Util.OneTime(ElapsedTime > _config.DashDuration, ref _slammed))
        {
            Log.Error("SLAM!");
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
    
}