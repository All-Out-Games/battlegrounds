using AO;
using Assembly.scripts.SkillSlots.Abilities;

namespace Assembly.scripts.UI;

public class AbilityBookWindow : UniqueUIWindow
{
    [Serialized] private UIButton SaveButton;
    [Serialized] private UIButton NextButton;
    [Serialized] private UIButton PrevButton;
    
    [Serialized] private UIButton Icon0; // Always punch. Cannot be changed
    [Serialized] private UIButton Icon1;
    [Serialized] private UIButton Icon2;
    [Serialized] private UIButton Icon3;
    [Serialized] private UIButton Icon4;
    [Serialized] private UIButton Icon5;

    [Serialized] private UIText AbilityText0;
    [Serialized] private UIText AbilityText1;
    [Serialized] private UIText AbilityText2;
    [Serialized] private UIText AbilityText3;
    [Serialized] private UIText AbilityText4;
    [Serialized] private UIText AbilityText5;

    [Serialized] private UIDirectionalLayout _skillList;
    private Dictionary<string, AbilityBookItem> _bookItems; // [skillkey : AbilityBookItem]
    private string _abilityBookItemPrefabPath = "AbilityBookItem.prefab";

    private FightPlayerSkillSlotsManager _slotsMgr;
    private FightPlayerSkillTree _skillTree;
    private SkillConfig.SkillTreeTabs _currentTab = SkillConfig.SkillTreeTabs.Basic;
    private int _currentTabIndex = 0;
    private int _tabAmount = 0;
    private string _selectedSkillKey; // selected key in the skillList

    public override void OnInstantiate()
    {
        base.OnInstantiate();
        FightPlayer fp = Network.LocalPlayer.Entity.GetComponent<FightPlayer>();
        _slotsMgr ??= fp.GetSkillSlots();
        _skillTree ??= fp.GetSkillTree();

        Entity layoutEntity = _skillList.Entity;
        Prefab itemPrefab = Assets.GetAsset<Prefab>(_abilityBookItemPrefabPath);
        foreach (string key in SkillConfig.GetAllSkillKeys())
        {
            SkillConfig.SkillTreeNodeConfig cfg = SkillConfig.GetConfig(key);
            if (cfg.NType == SkillConfig.NodeType.SkillUnlock) // Create nodes for all active skills, but only activate when they are unlocked.
            {
                AbilityBookItem itm = itemPrefab.Instantiate().GetComponent<AbilityBookItem>();
                itm.Initialize(this);
                itm.SetSkillName(key);
                itm.SetIcon(Assets.GetAsset<Texture>(SkillConfig.GetIconPath(key)));
                
                itm.Entity.SetParent(layoutEntity, false);
                Log.Debug($"{key} item Created!");
            }
        }
    }

    public override void Start()
    {
        base.Start();
        List<FightAbility> faList = _slotsMgr.GetCurrentAbilities();
        
        // Fill Skill Slots on the UI with current abilities
        FightAbility punch = faList[0];
        Icon0.Settings = Icon0.Settings with { Sprite = punch.Icon };
        AbilityText0.Text = punch.SkillKey;
        FightAbility ab1 = faList[1];
        Icon1.Settings = Icon1.Settings with { Sprite = ab1.Icon };
        AbilityText1.Text = ab1.SkillKey;
        FightAbility ab2 = faList[2];
        Icon2.Settings = Icon2.Settings with { Sprite = ab2.Icon };
        AbilityText2.Text = ab2.SkillKey;
        FightAbility ab3 = faList[3];
        Icon3.Settings = Icon3.Settings with { Sprite = ab3.Icon };
        AbilityText3.Text = ab3.SkillKey;
        FightAbility ab4 = faList[4];
        Icon4.Settings = Icon4.Settings with { Sprite = ab4.Icon };
        AbilityText4.Text = ab4.SkillKey;
        FightAbility ab5 = faList[5];
        Icon5.Settings = Icon5.Settings with { Sprite = ab5.Icon };
        AbilityText5.Text = ab5.SkillKey;

        // Instantiate all ability book items
        
    }

    private void PreviousTab()
    {
        _currentTabIndex -= 1;
        if (_currentTabIndex == -1)
        {
            _currentTabIndex = _tabAmount - 1;
        }
        UpdateSkillTreeTab(SkillConfig.STConfigTabsList[_currentTabIndex]);
    }

    private void NextTab()
    {
        _currentTabIndex += 1;
        if (_currentTabIndex >= _tabAmount)
        {
            _currentTabIndex = 0;
        }
        UpdateSkillTreeTab(SkillConfig.STConfigTabsList[_currentTabIndex]);
    }

    private void UpdateSkillTreeTab(SkillConfig.SkillTreeTabs tab)
    {
        // All ability nodes are loaded upon startup. We just need to enable those belong to this tab
    }
}