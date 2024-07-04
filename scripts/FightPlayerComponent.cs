using AO;

/// <summary>
/// Base class for components added to FightPlayer
/// Wrapped in more functionality to improve readability
/// </summary>
public class FightPlayerComponent : Component
{
    
    protected FightPlayer _player;

    public override void Awake()
    {
        _player = Entity.GetComponent<Player>() as FightPlayer;
        //Log.Info($"Set player: {_player.Name}");
    }
}