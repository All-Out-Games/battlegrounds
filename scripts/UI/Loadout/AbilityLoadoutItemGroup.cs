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
        // TODO
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
}