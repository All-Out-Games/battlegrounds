using AO;

/// <summary>
/// Controller of the arena player
/// </summary>
public class FightPlayerController : Component
{
    private FightPlayer _player;
    
    public override void Awake()
    {
        _player = Entity.GetComponent<FightPlayer>();
    }

    public override void Start()
    {

    }
}