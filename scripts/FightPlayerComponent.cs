using AO;
using StreamReader = AO.StreamReader;
using StreamWriter = AO.StreamWriter;

/// <summary>
/// Base class for components added to FightPlayer
/// Wrapped in more functionality to improve readability
/// </summary>
public class FightPlayerComponent : Component, INetworkedComponent
{
    
    protected FightPlayer _player;

    public override void Awake()
    {
        _player = Entity.GetComponent<Player>() as FightPlayer;
        //Log.Info($"Set player: {_player.Name}");
    }

    public FightPlayer GetFightPlayer()
    {
        return _player;
    }

    public virtual void NetworkSerialize(StreamWriter writer)
    {
        
    }

    public virtual void NetworkDeserialize(StreamReader reader)
    {
        
    }
}