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
    
    

    public void Initialize(AbilityLoadoutPage parentPage, int idx)
    {
        _page = parentPage;
        Index = idx;
    }

    public void Toggle()
    {
        _toggled = !_toggled;
        _clickEnableGroup.LocalEnabled = _toggled;
        if (_toggled)
        {
            _page.OnSlotSelected(this);
        }
        //Log.Warn($"Toggled: {_toggled}; Slot: {SkillKey}");
    }

    public void Unselect()
    {
        if (_toggled)
        {
            Toggle();
        }
    }

    public void SetSkillKey(string skey)
    {
        SkillKey = skey;
        FightClubUtils.SetButtonTexture(SkillButton, SkillConfig.GetIconPath(skey));
    }
}