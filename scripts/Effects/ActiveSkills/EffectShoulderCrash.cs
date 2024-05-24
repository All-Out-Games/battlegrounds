using AO;

public class EffectShoulderCrash : FightEffect
{
    private EffectConfig.ShoulderCrashConfig _config;
    private string _skillSlotKey;

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
        FightPlayer.AddPlayerCollisionFunction(OnShoulderCrashCollision);
        FightPlayer.AddDash_Server(GetDashDirection() * _config.DashSpeed, _config.DashDuration);
    }
    
    public override void OnEffectEnd(bool interrupt)
    {
        FightPlayer.RemovePlayerCollisionFunction(OnShoulderCrashCollision);
        _interactedEntity = null;
    }

    public override bool IsActiveEffect { get; }
    public override bool BlockAbilityActivation { get; }
    public override bool IsValidTarget { get; }

    public void AssignConfig(EffectConfig.ShoulderCrashConfig cfg, string slotKey)
    {
        _config = cfg;
        _skillSlotKey = slotKey;
    }

    protected Vector2 GetDashDirection()
    {
        //Log.Debug(FightPlayer.LastInputs.ToString());
        // Use input direction if we have one; use face direction otherwise.
        return FightPlayer.Velocity.Length > 0.01 ? FightPlayer.Velocity.Normalized :
            FightPlayer.GetFacingDirection() ? Vector2.Right : Vector2.Left;
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
                otherPlayer.TakeDamage(_config.ContactDamage);
            }

        }
    }
}