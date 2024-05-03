using AO;

public class FightPlayerComponent : Component
{
    protected FightPlayer _player;
    
    public void AssignPlayer(FightPlayer pl)
    {
        _player = pl;
    }
}