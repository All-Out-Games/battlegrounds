using AO;

namespace Assembly.scripts.UI.Legacy;

public class AbilityBookItem : Component
{
    [Serialized] private Entity _selectionBorder;
    [Serialized] private UIText _skillName;
    [Serialized] private UIImage _skillIcon;
    [Serialized] private UIButton _btn;
    
    public bool Selected;
    public string SkillKey;
    public SkillConfig.SkillTreeTabs NTab;

    private AbilityBookWindow _abWindow;

    public void SetSkillName(string txt)
    {
        _skillName.Text = txt;
        SkillKey = txt; // Currently using skill key as skill name. Later it might change
    }

    public void SetIcon(Texture iconSprite)
    {
        _skillIcon.Sprite = iconSprite;
    }

    public void SetSelected(bool selected)
    {
        Selected = selected;
        if (selected)
        {
            _skillName.Settings = _skillName.Settings with { Color = Vector4.LightBlue };
        }
        else
        {
            _skillName.Settings = _skillName.Settings with { Color = Vector4.White };
        }
    }

    public void Initialize(AbilityBookWindow parentWindow, SkillConfig.SkillTreeTabs ntab)
    {
        _abWindow = parentWindow;
        NTab = ntab;
        _btn.OnClicked += () => { parentWindow.OnSkillSelect(this); };
    }
}