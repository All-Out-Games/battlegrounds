using AO;

namespace Assembly.scripts.VFX;


public class VFX : Component
{
    [Serialized] protected bool Loop;
    [Serialized] public Spine_Animator Animator;
    [Serialized] protected float EntityLifeTime;
    protected bool LifeTimeEnded;
    protected float LifeTime;
    
    public void Despawn()
    {
        if(Network.IsServer) Network.Despawn(Entity);
        Entity.Destroy();
    }
    
    public override void Awake()
    {
        base.Awake();
        Animator = Entity.GetComponent<Spine_Animator>();
        
        //Animator.DepthOffset = 3;
    }
}
public class BaseVFX : VFX
{
    [Serialized] public string[] StartAnimationStr; // Random Play

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
            Despawn();
            //Log.Warn($"Entity {Entity.Name} Destroyed!");
        }

        LifeTime += Time.DeltaTime;
    }
}

public class SelectionVFX : VFX
{
    protected bool Started;

    /// <summary>
    /// Use FightClubGameManager -> ClientSpawn to spawn the vfx and call this function to select the correct animation to play.
    /// </summary>
    /// <param name="selectionKey"></param>
    /// <param name="loop"></param>
    public void StartVFX(string selectionKey, bool loop)
    {
        Loop = loop;
        Animator.SpineInstance.SetAnimation(selectionKey, Loop);
        Started = true;
    }
    
    public override void Update()
    {
        if (Started)
        {
            if (Util.OneTime(LifeTime > EntityLifeTime, ref LifeTimeEnded))
            {
                Despawn();
                //Log.Warn($"Entity {Entity.Name} Destroyed!");
            }

            LifeTime += Time.DeltaTime;
        }
    }
}