using AO;

namespace Assembly.scripts.UI;

public class AbilityLoadoutItemGroup : Component
{
    
    [Serialized] public UIButton Button;
    [Serialized] public UIButton InfoButton;
    public string SkillKey;
    
    [Serialized] private Entity _selectionBorder;
    [Serialized] private Entity _equippedBorder;
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
}