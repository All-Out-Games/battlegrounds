using AO;

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

    [Serialized] private UIText _abilityTabName;
    [Serialized] private UIText _emptyTabText;

    [Serialized] private UIDirectionalLayout _skillList;

    private UIButton[] _equipButtons = new UIButton[6];
    private UIText[] _equipTexts = new UIText[6];
    
    private Dictionary<string, AbilityBookItem> _bookItems = new Dictionary<string, AbilityBookItem>(); // [skillkey : AbilityBookItem]
    private string _abilityBookItemPrefabPath = "AbilityBookItem.prefab";
    private string[] _equippedSkillKey = new string[6];

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

        Entity layoutEntity = _skillList.Entity;
        Prefab itemPrefab = Assets.GetAsset<Prefab>(_abilityBookItemPrefabPath);
        
        // Create nodes for all active skills, but only activate when they are unlocked.
        foreach (string key in SkillConfig.GetAllSkillKeys())
        {
            SkillConfig.SkillTreeNodeConfig cfg = SkillConfig.GetConfig(key);
            if (cfg.NType == SkillConfig.NodeType.SkillUnlock) 
            {
                AbilityBookItem itm = itemPrefab.Instantiate().GetComponent<AbilityBookItem>();
                itm.Initialize(this, cfg.NTab);
                itm.SetSkillName(key);
                itm.SetIcon(Assets.GetAsset<Texture>(SkillConfig.GetIconPath(key)));
                
                itm.Entity.SetParent(layoutEntity, false);
                _bookItems.Add(key, itm);
                //Log.Debug($"{key} item Created!");
            }
        }
        
        OnWindowOpen += (window, player) =>
        {
            // Update currently equipped skills to the UI
            List<FightAbility> faList = _slotsMgr.GetCurrentAbilities();
            _equipButtons[0] = Icon0;
            _equipButtons[1] = Icon1;
            _equipButtons[2] = Icon2;
            _equipButtons[3] = Icon3;
            _equipButtons[4] = Icon4;
            _equipButtons[5] = Icon5;
        
            _equipTexts[0] = AbilityText0;
            _equipTexts[1] = AbilityText1;
            _equipTexts[2] = AbilityText2;
            _equipTexts[3] = AbilityText3;
            _equipTexts[4] = AbilityText4;
            _equipTexts[5] = AbilityText5;
        
            for (int i = 0; i < 6; i++)
            {
                _equippedSkillKey[i] = faList[i].SkillKey;
                FightAbility ab = faList[i];
                _equipButtons[i].Settings = _equipButtons[i].Settings with { Sprite = ab.Icon };
                _equipTexts[i].Text = ab.SkillKey;
            }
        };
    }

    public override void Start()
    {
        base.Start();
        

        // Btn events
        _tabAmount = SkillConfig.STConfigTabsList.Count;
        UpdateSkillTreeTab();
        
        NextButton.OnClicked += NextTab;
        PrevButton.OnClicked += PreviousTab;
        SaveButton.OnClicked += SaveSkills;
        // Punch cannot be replaced
        Icon1.OnClicked += () => { OnSkillEquip(1); };
        Icon2.OnClicked += () => { OnSkillEquip(2); };
        Icon3.OnClicked += () => { OnSkillEquip(3); };
        Icon4.OnClicked += () => { OnSkillEquip(4); };
        Icon5.OnClicked += () => { OnSkillEquip(5); };
        
    }
    
    

    public override void OnDestroy()
    {
        base.OnDestroy();
        NextButton.OnClicked -= NextTab;
        PrevButton.OnClicked -= PreviousTab;
        SaveButton.OnClicked -= SaveSkills;
        
        Icon1.OnClicked = null;
        Icon2.OnClicked = null;
        Icon3.OnClicked = null;
        Icon4.OnClicked = null;
        Icon5.OnClicked = null;

        OnWindowOpen = null;
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

    public void OnSkillSelect(AbilityBookItem itm)
    {
        if (itm.SkillKey == _selectedSkillKey)
        {
            _selectedSkillKey = String.Empty;
            itm.SetSelected(false);
        }
        else
        {
            if(_selectedSkillKey != String.Empty) _bookItems[_selectedSkillKey].SetSelected(false);
            _selectedSkillKey = itm.SkillKey;
            itm.SetSelected(true);
        }
    }

    private void SaveSkills()
    {
        for (int i = 1; i < 6; i++)
        {
            Type f = FightAbility.AbilityQueryDict[_equippedSkillKey[i]];
            
            _slotsMgr.ReplaceSlot(i, _slotsMgr.GetAbilityInstance(f));
            _slotsMgr.CallServer_SetSavedSkillSlot(i, _equippedSkillKey[i]);
        }
        CloseWindow();
    }

    private void OnSkillEquip(int idx)
    {
        for (int i = 1; i < 6; i++)
        {
            // Unequip to avoid duplicated entries
            if (_selectedSkillKey == _equippedSkillKey[i])
            {
                if (i == idx) return;
                //Log.Warn($"Dupe Skill detected: {_selectedSkillKey}");
                _equippedSkillKey[i] = FightAbility.DefaultSkillKey;
                _equipButtons[i].Settings = 
                    _equipButtons[i].Settings with { Sprite = Assets.GetAsset<Texture>(FightAbility.DefaultIconPath) };
                _equipTexts[i].Text = FightAbility.DefaultSkillKey;
            }
        }
        
        _equippedSkillKey[idx] = _selectedSkillKey;
        OnSkillSelect(_bookItems[_selectedSkillKey]); // unselect the current key
        
        //Then update the icon and name
        var cfg = SkillConfig.GetConfig(_equippedSkillKey[idx]);
        _equipButtons[idx].Settings =
            _equipButtons[idx].Settings with { Sprite = Assets.GetAsset<Texture>(cfg.IconPath) };
        _equipTexts[idx].Text = cfg.SkillKey;
    }

    private void UpdateSkillTreeTab(SkillConfig.SkillTreeTabs tab = SkillConfig.SkillTreeTabs.Basic)
    {
        // All ability nodes are loaded upon startup. We just need to enable those belong to this tab
        _currentTab = tab;
        _abilityTabName.Text = SkillConfig.STTabsNameQueryDict[tab];
        bool haveItemsFlag = false;
        foreach (var kv in _bookItems)
        {
            // a. Enable items in tab
            var item = kv.Value;
            if (item.NTab == tab && _skillTree.GetSkillLevel(item.SkillKey) > 0)
            {
                item.Entity.LocalEnabled = true;
                haveItemsFlag = true;
            }
            else
            {
                item.Entity.LocalEnabled = false;
            }
        }

        _emptyTabText.Entity.LocalEnabled = !haveItemsFlag; // Enable when the tab is empty (possible for an entirely passive tree)
    }
}