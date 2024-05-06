using AO;

public class EffectRollOut : AEffect
{
    private FightPlayer _player;
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
        _player = (FightPlayer)Player;
        _player.AddSpeedModifier(_config.SpeedBuffMultiplier);
        _player.AddPlayerCollisionFunction(OnRolloutCollision);
        
    }

    public override void OnEffectEnd(bool interrupt)
    {
        _player.RemoveSpeedModifier(_config.SpeedBuffMultiplier);
        _player.RemovePlayerCollisionFunction(OnRolloutCollision);
    }
    
    

    public override bool IsActiveEffect { get; }
    public override bool BlockAbilityActivation { get; }
    public override bool IsValidTarget { get; }

    protected void OnRolloutCollision(Entity other)
    {
        FightPlayer otherPlayer = other.GetComponent<FightPlayer>();
        if (otherPlayer != null && Network.IsServer)
        {
            Vector2 bumpDir = other.Position - Entity.Position;
            otherPlayer.CallClient_AddBump(bumpDir * _config.BumpStrength, false);
        }
    }
}