using AO;

namespace Assembly.scripts.UI;

public class UIButtonFitAspect : Component
{
    [Serialized] protected UICallback Cb;
    [Serialized] protected UIButton Btn;

    public override void Awake()
    {
        base.Awake();
        Cb.Callback += FitAspect;
    }

    /// <summary>
    /// Fit the aspect of the Button.
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    protected Rect FitAspect(Rect rect)
    {
        return rect.FitAspect(Btn.Settings.Sprite.Aspect);
    }
}