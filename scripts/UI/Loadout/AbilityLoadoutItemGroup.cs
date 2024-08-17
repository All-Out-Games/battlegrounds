using AO;

namespace Assembly.scripts.UI;

public class AbilityLoadoutItemGroup : Component
{
    
    [Serialized] public UIButton Button;
    [Serialized] public UIButton InfoButton;
    public string SkillKey;
    
    [Serialized] private Entity _selectionBorder;
    [Serialized] private Entity _equippedBorder;
    [Serialized] private Entity _replaceIcon;
    [Serialized] private Entity[] _stars;
    private AbilityLoadoutPage _abWindow;
    
    public SkillConfig.SkillTreeTabs NTab;

    public bool Equipped = false;
    

    public void Initialize(AbilityLoadoutPage parentWindow, SkillConfig.SkillTreeTabs ntab)
    {
        _abWindow = parentWindow;
        NTab = ntab;
    }

    public void OnItemSelected()
    {
        _abWindow.OnItemSelected(this);
    }

    public void OnInfoButtonSelected()
    {
        _abWindow.OnItemInfoClicked(this);
    }

    public void SetItemEquipped(bool equip)
    {
        Equipped = equip;
        _equippedBorder.LocalEnabled = equip;
    }

    public void SetItemHighlighted(bool selected)
    {
        _selectionBorder.LocalEnabled = selected;
    }

    public void OnParentStateChange(AbilityLoadoutPage.LoadoutPageState state)
    {
        if (state == AbilityLoadoutPage.LoadoutPageState.Swap)
        {
            _replaceIcon.LocalEnabled = true;
        }
        else
        {
            _replaceIcon.LocalEnabled = false;
        }
    }

    public void RefreshLevel(int level, bool upgradable)
    {
        if (upgradable)
        {
            ShowStars(level);
        }
        else
        {
            ShowStars(0);
        }
    }
    
    public void ShowStars(int star)
    {
        for (int i = 0; i < 4; i++)
        {
            _stars[i].LocalEnabled = i < star;
        }
    }
}