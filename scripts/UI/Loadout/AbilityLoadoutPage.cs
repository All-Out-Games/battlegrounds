using AO;
using Assembly.scripts.UI.SkillTree;

namespace Assembly.scripts.UI;

public class AbilityLoadoutPage : UniqueUIWindow
{
    public enum LoadoutPageState
    {
        Normal, // Nothing selected
        Swap, // Swapping a skill slot (with another slot or an item in skill book section)
        Equip // Trying to equip a skill from skill book session
    }
    [Serialized] private UIText _debugTxt;
    
    // Loadout Section (skill slots)
    [Serialized] private UIImage _punchIcon;
    [Serialized] private AbilityLoadoutSlot[] _loadoutSlots;

    [Serialized] private UIButton _nextTab;
    [Serialized] private UIButton _prevTab;

    [Serialized] private UISkillTabButton[] _tabButtons;
    
    
    
    // Skill book Section

    private List<SkillConfig.SkillTreeTabs> _availableTabs;
    
    [Serialized] private UIText _abilityTabName;
    [Serialized] private UIText _emptyTabText;
    [Serialized] private UIGrid _skillList;

    // TODO: Info popup
    [Serialized] private AbilityInfoScreen _infoScreen;
    
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
    
    private AbilityLoadoutSlot _slotForSwap = null; // Slot refers to one of the 5 swappable slots in the Loadout Screen
    private AbilityLoadoutItemGroup _selectedItem = null; // Item refers to the purchased skills in the item lists

