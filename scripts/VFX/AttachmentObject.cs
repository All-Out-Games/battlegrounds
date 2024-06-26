using AO;

namespace Assembly.scripts.VFX;

public class AttachmentObject : Component
{
    
    // Objects that attach to the player. Offset uses player local coord.
    // After the lifetime, it will destroy itself.
    [Serialized] protected float EntityLifeTime;
    protected bool LifeTimeEnded;
    protected float LifeTime;
    
    public void Despawn()
    {
        if(Network.IsServer) Network.Despawn(Entity);
        Entity.Destroy();
    }

    // Call after instantiate
    public void Spawn(Entity playerEntity, Vector2 offset, bool networked, float lifetime)
    {
        Entity.Position = playerEntity.Position + offset;
        Entity.SetParent(playerEntity, true);
        EntityLifeTime = lifetime;
        if (networked)
        {
            Network.Spawn(Entity);
        }
    }

    public override void Update()
    {
        base.Update();
        if (Util.OneTime(LifeTime > EntityLifeTime, ref LifeTimeEnded))
        {
            Despawn();
            //Log.Warn($"Entity {Entity.Name} Destroyed!");
        }

        LifeTime += Time.DeltaTime;
    }
}