using AO;

namespace Assembly.scripts.UI;

public class AbilityLoadoutSlot : Component
{
    [Serialized] public UIButton SkillButton;
    [Serialized] public UIButton SwapButton;
    [Serialized] public UIButton RemoveButton;

    [Serialized] private Entity _clickEnableGroup;

    public string SkillKey = "Empty";
    
    private AbilityLoadoutPage _page;

    private bool _toggled = false;
    public int Index = 0;
    
    public override void Start()
    {
        base.Start();
        
    }

    public void Initialize(AbilityLoadoutPage parentPage, int idx)
    {
        _page = parentPage;
        Index = idx;
    }

    public void Toggle()
    {
        _toggled = !_toggled;
        _clickEnableGroup.LocalEnabled = _toggled;
    }

    public void Unselect()
    {
        if (_toggled)
        {
            Toggle();
        }
    }
}