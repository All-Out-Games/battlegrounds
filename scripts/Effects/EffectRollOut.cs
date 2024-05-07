using AO;

public class EffectRollOut : FightEffect
{
    private AbilityConfig.RollOutConfig _config;

    public EffectRollOut()
    {
        IsActiveEffect = false;
        BlockAbilityActivation = false;
        IsValidTarget = false;
    }

    /// <summary>
    /// Call this function before adding the created Effect instance to the player!
    /// </summary>
    /// <param name="cfg"></param>
    public void AssignConfig(AbilityConfig.RollOutConfig cfg)
    {
        _config = cfg;
    }
    
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        FightPlayer.AddSpeedModifier(_config.SpeedBuffMultiplier);
        FightPlayer.AddPlayerCollisionFunction(OnRolloutCollision);
        
    }

    public override void OnEffectEnd(bool interrupt)
    {
        FightPlayer.RemoveSpeedModifier(_config.SpeedBuffMultiplier);
        FightPlayer.RemovePlayerCollisionFunction(OnRolloutCollision);
    }
    
    

    public override bool IsActiveEffect { get; }
    public override bool BlockAbilityActivation { get; }
    public override bool IsValidTarget { get; }

    protected void OnRolloutCollision(Entity other)
    {
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
                // TODO: Bumping is changing on the engine side, just do the damage for now (Shin, May07 2024)
                otherPlayer.AddBumpFrom(FightPlayer, add, false);
                //otherPlayer.CallClient_AddBump(add, false);
                otherPlayer.TakeDamage(3);
            }

        }
    }
}