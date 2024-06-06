using AO;

namespace Assembly.scripts.UI;

public class AbilityBookItem : Component
{
    [Serialized] private Entity _selectionBorder;
    [Serialized] private UIText _skillName;
    [Serialized] private UIImage _skillIcon;
    [Serialized] private UIButton _btn;
    
    public bool Selected;

    private AbilityBookWindow _abWindow;

    public void SetSkillName(string txt)
    {
        _skillName.Text = txt;
    }

    public void SetIcon(Texture iconSprite)
    {
        _skillIcon.Sprite = iconSprite;
    }

    public void Initialize(AbilityBookWindow parentWindow)
    {
        _abWindow = parentWindow;
        
    }
}