    public override void OnInstantiate()
    {
        base.OnInstantiate();
        
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
        Prefab itemPrefab = Assets.GetAsset<Prefab>("LoadoutItemGroup.prefab");
        
        foreach (string key in SkillConfig.GetAllSkillKeys())
        {
            
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

                itm.Button.OnClicked += itm.OnItemSelected;
                itm.InfoButton.OnClicked += itm.OnInfoButtonSelected;
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
        // Initialize Loadout Slots
        for (int i = 0; i < 5; i++)
        {
            var slot = _loadoutSlots[i];
            slot.Initialize(this, i+1); // This index is the actual index in the _equippedSkillKey array
            slot.SkillButton.OnClicked += slot.Toggle;
            slot.SwapButton.OnClicked += slot.OnSwapClicked;
            slot.RemoveButton.OnClicked += slot.OnRemoveClicked;
        }
        
        // First Open Phase 4
        // Associate Tab Buttons
        for(int i = 0; i < _tabButtons.Length; i++)
        {
            _tabButtons[i].Initialize(i, OnTabClicked);
        }
        
        // First Open Phase 5
        // Info Screen
        _infoScreen.InfoQuitBtn.OnClicked += () => { SetInfoScreenEnabled(false); };
        
        ResetSelection();
        
    }
    
    public void ResetSelection()
    {
        // Unselect loadout slots
        _slotForSwap = null;
        foreach (var slot in _loadoutSlots)
        {
            slot.Unselect();
        }

        // Unselect skill book items
        _selectedItem = null;
        RefreshItemEquippedBorders();
        
        SetState(LoadoutPageState.Normal);
        
        //Log.Warn($"Equipped Skills {_equippedSkillKey[0]} {_equippedSkillKey[1]} {_equippedSkillKey[2]} {_equippedSkillKey[3]} {_equippedSkillKey[4]} {_equippedSkillKey[5]}");
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
        SaveSkills();
        
        Chat.SetChatMode(Chat.Mode.Default);
    }

    public override void OpenWindow()
    {
        UpdateSkillTreeTab(_currentTab);
        FetchEquippedSkills();
        base.OpenWindow();

        _elapsedTimeSinceOpen = 0;
        ResetSelection();
        
        Chat.SetChatMode(Chat.Mode.BubbleOnly);
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

    private void OnTabClicked(int index)
    {
        _currentTabIndex = index;
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

        _emptyTabText.Entity.LocalEnabled = !haveItemsFlag; // Enable when the tab is empty 
    }

    private void FetchEquippedSkills()
    {
        // Put equipped abilities in slots
        List<FightAbility> faList = _slotsMgr.GetCurrentAbilities();
        _equippedSkillKey = faList.Select(a => a.SkillKey).ToArray();
        // Punch
        string punchKey = "Punch";
        if (_slotsMgr.GetFightPlayer().PunchLevel > 1) punchKey = $"Punch{_slotsMgr.GetFightPlayer().PunchLevel}";
        _punchIcon.Sprite = Assets.KeepLoaded<Texture>(SkillConfig.GetIconPath(punchKey));
        for (int i = 0; i < 5; i++)
        {
            _loadoutSlots[i].SetSkillKey(_equippedSkillKey[i+1]);
        }
    }

    private void RefreshItemEquippedBorders()
    {
        foreach (var kv in _bookItems)
        {
            // Enable items in tab if player owns the skill & skill belongs to this tab
            var item = kv.Value;
            item.SetItemEquipped(_equippedSkillKey.Contains(kv.Key));
            item.SetItemHighlighted(_selectedItem == item);
        }
    }
    
    /// <summary>
    /// [Important Function]
    /// Equip a skill by key and index. Will remove duplicate if you try to equip an already equipped skill in another slot.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="idx"></param>
    private void EquipSkill(string key, int idx)
    {
        // Phase 1. Remove Duplicated Skill, if detected
        for (int i = 1; i < 6; i++)
        {
            var slot = _loadoutSlots[i-1]; // slot idx need i-1 because of punch 
            if (slot.SkillKey == key)
            {
                if (i == idx) return; // Equipping the same key. 
                RemoveSkill(i);
            }
        }
        // Phase 2. Put the in the new skill
        _equippedSkillKey[idx] = key;
        _loadoutSlots[idx-1].SetSkillKey(key);

    }

    /// <summary>
    /// [Important Function]
    /// Remove a skill slot, by equipping an Empty key onto it
    /// </summary>
    /// <param name="idx"></param>
    private void RemoveSkill(int idx)
    {
        if (idx == 0)
        {
            Log.Error("You can't remove Punch!");
            return;
        }
        // Equip an Empty skill at the slot.
        _equippedSkillKey[idx] = FightAbility.DefaultSkillKey;
        
        // Remove the slot (Punch does not have a slot so the slot index need -1)
        _loadoutSlots[idx-1].SetSkillKey(FightAbility.DefaultSkillKey);
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
        
        // Change replace icons based on stated
        foreach (var kv in _bookItems)
        {
            // Enable items in tab if player owns the skill & skill belongs to this tab
            var item = kv.Value;
            item.OnParentStateChange(st); // Replace icon appear only in swap state
        }

        foreach (var slot in _loadoutSlots)
        {
            slot.OnParentStateChange(st); // Replace icon appear in swap state / equip state
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
    }

    private void SetInfoScreenEnabled(bool enable)
    {
        _infoScreen.Entity.LocalEnabled = enable;
    }
    
    // Callbacks - Loadout Section
    public void OnSlotSelected(AbilityLoadoutSlot selected)
    {
        switch (State)
        {
            case LoadoutPageState.Normal:
                // The selected slot will toggle itself, here we untoggle all others
                foreach (var slot in _loadoutSlots)
                {
                    if (slot != selected)
                    {
                        slot.Unselect();
                    }
                }
                break;
            case LoadoutPageState.Equip:
                if (_selectedItem == null)
                {
                    Log.Error("We should always have an item selected when the state is Equip!");
                    return;
                }
                // Equip the skill on the slot clicked
                EquipSkill(_selectedItem.SkillKey, selected.Index);
                
                RefreshItemEquippedBorders();
                
                ResetSelection();
                break;
            case LoadoutPageState.Swap:
                // Swap the selected slot and the clicked slot.
                if (_slotForSwap == null)
                {
                    Log.Error("We should always have a slot for swap when the state is Swap!");
                    return;
                }
                if (selected != _slotForSwap)
                {
                    string keyA = selected.SkillKey; // cahche before swap
                    string keyB = _slotForSwap.SkillKey;
                    
                    EquipSkill(keyA, _slotForSwap.Index);
                    EquipSkill(keyB, selected.Index);
                }
                ResetSelection();
                break;
        }
    }

    public void OnSwapClicked(AbilityLoadoutSlot selected)
    {
        // Called after the swap button on the ability slot is clicked
        _slotForSwap = selected;
        SetState(LoadoutPageState.Swap);
        
        // You can swap the slot with another slot, or swap it with an unequipped ability
        // These logics are handled in OnSlotSelected / OnItemSelected, in the respective branch.
    }

    public void OnRemoveClicked(AbilityLoadoutSlot selected)
    {
        _slotForSwap?.Unselect();
        _slotForSwap = null;
        selected.Unselect();
        RemoveSkill(selected.Index);
        RefreshItemEquippedBorders();
        ResetSelection();
    }
    
    // Callbacks - Skill book Section
    public void OnItemSelected(AbilityLoadoutItemGroup selected)
    {
        Log.Warn($"Selected {selected.SkillKey}");
        switch (State)
        {
            case LoadoutPageState.Normal:
                ResetSelection();
                _selectedItem = selected;
                SetState(LoadoutPageState.Equip);
                RefreshItemEquippedBorders();
                break;
            case LoadoutPageState.Equip:
                if (_selectedItem == null)
                {
                    Log.Error("We should always have an item selected when the state is Equip!");
                    return;
                }
                if (_selectedItem == selected) // click the highlighted item again -> Unselect
                {
                    _selectedItem = null;
                    SetState(LoadoutPageState.Normal);
                }
                else
                {
                    // Change to the newly selected item. Remain in equip state
                    _selectedItem = selected;
                    Log.Warn($"{selected.SkillKey} Selected");
                }
                RefreshItemEquippedBorders();
                break;
            case LoadoutPageState.Swap:
                if (_slotForSwap == null)
                {
                    Log.Error("We should always have a slot for swap when the state is Swap!");
                    return;
                }
                // Equip the selected slot with the clicked item.
                EquipSkill(selected.SkillKey, _slotForSwap.Index);
                RefreshItemEquippedBorders();
                ResetSelection();
                break;
        }
    }

    public void OnItemInfoClicked(AbilityLoadoutItemGroup selected)
    {
        SetInfoScreenEnabled(true);
        _infoScreen.SetDescription(selected.SkillKey);
    }
}