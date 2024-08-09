using AO;

namespace Assembly.scripts.UI;

public class UIColorMultiplierBegin : Component
{
    [Serialized] public Vector4 Modifier;
    [Serialized] protected UICallback Cb;
    [Serialized] protected UIColorMultiplierEnd End;

    public override void Awake()
    {
        base.Awake();
        Cb.Callback += PushModifier;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Cb.Callback -= PushModifier;
    }

    protected Rect PushModifier(Rect rect)
    {
        if (End != null)
        {
            AO.UI.PushColorMultiplier(Modifier);
        }
        else
        {
            Log.Error("UIColorMultiplierBegin must be used in par with its End Component!");
        }
        
        return rect;
    } 
}

public class UIColorMultiplierEnd : Component
{
    [Serialized] protected UICallback Cb;
    [Serialized] protected UIColorMultiplierBegin Begin;
    
    public override void Awake()
    {
        base.Awake();
        Cb.Callback += PopModifier;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Cb.Callback -= PopModifier;
    }

    
    protected Rect PopModifier(Rect rect)
    {
        if (Begin != null)
        {
            AO.UI.PopColorMultiplier();
        }
        else
        {
            Log.Error("UIColorMultiplierEnd must be used in par with its Begin Component!");
        }
        
        return rect;
    } 
}