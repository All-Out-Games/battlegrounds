using AO;

namespace Assembly.scripts.Koh;

public class KohManager : Component
{
    public static KohManager _instance;
    public static KohManager Instance
    {
        get
        {
            if (!_instance.Alive())
            {
                foreach (var c in Scene.Components<KohManager>())
                {
                    _instance = c;
                    _instance.Awaken();
                    break;
                }
            }
            return _instance;
        }
    }
    
    private SyncVar<int> _currentState = new();
    public GameState State
    {
        get => (GameState)_currentState.Value;
        set => _currentState.Set((int)value);
    }

    
}

public enum GameState
{
    WaitingForPlayers,
    CountingDown,
    StartRound,
    Round,
    RoundEnd
}
