using AO;
using Assembly.scripts.UI.Legacy;

namespace Assembly.scripts.UI.SkillTree;

public class SkillTreePage : UniqueUIWindow
{
    public static Vector2 GetGridPosition(int x, int y)
    {
        return new Vector2(180*(x-1), 600-150*y);
    }

    
    [Serialized] private Entity _skillPageParent;
    [Serialized] private AbilityInfoScreen _infoScreen;
    [Serialized] private UIText _costText, _buyText;
    [Serialized] private UIButton _buyButton;
    private Dictionary<string, SkillTreeItem> _treeItems;
    
    // Tab
    [Serialized] private UIButton _nextTab;
    [Serialized] private UIButton _prevTab;
    [Serialized] private UISkillTabButton[] _tabButtons;
    [Serialized] private UIText _abilityTabName;
    [Serialized] private UIImage _tabBg;
    
    private SkillConfig.SkillTreeTabs _currentTab = SkillConfig.SkillTreeTabs.Basic;
    private int _currentTabIndex = 0;
    private int _tabAmount = 0;
    
    // Skill book Section
    private List<SkillConfig.SkillTreeTabs> _availableTabs;
    
    // Data
    private FightPlayerSkillTree _skillTree;
    private FightPlayerSkillSlotsManager _slotsMgr;
    private SkillTreeItem _selectedItem;
    private FightPlayer _localPlayer;

    public override void Start()
    {
        base.Start();
    }

    public override void OnInstantiate()
    {
        base.OnInstantiate();
        _localPlayer = Network.LocalPlayer.Entity.GetComponent<FightPlayer>();
        _slotsMgr ??= _localPlayer.GetSkillSlots();
        _skillTree ??= _localPlayer.GetSkillTree();
        _treeItems = new Dictionary<string, SkillTreeItem>();
        
        // First Open Phase 1
        // Spawn all skills
        Prefab itemPrefab = Assets.GetAsset<Prefab>("SkillTreeItem.prefab");
        
        foreach (string key in SkillConfig.GetAllSkillKeys())
        {
            SkillConfig.SkillTreeNodeConfig cfg = SkillConfig.GetConfig(key);
            SkillTreeItem itm = itemPrefab.Instantiate().GetComponent<SkillTreeItem>();
            itm.Entity.SetParent(_skillPageParent, false);
            itm.Initialize(this, cfg.GridX, cfg.GridY, cfg.SkillKey);
            itm.ItemButton.OnClicked += itm.OnItemClicked;
            //Log.Error($"Icon for Key {key} : {SkillConfig.GetIconPath(key)}");
            _treeItems.Add(key, itm);
        }
        
        // First Open Phase 2
        // Tab stuff (arrow buttons and tab buttons)
        _availableTabs = new List<SkillConfig.SkillTreeTabs>(SkillConfig.STConfigTabsList);
        _tabAmount = _availableTabs.Count;
        _prevTab.OnClicked += PreviousTab;
        _nextTab.OnClicked += NextTab;
        UpdateSkillTreeTab(SkillConfig.SkillTreeTabs.Basic); // Select Basic Tab (Tab 0)
        
        // First Open Phase 3
        // Tab buttons
        for(int i = 0; i < _tabButtons.Length; i++)
        {
            _tabButtons[i].Initialize(i, OnTabClicked);
        }

        _buyButton.OnClicked += OnBuyButtonClicked;


    }

    public override void OpenWindow()
    {
        base.OpenWindow();
        _localPlayer.CoinUpdateEvent += UpdateAllItems;
        
        SetInfoScreenEnabled(false);
        ResetSelection();
        UpdateAllItems();
        
        Chat.SetChatMode(Chat.Mode.BubbleOnly);
    }

    public override void CloseWindow()
    {
        base.CloseWindow();
        _localPlayer.CoinUpdateEvent -= UpdateAllItems;
        
        Chat.SetChatMode(Chat.Mode.Default);
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
        _abilityTabName.Text = SkillConfig.STTabsNameQueryDict[_currentTab];
        _tabBg.Sprite = Assets.GetAsset<Texture>(SkillConfig.STTabQueryDict[_currentTab].SkillPageBg);
        
        foreach (var kv in _treeItems)
        {
            // Enable items in tab if player owns the skill & skill belongs to this tab
            var item = kv.Value;
            
            if (item.NTab == tab)
            {
                item.Entity.LocalEnabled = true;
            }
            else
            {
                item.Entity.LocalEnabled = false;
            }
        }
    }

    private void UpdateAllItems(int coins = 0)
    {
        var currentAbilities = _slotsMgr.GetCurrentAbilities().Select(ability => ability.SkillKey).ToArray();
        foreach (var item in _treeItems)
        {
            item.Value.UpdateItem(_skillTree,currentAbilities );
        }
    }

    private void ResetSelection()
    {
        _selectedItem = null;
        foreach (var item in _treeItems)
        {
            item.Value.Unselect();
        }
    }
    
    private void SetInfoScreenEnabled(bool enable)
    {
        _infoScreen.Entity.LocalEnabled = enable;
    }

    // Callback - items
    public void OnItemSelected(SkillTreeItem item)
    {
        if(_selectedItem == item) return;
        
        ResetSelection();
        _selectedItem = item;
        _infoScreen.SetDescription(item.Config.SkillKey);
        SetInfoScreenEnabled(true);
        
        // Buy button - check item status
        switch (item.Status)
        {
            case SkillTreeItem.NodeStatus.Purchased:
                _buyText.Text = $"Purchased {item.Config.SkillKey}";
                _buyButton.Interactable = false;
                break;
            case SkillTreeItem.NodeStatus.Attainable:
                _buyText.Text = $"Buy {item.Config.SkillKey}";
                _buyButton.Interactable = true;
                break;
            case SkillTreeItem.NodeStatus.Locked:
                _buyText.Text = "Need Prerequisite!";
                _buyButton.Interactable = false;
                break;
        }
        _costText.Text = $"{item.Config.UpgradeCost}";
        
    }
    // Callback - buy button
    public void OnBuyButtonClicked()
    {
        if (_selectedItem.Status == SkillTreeItem.NodeStatus.Attainable)
        {
            ConfirmOrCancelDialog dialog = UIManager.Instance.OpenUniqueUIWindow(UniqueWindowKeys.AbilityUnlockDialogPath) as ConfirmOrCancelDialog;
            dialog.InitializeWithConfig(_selectedItem.Config, _selectedItem.OnAbilityUpgradeReturn);
        }
    }
    
}