using AO;

namespace Assembly.scripts.VFX;

public class BaseVFX : Component
{
    [Serialized] protected string[] StartAnimationStr;
    [Serialized] protected bool Loop;
    [Serialized] protected Spine_Animator Animator;
    
    [Serialized] protected float EntityLifeTime;
    protected bool LifeTimeEnded;
    protected float LifeTime;

    public override void Awake()
    {
        base.Awake();
        Animator = Entity.GetComponent<Spine_Animator>();
        
        //Animator.DepthOffset = 3;
    }

    public override void Start()
    {
        base.Start();
        var rngTrigger = StartAnimationStr.GetRandom();
        Animator.SpineInstance.SetAnimation(rngTrigger, Loop);
    }

    public override void Update()
    {
        if (Util.OneTime(LifeTime > EntityLifeTime, ref LifeTimeEnded))
        {
            if(Network.IsServer) Network.Despawn(Entity);
            Entity.Destroy();
            //Log.Warn($"Entity {Entity.Name} Destroyed!");
        }

        LifeTime += Time.DeltaTime;
    }
}