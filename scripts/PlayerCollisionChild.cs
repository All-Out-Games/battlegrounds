using AO;

public class PlayerCollisionChild : Component
{
    public static List<PlayerCollisionChild> AllChildren = new();

    public FightPlayer Player;

    public override void Start()
    {
        AllChildren.Add(this);
    }

    public override void OnDestroy()
    {
        AllChildren.Remove(this);
    }
}