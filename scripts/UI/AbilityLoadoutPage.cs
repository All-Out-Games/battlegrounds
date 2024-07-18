using AO;

namespace Assembly.scripts.UI;

public class AbilityLoadoutPage : UniqueUIWindow
{
    // Loadout Session
    [Serialized] private UIImage _punchIcon;
    [Serialized] private UIButton _icon1;
    [Serialized] private UIButton _icon2;
    [Serialized] private UIButton _icon3;
    [Serialized] private UIButton _icon4;
    [Serialized] private UIButton _icon5;
    private UIButton[] _equipButtons = new UIButton[5];
    private UIText[] _equipTexts = new UIText[5];
    
    // Skill book Session
    [Serialized] private UIText _abilityTabName;
    [Serialized] private UIText _emptyTabText;
    [Serialized] private UIDirectionalLayout _skillList;

    // TODO: Info popup
    
    
    // Data
    private FightPlayerSkillSlotsManager _slotsMgr;
    private FightPlayerSkillTree _skillTree;
    private SkillConfig.SkillTreeTabs _currentTab = SkillConfig.SkillTreeTabs.Basic;
    private int _currentTabIndex = 0;
    private int _tabAmount = 0;
    private string _selectedSkillKey = String.Empty; // selected key in the skillList

    public override void OnInstantiate()
    {
        base.OnInstantiate();
        
        FightPlayer fp = Network.LocalPlayer.Entity.GetComponent<FightPlayer>();
        _slotsMgr ??= fp.GetSkillSlots();
        _skillTree ??= fp.GetSkillTree();
    }
}