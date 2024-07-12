using AO;

namespace Assembly.scripts.SceneObjects;

public abstract class OwnedObjectComponent : Component
{
    [Serialized] protected FightPlayer Owner;

    public abstract void OnOwnerLeave(FightPlayer player);

    public override void Start()
    {
        base.Start();
        FightClubGameManager.Instance.PlayerLeaveEvent += OnOwnerLeave;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        FightClubGameManager.Instance.PlayerLeaveEvent -= OnOwnerLeave;
    }
}