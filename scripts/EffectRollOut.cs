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
    /// <param name="player"></param>
    public void AssignConfig(AbilityConfig.RollOutConfig cfg)
    {
        _config = cfg;
    }
    
    public override void OnEffectStart()
    {
        _player = (FightPlayer)Player;
        _player.AddSpeedModifier(_config.SpeedBuffMultiplier);
        
    }

    public override void OnEffectEnd(bool interrupt)
    {
        _player.RemoveSpeedModifier(_config.SpeedBuffMultiplier);
    }

    public override bool IsActiveEffect { get; }
    public override bool BlockAbilityActivation { get; }
    public override bool IsValidTarget { get; }
}