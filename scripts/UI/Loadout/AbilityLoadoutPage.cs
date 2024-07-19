using AO;

namespace Assembly.scripts.UI;

public class AbilityLoadoutPage : UniqueUIWindow
{
    public enum LoadoutPageState
    {
        Normal, // Nothing selected
        Swap, // Swapping a skill slot
        Equip // Trying to equip a skill from skill book session
    }
    [Serialized] private UIText _debugTxt;
    
    // Loadout Section (skill slots)
    [Serialized] private UIImage _punchIcon;
    [Serialized] private AbilityLoadoutSlot[] _loadoutSlots;

    [Serialized] private UIButton _nextTab;
    [Serialized] private UIButton _prevTab;
    
    
    
    
    
    // Skill book Section

    private List<SkillConfig.SkillTreeTabs> _availableTabs;
    
    [Serialized] private UIText _abilityTabName;
    [Serialized] private UIText _emptyTabText;
    [Serialized] private UIGrid _skillList;

    // TODO: Info popup
    
    
    // Data
    private FightPlayerSkillSlotsManager _slotsMgr;
    private FightPlayerSkillTree _skillTree;
    private string[] _equippedSkillKey = new string[6];  
    // NOTE: This page is very complicated because design asked for like one million possible input sequence of changing loadout
    // But, ultimately, this array is all we care about. It's modified through EquipSkill / RemoveSkill
    // and passed to slots manager on the player when we quit this page. 

    private float _elapsedTimeSinceOpen = 0;
    
    // Tabs
    private SkillConfig.SkillTreeTabs _currentTab = SkillConfig.SkillTreeTabs.Brawler;
    private int _currentTabIndex = 0;
    private int _tabAmount = 0;

    // Skill book items
    private Dictionary<string, AbilityLoadoutItemGroup> _bookItems;
    
    
    // State
    public LoadoutPageState State = LoadoutPageState.Normal;
    
    private AbilityLoadoutSlot _selectedSlot = null; // Slot refers to one of the 5 swappable slots in the Loadout Screen
    private AbilityLoadoutItemGroup _selectedItem = null; // Item refers to the purchased skills in the item lists

    public override void OnInstantiate()
    {
        base.OnInstantiate();
        Log.Warn("ALP Instantiated!");
        
        // Get the player's skill tree and slot manager. The skill book will be instantiated after the player first open the book
        // it exists on local client only
        FightPlayer fp = Network.LocalPlayer.Entity.GetComponent<FightPlayer>();
        _slotsMgr ??= fp.GetSkillSlots();
        _skillTree ??= fp.GetSkillTree();
        
        // First Open Phase 1
        // Spawn all skill book items (i.e. all active skills)
        // Create nodes for all active skills, but only activate when they are unlocked.
        Entity layoutEntity = _skillList.Entity;

        _bookItems = new Dictionary<string, AbilityLoadoutItemGroup>();
        
        foreach (string key in SkillConfig.GetAllSkillKeys())
        {
            Prefab itemPrefab = Assets.GetAsset<Prefab>("LoadoutItemGroup.prefab");
            SkillConfig.SkillTreeNodeConfig cfg = SkillConfig.GetConfig(key);
            if (cfg.NType == SkillConfig.NodeType.SkillUnlock) // Active skills only
            {
                AbilityLoadoutItemGroup itm = itemPrefab.Instantiate().GetComponent<AbilityLoadoutItemGroup>();
                itm.Initialize(this, cfg.NTab);
                itm.SkillKey = cfg.SkillKey;
                FightClubUtils.SetButtonTexture(itm.Button, SkillConfig.GetIconPath(key));
                //Log.Error($"Icon for Key {key} : {SkillConfig.GetIconPath(key)}");
                
                itm.Entity.SetParent(layoutEntity, false);
                _bookItems.Add(key, itm);
                //Log.Debug($"{key} item Created!");
            }
        }

        // First Open Phase 2
        // Skill Tabs. Remove Basic Tree (No active skills are in basic tab)
        _availableTabs = new List<SkillConfig.SkillTreeTabs>(SkillConfig.STConfigTabsList);
        _availableTabs.Remove(SkillConfig.SkillTreeTabs.Basic);
        _tabAmount = _availableTabs.Count;
        _prevTab.OnClicked += PreviousTab;
        _nextTab.OnClicked += NextTab;
        
        // First Open Phase 3
        // Initialize Slots
        for (int i = 0; i < 5; i++)
        {
            var slot = _loadoutSlots[i];
            slot.Initialize(this, i+1);
            slot.SkillButton.OnClicked += slot.Toggle;
        }
        
        
        ResetSelection();
        
    }

    public void UpdateLoadoutData()
    {
        // From slotsMgr and skillTree, populate the loadout 
    }
    public void ResetSelection()
    {
        // TODO: Unselect all items
        State = LoadoutPageState.Normal;
        return;
        foreach (var kv in _bookItems)
        {
            
        }

        foreach (var slot in _loadoutSlots)
        {
            slot.Unselect();
        }
    }

