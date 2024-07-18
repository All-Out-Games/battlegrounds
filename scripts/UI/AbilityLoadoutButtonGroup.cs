using AO;

namespace Assembly.scripts.UI;

public class AbilityLoadoutButtonGroup : Component
{
    [Serialized] private UIButton _skillButton;
    [Serialized] private UIButton _swapButton;
    [Serialized] private UIButton _removeButton;

    [Serialized] private Entity _clickEnableGroup;

    public string SkillKey = "Empty";
    
    private AbilityLoadoutPage _page;

    private bool _toggled = false;
    private int _index = 0;
    
    public override void Start()
    {
        base.Start();
        
    }

    public void Initialize(AbilityLoadoutPage parentPage)
    {
        _page = parentPage;
    }

    // TODO
    public void OnSkillButtonClicked()
    {
        
    }

    public void OnSwapButtonClicked()
    {
        
    }

    public void OnRemoveButtonClicked()
    {
        
    }
}