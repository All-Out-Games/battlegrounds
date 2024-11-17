using AO;
using StreamReader = AO.StreamReader;
using StreamWriter = AO.StreamWriter;

namespace Assembly.scripts.SceneObjects;

/// <summary>
/// Components that has a owner.
/// Implement OnOwnerLeave to destroy entity when the player leaves.
/// </summary>
public abstract class OwnedObjectComponent : Component, INetworkedComponent
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
    public virtual void NetworkSerialize(StreamWriter writer)
    {
        writer.WriteNetworkedEntity(Owner.Entity);
    }

    public virtual void NetworkDeserialize(StreamReader reader)
    {
        var ownerEntity = reader.ReadNetworkedEntity();
        if (ownerEntity.Alive())
        {
            Owner = ownerEntity.GetComponent<FightPlayer>();
        }
    }
}