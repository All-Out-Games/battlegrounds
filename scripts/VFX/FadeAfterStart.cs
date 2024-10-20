using AO;

namespace Assembly.scripts.VFX;

public class FadeAfterStart : Component
{
    // This will fade the spine animator / sprite renderer after a set timer.

    [Serialized] protected bool FadeSpine;
    [Serialized] protected bool FadeSprite;

    [Serialized] protected float PersistTime = 1f; // How long this object should persist before fading
    [Serialized] protected float FadeTime = 1f; // How many seconds you want before this object completely fades away? 
    // NOTE: FadeTime must be longer than persist time!

    protected Spine_Animator Animator;
    protected Sprite_Renderer Sprite;
    

    public float ElapsedTime = 0;

    public Action OnFaded;
    protected bool Faded;
    
    public override void Awake()
    {
        base.Awake();
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
            Entity.Destroy();
        }

        if (FadeTime < 0.1f) FadeTime = 0.1f;
        if (PersistTime < 0) PersistTime = 0;
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

        if (Util.OneTime(ElapsedTime > FadeTime, ref Faded))
        {
            OnFaded?.Invoke();
        }
        
    }

    public void SetPersistFadeTime(float persistTime, float fadeTime)
    {
        PersistTime = persistTime;
        FadeTime = fadeTime;
    }

    /// <summary>
    /// Set ElapsedTime = PersistTime - delay (default to 0.1f)
    /// </summary>
    /// <param name="delay"></param>
    public void FadeImmediately(float delay = 0.1f, float fadeDelay = 1f)
    {
        ElapsedTime = PersistTime - delay;
        FadeTime = PersistTime + fadeDelay;
    }

    public void ExtendLifetime(float delay = 3f)
    {
        PersistTime += delay;
        FadeTime += delay;
    }

    public bool IsFading(){
        return ElapsedTime > PersistTime;
    }
}