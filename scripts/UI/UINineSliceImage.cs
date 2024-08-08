using AO;

namespace Assembly.scripts.UI;

public class UINineSliceImage : Component
{
    [Serialized] public Texture Image;
    [Serialized] public UICallback Callback;
    [Serialized] public Vector4 Slice;
    [Serialized] public float SliceScale;

    public override void Awake()
    {
        base.Awake();
        Callback.Callback += DrawNineSlice;
    }

    Rect DrawNineSlice(Rect rect)
    {
        AO.UI.Image(rect, Image, new AO.UI.NineSlice() {slice = Slice, sliceScale = SliceScale});
        return rect;
    }
}