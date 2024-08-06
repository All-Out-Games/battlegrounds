using AO;

namespace Assembly.scripts.UI;

public class UISkillTabButton : Component
{

    public int Index;
    private Texture _pressedTexture;
    private Texture _normalTexture;
    private UIButton _btn;

    [Serialized] private Entity _selectionBorder;

    public Action<int> OnTabButtonClicked;

    public void Initialize(int idx, Action<int> onIndexedTabClicked)
    {
        Index = idx;
        OnTabButtonClicked += onIndexedTabClicked;

        _btn = Entity.GetComponent<UIButton>();
        
        if (_btn != null)
        {
            _btn.OnClicked += () =>
            {
                OnTabButtonClicked.Invoke(Index);
            };
            _pressedTexture = _btn.Settings.SpritePressed;
            _normalTexture = _btn.Settings.Sprite;
        }
        else
        {
            Log.Error("Skilltab button must have the button component!");
        }
        
    }

    public void SetToggled(bool toggled)
    {
        _selectionBorder.LocalEnabled = toggled;
        if (toggled)
        {
            var buttonSettings = _btn.Settings with
            {
                SpritePressed = _normalTexture,
                Sprite = _pressedTexture
            };
            _btn.Settings = buttonSettings;
        }
        else
        {
            var buttonSettings = _btn.Settings with
            {
                SpritePressed = _pressedTexture,
                Sprite = _normalTexture
            };
            _btn.Settings = buttonSettings;
        }
    }
}