    public override void Update()
    {
        _elapsedTimeSinceOpen += Time.DeltaTime;
        base.Update();
    }

    public override void CloseWindow()
    {
        // Refuse to close if open time < 1s
        if(_elapsedTimeSinceOpen < 1.0f) return;
        
        ResetSelection();
        base.CloseWindow();
    }

    public override void OpenWindow()
    {
        ResetSelection();
        UpdateSkillTreeTab(_currentTab);
        FetchEquippedSkills();
        base.OpenWindow();

        _elapsedTimeSinceOpen = 0;
    }

    private void PreviousTab()
    {
        _currentTabIndex -= 1;
        if (_currentTabIndex == -1)
        {
            _currentTabIndex = _tabAmount - 1;
        }
        UpdateSkillTreeTab(_availableTabs[_currentTabIndex]);
    }

    private void NextTab()
    {
        _currentTabIndex += 1;
        if (_currentTabIndex >= _tabAmount)
        {
            _currentTabIndex = 0;
        }
        UpdateSkillTreeTab(_availableTabs[_currentTabIndex]);
    }
    
    private void UpdateSkillTreeTab(SkillConfig.SkillTreeTabs tab)
    {
        // All ability nodes are loaded upon startup. We just need to enable those belong to this tab
        _currentTab = tab;
        _abilityTabName.Text = SkillConfig.STTabsNameQueryDict[tab];
        bool haveItemsFlag = false;
        foreach (var kv in _bookItems)
        {
            // Enable items in tab if player owns the skill & skill belongs to this tab
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

    private void FetchEquippedSkills()
    {
        List<FightAbility> faList = _slotsMgr.GetCurrentAbilities();
        // Punch
        _punchIcon.Sprite = faList[0].Icon;
        for (int i = 0; i < 5; i++)
        {
            _loadoutSlots[i].SetSkillKey(faList[i+1].SkillKey);
        }
    }
    
    // Equip Skill
    private void EquipSkill(string key, int idx)
    {
        // Phase 1. Remove Duplicated Skill, if detected
        for (int i = 1; i < 6; i++)
        {
            var slot = _loadoutSlots[i];
            if (slot.SkillKey == key)
            {
                if (i == idx) return;
                RemoveSkill(i);
            }
        }
        
        
    }

    private void RemoveSkill(int idx)
    {
        if (idx == 0)
        {
            Log.Error("You can't remove Punch!");
            return;
        }
        // Equip an Empty skill at the slot.
        _equippedSkillKey[idx] = FightAbility.DefaultSkillKey;

        int slotIdx = idx - 1;
        // Remove the slot (Punch does not have a slot so the slot index need -1)
        
    }

    private void SetState(LoadoutPageState st)
    {
        State = st;
        switch (st)
        {
            case LoadoutPageState.Normal:
                _debugTxt.Text = "N";
                break;
            case LoadoutPageState.Swap:
                _debugTxt.Text = "S";
                break;
            case LoadoutPageState.Equip:
                _debugTxt.Text = "E";
                break;
        }
    }
    
    // Callbacks - Loadout Section
    public void OnSlotSelected(AbilityLoadoutSlot selected)
    {
        switch (State)
        {
            case LoadoutPageState.Normal:
                // Toggle the slot, only one allowed each time
                foreach (var slot in _loadoutSlots)
                {
                    if (slot != selected)
                    {
                        slot.Unselect();
                    }
                    
                }
                break;
            case LoadoutPageState.Equip:
                // Equip the selected skill in this slot
                SetState(LoadoutPageState.Normal);
                break;
            case LoadoutPageState.Swap:
                // Swap the selected slot and the clicked slot.
                SetState(LoadoutPageState.Normal);
                break;
        }
    }

    public void OnSwapClicked(AbilityLoadoutSlot selected)
    {
        // Called after the swap button on the ability slot is clicked
        _selectedSlot?.Unselect();
        _selectedSlot = selected;
        SetState(LoadoutPageState.Swap);
        
        // You can swap the slot with another slot, or swap it with an unequipped ability
    }
    
    // Callbacks - Skill book Section
    public void OnItemSelected(AbilityLoadoutItemGroup selected)
    {
        switch (State)
        {
            case LoadoutPageState.Normal:
                if (!selected.Equipped)
                {
                    // Enter equip state
                    SetState(LoadoutPageState.Equip);
                    _selectedItem = selected;
                }
                break;
            case LoadoutPageState.Equip:
                // Change to the newly selected item
                _selectedItem = selected;
                break;
            case LoadoutPageState.Swap:
                // Swap the selected slot and the clicked item.
                EquipSkill(selected.SkillKey, _selectedSlot.Index);
                SetState(LoadoutPageState.Normal);
                break;
        }
    }
}