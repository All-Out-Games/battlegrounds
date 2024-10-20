using AO;
using Vector2 = System.Numerics.Vector2;

namespace Assembly.scripts.SceneObjects.Projectiles;

public partial class BaseProjectile : OwnedObjectComponent
{
    protected Projectile EngineProjectile;

    protected List<Entity> WhiteList = new List<Entity>(); // Projectiles will not interact with this list of entities.

    [Serialized] protected int Damage = 0;
    [Serialized] protected bool Pierce = false;

    [Serialized] public float LifeTime;
    [Serialized] protected float TimeElapsed;

    [Serialized] public bool Blockable = true;

    protected ulong SoundId = default;

    [Serialized] protected float EngineProjectileSpeed;
    [Serialized] protected float SpeedModifier;

    public override void Awake()
    {
        base.Awake();
        EngineProjectile = Entity.GetComponent<Projectile>();
        if (EngineProjectile == null)
        {
            Log.Error($"Projectile {Entity.Name} does not have Projectile component attached!");
            Entity.Destroy();
            return;
        }
        EngineProjectile.Awaken();
        EngineProjectile.OnHit += OnHit;

        
    }
    
    private void LazyInitialize()
    {
        WhiteList.Add(Owner.Entity);
        WhiteList.Add(Owner.CollisionEntity);
        TimeElapsed = 0;
        EngineProjectileSpeed = EngineProjectile.Speed;
        SpeedModifier = 1.0f;
    }

    public override void OnDestroy()
    {
        if (EngineProjectile.Alive())
        {
            EngineProjectile.OnHit -= OnHit;
        }
        if (SoundId != default)
        {
            //Log.Warn($"Sound Stop ID = {SoundId}");
            SFX.FadeOutAndStop(SoundId, 0.3f);
        }
    }

    public override void Update()
    {
        if (!Owner.Alive())
        {
            return;
        }
        base.Update();
        // Server Authoritative Projectile
        TimeElapsed += Time.DeltaTime;
        if (TimeElapsed > LifeTime)
        {
            Entity.Destroy();
        }
    }

    /// <summary>
    /// Base function to call when a collision is detected. Do not override unless you absolutely need to.
    /// Implement different effects in DoProjectileEffect
    /// </summary>
    /// <param name="other"></param>
    /// <param name="predicted"></param>
    protected virtual void OnHit(Entity other, bool predicted)
    {
        if (Blockable && TimeElapsed > 0.2f) // Must be at least 0.2s old to be blocked
        {
            var blockComp = other.GetComponent<ProjectileBlocker>();
            if (blockComp.Alive())
            {
                blockComp.DoBlockerEffect(this);
            }
        }
        
        if (!WhiteList.Contains(other))
        {
            WhiteList.Add(other);
            DoProjectileEffect(other, predicted);
        }
    }

    /// <summary>
    /// Override ME!
    /// </summary>
    /// <param name="other"></param>
    /// <param name="predicted"></param>
    protected virtual void DoProjectileEffect(Entity other, bool predicted)
    {
        // NOTE: Will be called on Server/Client
    }

    public virtual void InitializeProjectile(FightPlayer owner, int dmg, bool pierce, object customData = null)
    {
        Owner = owner;
        Damage = dmg;
        Pierce = pierce;
        
        LazyInitialize();
    }

    public void AddIgnoredPlayer(FightPlayer fp)
    {
        WhiteList.Add(fp.Entity);
        WhiteList.Add(fp.CollisionEntity);
    }

    public override void Despawn()
    {
        Entity.Destroy();
    }

    public void ModifySpeed(float multiplier)
    {
        SpeedModifier = multiplier;
        if (EngineProjectile.Alive())
        {
            EngineProjectile.Speed = EngineProjectileSpeed * SpeedModifier;
            var rb = GetComponent<Rigidbody>();
            rb.Velocity *= SpeedModifier;
        }
    }

    public void ResumeSpeed()
    {
        if (EngineProjectile.Alive())
        {
            EngineProjectile.Speed = EngineProjectileSpeed;
            var rb = GetComponent<Rigidbody>();
            rb.Velocity /= SpeedModifier;
        }
        
    }
}