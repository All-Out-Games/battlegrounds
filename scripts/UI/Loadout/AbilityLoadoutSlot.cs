using AO;

namespace Assembly.scripts.UI;

public class AbilityLoadoutSlot : Component
{
    [Serialized] public UIButton SkillButton;
    [Serialized] public UIButton SwapButton;
    [Serialized] public UIButton RemoveButton;

    [Serialized] private Entity _clickEnableGroup;
    [Serialized] private Entity _replaceIcon;

    public string SkillKey = "Empty";
    
    private AbilityLoadoutPage _page;

    private bool _toggled = false;
    /// <summary>
    /// This is the index of this item's skillkey in the _equippedSkillKey array
    /// Due to the first slot being punch, Index-1 will be the slot's position in its Component array.
    /// 
    /// Use it directly when calling RemoveSkill / EquipSkill ; When accessing UI, use Index - 1
    /// 
    /// </summary>
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

    public void OnSwapClicked()
    {
        _page.OnSwapClicked(this);
    }

    public void OnRemoveClicked()
    {
        _page.OnRemoveClicked(this);
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

        RemoveButton.Interactable = skey != FightAbility.DefaultSkillKey; // You cannot remove an Empty slot
    }
    
    public void OnParentStateChange(AbilityLoadoutPage.LoadoutPageState state)
    {
        if (state != AbilityLoadoutPage.LoadoutPageState.Normal)
        {
            _replaceIcon.LocalEnabled = true;
        }
        else
        {
            _replaceIcon.LocalEnabled = false;
        }
    }
}