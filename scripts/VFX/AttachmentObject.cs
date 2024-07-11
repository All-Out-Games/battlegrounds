using AO;

namespace Assembly.scripts.VFX;

public class AttachmentObject : Component
{
    
    // Objects that attach to the player. Offset uses player local coord.
    // After the lifetime, it will destroy itself.
    [Serialized] protected float EntityLifeTime;
    protected bool LifeTimeEnded;
    protected float TimeElapsed;
    protected bool Networked;
    
    public void Despawn()
    {
        if(Network.IsServer && Networked) Network.Despawn(Entity);
        Entity.Destroy();
    }

    // Call after instantiate
    public void Spawn(Entity playerEntity, Vector2 offset, bool networked, float lifetime)
    {
        Networked = networked;
        Entity.Position = playerEntity.Position + offset;
        Entity.SetParent(playerEntity, true);
        EntityLifeTime = lifetime;
        if (Network.IsServer && networked)
        {
            Network.Spawn(Entity); 
            // Note: Most of the time you don't need networked = true
            // Because Effects are synced automatically and they will spawn the attachment object, if they need one.
            // See EffectRage.cs for example
        }
    }

    public override void Update()
    {
        base.Update();
        if (Util.OneTime(TimeElapsed > EntityLifeTime, ref LifeTimeEnded))
        {
            Despawn();
            Log.Warn($"Entity {Entity.Name} Destroyed!, Lifetime = {EntityLifeTime}");
        }

        TimeElapsed += Time.DeltaTime;
    }
}