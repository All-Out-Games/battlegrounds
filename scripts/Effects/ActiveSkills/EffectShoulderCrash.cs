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
public sealed class EffectShoulderCrash : FightEffectWithImmunity
{
    private EffectConfig.ShoulderCrashConfig _config;
    private List<Entity> _interactedEntity = new List<Entity>();
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => false;

    protected override bool PreventMovement => true;
    protected override string InvincibilityReason => "ShoulderCrash";


    public override void OnEffectStart()
    {
        base.OnEffectStart();
        AssignConfig(EffectConfig.ShoulderCrashConfig.GetDefault(FightPlayer.CurrentAttack));
        DurationRemaining = _config.DashDuration + 0.1f;
        FightPlayer.AddPlayerCollisionFunction(OnShoulderCrashCollision);

        Vector2 dir = GetDashDirection();
        
        FightPlayer.AddDash(dir * _config.DashSpeed, _config.DashDuration);
        // The player is invincible and not allowed to input movement during the dash
        FightStateMachine.SetTrigger("shoulder_crash");
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.RemovePlayerCollisionFunction(OnShoulderCrashCollision);
        _interactedEntity = null;
        FightStateMachine.SetTrigger("shoulder_crash_end");
    }



    public void AssignConfig(EffectConfig.ShoulderCrashConfig cfg)
    {
        _config = cfg;
    }

    protected Vector2 GetDashDirection()
    {
        return AbilityPositionOrDirection.Normalized;
    }
    
    protected void OnShoulderCrashCollision(Entity other)
    {
        if (_interactedEntity.Contains(other)) return; // Only interact once with each entity
        
        _interactedEntity.Add(other);
        
        FightPlayer otherPlayer = other.GetComponent<FightPlayer>();
        if (otherPlayer != null)
        {
            Vector2 bumpDir = other.Position - Entity.Position;
            var add = bumpDir * _config.BumpStrength;
            otherPlayer.AddBumpFrom(FightPlayer, add, false);
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.ContactDamage) with{ InterruptLevel = FightPlayer.DamageInfo.KnockBackInterruptLevel};
            otherPlayer.TakeDamage(FightPlayer, info);
        }
    }
}