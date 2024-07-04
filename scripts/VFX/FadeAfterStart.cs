using AO;

namespace Assembly.scripts.VFX;

public class FadeAfterStart : Component
{
    // This will fade the spine animator / sprite renderer after a set timer.

    [Serialized] protected bool FadeSpine;
    [Serialized] protected bool FadeSprite;

    [Serialized] protected float PersistTime = 1f;
    [Serialized] protected float FadeTime = 1f;

    protected Spine_Animator Animator;
    protected Sprite_Renderer Sprite;
    
    protected bool FadeCompleted;

    protected float ElapsedTime = 0;
    
    public override void Start()
    {
        base.Start();
        ElapsedTime = 0;
        if (FadeSpine)
        {
            Animator = Entity.GetComponent<Spine_Animator>();
            if (Animator == null)
            {
                Log.Error($"FadeAfterStart on {Entity.Name} did not find a spine animator!");
                LocalEnabled = false;
            }
        }

        if (FadeSprite)
        {
            Sprite = Entity.GetComponent<Sprite_Renderer>();
            if (Sprite == null)
            {
                Log.Error($"FadeAfterStart on {Entity.Name} did not find a sprite renderer!");
                LocalEnabled = false;
            }
        }

        if (FadeTime <= PersistTime)
        {
            Log.Warn("FadeAfterStart: Fade time must be more than persist time!");
        }
    }

    public override void Update()
    {
        base.Update();
        ElapsedTime += Time.DeltaTime;
        if (ElapsedTime < PersistTime || ElapsedTime > FadeTime + 0.05f)
        {
            return;
        }
        
        float progress10 = (ElapsedTime - PersistTime) / (FadeTime - PersistTime);
            
        progress10 = float.Clamp(1f - progress10, 0, 1);
        
        if (FadeSpine)
        {
            Animator.SpineInstance.ColorMultiplier = Animator.SpineInstance.ColorMultiplier with { W = progress10 };
        }

        if (FadeSprite)
        {
            Sprite.Tint = Sprite.Tint with { W = progress10 };
        }
        
    }
}