using AO;

/// <summary>
/// Base class for components added to FightPlayer
/// Wrapped in more functionality to improve readability
/// </summary>
public class FightPlayerComponent : Component
{
    
    protected FightPlayer _player;

    public void AssignPlayer(FightPlayer pl)
    {
        _player = pl;
    }

    public FightPlayerComponent(FightPlayer pl)
    {
        _player = pl;
    }

    public FightPlayerComponent()
    {
        
    }
}