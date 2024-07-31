using AO;

namespace Assembly.scripts.UI;

public class UIMaskBegin : Component
{
    [Serialized] protected UICallback Cb;
    
    [Serialized] protected Texture Mask;
    [Serialized] protected UIMaskEnd End;

    public IM.MaskScope Scope;
    
    
    public override void Awake()
    {
        base.Awake();
        Cb.Callback += EnableMask;
        Log.Debug($"A UICallback has been created by UIMaskBegin, on {Entity.Name}");
    }

    public Rect EnableMask(Rect rect)
    {
        if (End != null)
        {
            Scope = IM.CreateMaskScope(rect);
            {
                using var _ = IM.BUILD_MASK_SCOPE(Scope);
                AO.UI.Image(rect, Mask);
            }

            IM.MaskScopeBegin(Scope);
        }
        else
        {
            Log.Error("UIMaskBegin has to be used in pair with UIMaskEnd!");
        }
        
        return rect;
    }
}

public class UIMaskEnd : Component
{
    [Serialized] protected UIMaskBegin Begin;

    [Serialized] protected UICallback Cb;

    public override void Awake()
    {
        base.Awake();
        Cb.Callback += DisableMask;
        Log.Debug($"A UICallback Component has been created by UIMaskEnd, on {Entity.Name}");
    }

    public Rect DisableMask(Rect rect)
    {
        if (Begin != null)
        {
            IM.MaskScopeEnd();
        }
        else
        {
            Log.Error("UIMaskEnd has to be used in pair with UIMaskBegin!");
        }
        
        return rect;
    }
}