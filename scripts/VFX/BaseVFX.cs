using AO;

namespace Assembly.scripts.VFX;


public class VFX : Component
{
    [Serialized] protected bool Loop;
    [Serialized] public Spine_Animator Animator;
    [Serialized] protected float EntityLifeTime;
    protected bool LifeTimeEnded;
    protected float LifeTime;
    
    [Serialized] public bool IsPermanent;
    
    public void Despawn()
    {
        Entity.Destroy();
    }
    
    public override void Awake()
    {
        base.Awake();
        Animator = Entity.GetComponent<Spine_Animator>();
        
        //Animator.DepthOffset = 3;
    }
    
    /// <summary>
    /// Runtime set lifetime, and reset timer
    /// </summary>
    /// <param name="t"></param>
    public void SetLifetime(float t)
    {
        EntityLifeTime = t;
        LifeTime = 0;
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

        if (!IsPermanent)
        {
            LifeTime += Time.DeltaTime;
        }
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

            if (!IsPermanent)
            {
                LifeTime += Time.DeltaTime;
            }
        }
    }
}