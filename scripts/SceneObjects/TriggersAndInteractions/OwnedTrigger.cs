using AO;

namespace Assembly.scripts.SceneObjects.TriggersAndInteractions;

/// <summary>
/// Server-spawned object in scene. 
/// </summary>
public partial class OwnedTrigger : OwnedObjectComponent
{
    [Serialized] protected Circle_Collider TriggerCollider;
    [Serialized] public Spine_Animator Animator;
    
    [Serialized] protected float EntityLifeTime;
    [Serialized] protected bool LifeTimeEnded;
    [Serialized] protected float TimeElapsed;
    
    
    protected List<Entity> InteractedEntities;
    protected bool Initialized;

    protected virtual void OnOtherPlayerEnter(FightPlayer fp)
    {
        
    }

    protected virtual void OnEntityEnter(Entity entity)
    {
        if (InteractedEntities.Contains(entity)) return;
        
        InteractedEntities.Add(entity);
        
        Log.Debug($"Entity {entity.Name} enters.");
        
        FightPlayer fp = entity.GetComponent<FightPlayer>();
        if (fp.Alive())
        {
            OnOtherPlayerEnter(fp);
        }
    }

    /// <summary>
    /// Call this function in ServerSpawn's after spawn action
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="lifeTime"></param>
    [ClientRpc]
    public virtual void Initialization(Entity owner, float lifeTime)
    {
        Owner = owner.GetComponent<FightPlayer>();
        EntityLifeTime = lifeTime;
        InteractedEntities = new List<Entity>();
        TriggerCollider.OnCollisionEnter += OnEntityEnter;
        Log.Debug($"Initialized! Owner = {owner.Name}");
        Initialized = true;
    }

    public override void Update()
    {
        base.Update();
        if (Owner == null)
        {
            Log.Warn($"Did not find owner of owned object for {Entity.Name}!");
            return;
        }
        
        if (Initialized && Util.OneTime(TimeElapsed > EntityLifeTime, ref LifeTimeEnded))
        {
            OnLifeTimeRunOut();
            Log.Debug($"LifeTime Runout called for {Entity.Name}");
        }

        TimeElapsed += Time.DeltaTime;
    }

    public override void Awake()
    {
        base.Awake();
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

    public override void Despawn()
    {
        Log.Debug($"Despawn called for {Entity.Name}");
        if (Network.IsServer)
        {
            Network.Despawn(Entity);
            Entity.Destroy();
        }
        
        
    }
}