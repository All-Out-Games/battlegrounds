using AO;

public sealed class EffectShoulderCrash : FightEffect
{
    private EffectConfig.ShoulderCrashConfig _config;

    private List<Entity> _interactedEntity;


    EffectShoulderCrash()
    {
        IsActiveEffect = false;
        BlockAbilityActivation = true;
        IsValidTarget = true;
        _interactedEntity = new List<Entity>();
    }

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        AssignConfig(EffectConfig.GetPlayerShoulderCrashConfig(FightPlayer.CurrentAttack));
        DurationRemaining = _config.DashDuration;
        FightPlayer.AddPlayerCollisionFunction(OnShoulderCrashCollision);

        Vector2 dir = GetDashDirection();
        FightPlayer.SetFacingDirection(dir.X > 0);
        FightPlayer.AddDash(dir * _config.DashSpeed, _config.DashDuration);
    }
    
    public override void OnEffectEnd(bool interrupt)
    {
        FightPlayer.RemovePlayerCollisionFunction(OnShoulderCrashCollision);
        _interactedEntity = null;
    }

    public override bool IsActiveEffect { get; }
    public override bool BlockAbilityActivation { get; }
    public override bool IsValidTarget { get; }

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
            if (Network.IsServer)
            {
                otherPlayer.AddBumpFrom(FightPlayer, add, false);
                FightPlayer.DamageReactionInfo info = new FightPlayer.DamageReactionInfo();
                otherPlayer.TakeDamage(_config.ContactDamage, FightPlayer, info);
            }

        }
    }
}