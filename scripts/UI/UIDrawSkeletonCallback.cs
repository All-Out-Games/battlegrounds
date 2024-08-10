using AO;

namespace Assembly.scripts.UI;

public class UIDrawSkeletonCallback : Component
{
    private UICallback _callback;
    private UISpineSkeleton _skeleton;
    
    public override void Awake()
    {
        base.Awake();
        _callback = GetComponent<UICallback>();
        _skeleton = GetComponent<UISpineSkeleton>();
        _skeleton.Instance.SetSkeleton(_skeleton.Spine);
        if (_callback == null || _skeleton == null)
        {
            Log.Error("You must attach a UISpineSkeleton and a UICallback function for UIDrawSkeletonCallback to work!");
            Entity.Destroy();
        }
        else
        {
            _callback.Callback += DrawSkeleton;
        }
    }

    private Rect DrawSkeleton(Rect rect)
    {
        AO.UI.DrawSkeleton(rect, _skeleton.Instance, new Vector2(175f,175f), 0);
        return rect;
    }
}