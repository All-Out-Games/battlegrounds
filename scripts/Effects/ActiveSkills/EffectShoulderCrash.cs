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
    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => false;

    protected override bool PreventMovement => true;
    protected override string InvincibilityReason => "ShoulderCrash";
    
    protected float NextDmgTick = 1;
    protected bool Ticked = false;


    public override void OnEffectStart()
    {
        base.OnEffectStart();
        AssignConfig(EffectConfig.ShoulderCrashConfig.GetDefault(FightPlayer.CurrentAttack));
        DurationRemaining = _config.DashDuration + 0.1f;
        //FightPlayer.AddPlayerCollisionFunction(OnShoulderCrashCollision);

        Vector2 dir = GetDashDirection();
        _interactedEntity.Add(Player.Entity);
        
        FightPlayer.AddDash(dir * _config.DashSpeed, _config.DashDuration);
        // The player is invincible and not allowed to input movement during the dash
        FightStateMachine.SetTrigger("shoulder_crash");
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        //FightPlayer.RemovePlayerCollisionFunction(OnShoulderCrashCollision);
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

        FightPlayer otherPlayer = other.GetComponent<FightPlayer>();
        if (otherPlayer != null)
        {
            ShoulderCrashDamage(otherPlayer);
        }
    }

    private void ShoulderCrashDamage(FightPlayer otherPlayer)
    {
        Vector2 bumpDir = otherPlayer.Entity.Position - Entity.Position;
        var add = bumpDir.Normalized * _config.BumpStrength;
        otherPlayer.AddBumpFrom(FightPlayer, add, false);
        FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.ContactDamage) with{ InterruptLevel = FightPlayer.DamageInfo.KnockBackInterruptLevel};
        otherPlayer.TakeDamage(FightPlayer, info);
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        foreach (var fp in FightClubGameManager.Instance.OverlapCircleForCombatPlayers(Entity.Position, 1.5f))
        {
            if (!_interactedEntity.Contains(fp.Entity))
            {
                ShoulderCrashDamage(fp);
                _interactedEntity.Add(fp.Entity);
            }
                
        }
    }
}