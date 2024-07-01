using AO;

namespace Assembly.scripts.SceneObjects.TriggersAndInteractions;

/// <summary>
/// Server-spawned object in scene. 
/// </summary>
public class OwnedTrigger : Component
{
    [Serialized] protected Collider TriggerCollider;
    [Serialized] public Spine_Animator Animator;
    
    protected float EntityLifeTime;
    protected bool LifeTimeEnded;
    protected float LifeTime;

    protected FightPlayer Owner;
    protected List<Entity> InteractedEntities;

    protected virtual void OnOtherPlayerEnter(FightPlayer fp)
    {
        
    }

    protected virtual void OnEntityEnter(Entity entity)
    {
        if (InteractedEntities.Contains(entity)) return;
        
        InteractedEntities.Add(entity);
        
        FightPlayer fp = entity.GetComponent<FightPlayer>();
        if (fp != null && fp != Owner)
        {
            OnOtherPlayerEnter(fp);
        }
    }

    /// <summary>
    /// Call this function in ServerSpawn's after spawn action
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="lifeTime"></param>
    public virtual void Initialization(FightPlayer owner, float lifeTime)
    {
        Owner = owner;
        EntityLifeTime = lifeTime;
        InteractedEntities = new List<Entity>();
        TriggerCollider.OnCollisionEnter += OnEntityEnter;
    }

    public override void Update()
    {
        base.Update();
        if (Util.OneTime(LifeTime > EntityLifeTime, ref LifeTimeEnded))
        {
            OnLifeTimeRunOut();
            //Log.Warn($"Entity {Entity.Name} Destroyed!");
        }

        LifeTime += Time.DeltaTime;
    }

    public override void Start()
    {
        base.Start();
        TriggerCollider ??= Entity.GetComponent<Circle_Collider>();
        Animator ??= Entity.GetComponent<Spine_Animator>();
        if (TriggerCollider == null)
        {
            Log.Error($"Collider is not found on {Entity.Name}!");
        }
        
    }

    /// <summary>
    /// Called when the timer of this object runs out
    /// Override this function to get more complicated behaviors.
    /// </summary>
    protected virtual void OnLifeTimeRunOut()
    {
        Despawn();
    }

    public void Despawn()
    {
        if(Network.IsServer) Network.Despawn(Entity);
        Entity.Destroy();
    }
    
}