using AO;
using Assembly.scripts.Effects;

public class AbilityShoulderCrash : FightAbility
{
    public override string SkillKey => "ShoulderCrash";

    public override Type Effect => typeof(EffectShoulderCrash);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => 5f;
    
    public override float Cooldown => EffectConfig.ShoulderCrashConfig.Cooldown;
}
public sealed class EffectShoulderCrash : FightEffect
{
    private EffectConfig.ShoulderCrashConfig _config;
    private List<Entity> _interactedEntity = new List<Entity>();
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => true;
    
    

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        AssignConfig(EffectConfig.ShoulderCrashConfig.GetDefault(FightPlayer.CurrentAttack));
        DurationRemaining = _config.DashDuration + 0.1f;
        FightPlayer.AddPlayerCollisionFunction(OnShoulderCrashCollision);

        Vector2 dir = GetDashDirection();
        FightPlayer.SetFacingDirection(dir.X > 0);
        FightPlayer.AddDash(dir * _config.DashSpeed, _config.DashDuration);
        // The player is invincible and not allowed to input movement during the dash
        FightPlayer.GetEffectMgr().AddEffect<EffectNoMovementWithInvincibility>(FightPlayer, DurationRemaining);
    }

    public override void OnEffectEnd(bool interrupt)
    {
        FightPlayer.RemovePlayerCollisionFunction(OnShoulderCrashCollision);
        _interactedEntity = null;
        FightPlayer.GetEffectMgr().RemoveEffect<EffectNoMovementWithInvincibility>(false);
    }



    public void AssignConfig(EffectConfig.ShoulderCrashConfig cfg)
    {
        _config = cfg;
    }

    protected Vector2 GetDashDirection()
    {
        //Log.Debug(FightPlayer.LastInputs.ToString());
        // Use input direction if we have one; use face direction otherwise.
        return AbilityPositionOrDirection.Normalized;
    }
    
    protected void OnShoulderCrashCollision(Entity other)
    {
        if (_interactedEntity.Contains(other)) return; // Only interact once with each entity
        
        _interactedEntity.Add(other);
        
        FightPlayer otherPlayer = other.GetComponent<FightPlayer>();
        if (otherPlayer != null)
        {
            if(otherPlayer.HasEffect<EffectNoMovement>())
            {
                return;
            }
            Vector2 bumpDir = other.Position - Entity.Position;
            var add = bumpDir * _config.BumpStrength;
            otherPlayer.AddBumpFrom(FightPlayer, add, false);
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.ContactDamage);
            otherPlayer.TakeDamage(FightPlayer, info);
        }
    }
}