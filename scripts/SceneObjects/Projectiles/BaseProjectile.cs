using AO;

namespace Assembly.scripts.SceneObjects.Projectiles;

public class BaseProjectile : OwnedObjectComponent
{
    protected Projectile EngineProjectile;

    protected List<Entity> WhiteList = new List<Entity>();

    [Serialized] protected int Damage = 0;
    [Serialized] protected bool Pierce = false;

    public float LifeTime;
    protected float TimeElapsed;

    protected ulong SoundId = default;

    public override void OnOwnerLeave(FightPlayer player)
    {
        //LocalEnabled = false;
    }

    public override void Start()
    {
        base.Start();
        EngineProjectile = Entity.GetComponent<Projectile>();
        if (EngineProjectile == null)
        {
            Log.Error($"Projectile {Entity.Name} does not have Projectile component attached!");
            Entity.Destroy();
            return;
        }
        WhiteList.Add(Owner.Entity);
        WhiteList.Add(Owner.CollisionEntity);
        EngineProjectile.OnHit += OnHit;

        TimeElapsed = 0;
    }

    public override void OnDestroy()
    {
        EngineProjectile.OnHit -= OnHit;
        if (SoundId != default)
        {
            Log.Warn($"Sound Stop ID = {SoundId}");
            SFX.FadeOutAndStop(SoundId, 0.3f);
        }
    }

    public override void Update()
    {
        base.Update();
        TimeElapsed += Time.DeltaTime;
        if (TimeElapsed > LifeTime)
        {
            Entity.Destroy();
        }
    }

    protected virtual void OnHit(Entity other, bool predicted)
    {
        
        if (!WhiteList.Contains(other))
        {
            WhiteList.Add(other);
            DoProjectileEffect(other, predicted);
        }
    }

    protected virtual void DoProjectileEffect(Entity other, bool predicted)
    {
        // NOTE: Will be called on Server/Client
    }

    public virtual void InitializeProjectile(FightPlayer owner, int dmg, bool pierce, object customData = null)
    {
        Owner = owner;
        Damage = dmg;
        Pierce = pierce;
    }
}