using AO;

namespace Assembly.scripts.SceneObjects;

/// <summary>
/// Components that has a owner.
/// Implement OnOwnerLeave to destroy entity when the player leaves.
/// </summary>
public abstract class OwnedObjectComponent : Component
{
    [Serialized] protected FightPlayer Owner;

    public override void Update()
    {
        base.Update();
        if (!Owner.Alive())
        {
            Log.Warn($"Owner of {Entity.Name} is not alive. Despawning.");
            Despawn();
        }
    }

    public abstract void Despawn();
}