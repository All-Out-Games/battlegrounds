using AO;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.UI.Legacy;

namespace Assembly.scripts.UI.SkillTree;

public class SkillTreePage : UniqueUIWindow
{
    public static Vector2 GridOrigin = new Vector2(0, 225);
    public static Vector2 GridInterval = new Vector2(150, 150);
    public static Vector2 GetGridPosition(int x, int y)
    {
        return new Vector2(GridInterval.X*(x-1), GridOrigin.Y-GridInterval.Y*y);
    }

    
    [Serialized] private Entity _skillPageParent;
    [Serialized] private AbilityInfoScreen _infoScreen;
    [Serialized] private UIText _costText, _buyText;
    [Serialized] private UIButton _buyButton;
    private Dictionary<string, SkillTreeItem> _treeItems;
    private Dictionary<Tuple<int, int>, SkillTreePipes> _treePipes;
    
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
        Rect screenRect = AO.UI.SafeRect;

        //Log.Warn($"Rect {AO.UI.ScreenRect.Width},{AO.UI.ScreenRect.Height}; SafeRect {AO.UI.SafeRect.Width}, {AO.UI.SafeRect.Height};  ScaleFactor {AO.UI.ScreenScaleFactor}");
        //GridFactor = new Vector2(1920f / screenRect.Width,1080f / screenRect.Height) * AO.UI.ScreenScaleFactor; // Scale grid
        
        //GridOrigin *= GridFactor;
        //GridOrigin.Y = float.Min(600, GridOrigin.Y);
        
        //GridInterval *= GridFactor;
        //GridInterval.X = float.Min(150, GridInterval.X);
        
        _localPlayer = Network.LocalPlayer.Entity.GetComponent<FightPlayer>();
        _slotsMgr ??= _localPlayer.GetSkillSlots();
        _skillTree ??= _localPlayer.GetSkillTree();
        _treeItems = new Dictionary<string, SkillTreeItem>();
        _treePipes = new Dictionary<Tuple<int, int>, SkillTreePipes>();
        
        
        // First Open Phase 1
        // Pipes
        Prefab itemPrefab = Assets.GetAsset<Prefab>("SkillTreeItem.prefab");
        Prefab pipe1Prefab = Assets.GetAsset<Prefab>("Pipe_1.prefab");
        Prefab pipePrefab = Assets.GetAsset<Prefab>("Pipe_Straight.prefab");
        
        // First pipe at (1,0)
        SkillTreePipes firstPipe = pipe1Prefab.Instantiate().GetComponent<SkillTreePipes>();
        firstPipe.SetOffset(GetGridPosition(1,0) with {Y = GridOrigin.Y - 0.5f* GridInterval.Y});
        firstPipe.Entity.SetParent(_skillPageParent, false);
        _treePipes.Add(Tuple.Create(1,0),firstPipe);
        // Other pipes up to (2,3)
        // In function UpdateAllItems, we update pipes with the node's current information.
        for (int i = 0; i <= GlobalData.GridMaxX; i++)
        {
            for (int j = 1; j <= GlobalData.GridMaxY; j++)
            {
                SkillTreePipes pipe = pipePrefab.Instantiate().GetComponent<SkillTreePipes>();
                Vector2 pos = GetGridPosition(i, j);
                pipe.SetOffset(pos with{ Y = pos.Y - 0.5f* GridInterval.Y});
                pipe.Entity.SetParent(_skillPageParent, false);
                _treePipes.Add(Tuple.Create(i,j), pipe);
            }
        }
        
        // Spawn all skills
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
        
        
        // First Open Phase 3
        // Tab buttons
        for(int i = 0; i < _tabButtons.Length; i++)
        {
            _tabButtons[i].Initialize(i, OnTabClicked);
        }

        _buyButton.OnClicked += OnBuyButtonClicked;

        UpdateSkillTreeTab(SkillConfig.SkillTreeTabs.Basic); // Select Basic Tab (Tab 0)

    }

    public override void OpenWindow()
    {
        base.OpenWindow();
        _localPlayer.CoinUpdateEvent += UpdateAllItems;
        
        SetInfoScreenEnabled(false);
        ResetSelection();
        UpdateAllItems();
        SFX.Play(SFXKeys.SkillShopAudio, new SFX.PlaySoundDesc());
        
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
        
        foreach (var tabBtn in _tabButtons)
        {
            if (tabBtn.Index == _currentTabIndex)
            {
                tabBtn.SetToggled(true);
            }
            else
            {
                tabBtn.SetToggled(false);
            }
        }

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
        UpdatePipe();
    }

    private void UpdateAllItems(int coins = 0)
    {
        var currentAbilities = _slotsMgr.GetCurrentAbilities().Select(ability => ability.SkillKey).ToArray();
        foreach (var item in _treeItems)
        {
            item.Value.UpdateItem(_skillTree,currentAbilities );
        }
        UpdatePipe();
    }

    private void UpdatePipe()
    {
        // Hide all pipes
        foreach (var pipe in _treePipes)
        {
            pipe.Value.Entity.LocalEnabled = false;
        }
        // Enable the ones related to current tree
        foreach (var kv in _treeItems)
        {
            // Enable items in tab if player owns the skill & skill belongs to this tab
            var item = kv.Value;
            
            if (item.NTab == _currentTab)
            {
                if (item.Config.ChildrenNodeKeys.Length == 0)
                {
                    continue; // If it's the last skill, don't show the pipe under it.
                }
                SkillTreePipes pipes;
                if (!_treePipes.TryGetValue(Tuple.Create(item.Config.GridX, item.Config.GridY), out pipes))
                {
                    Log.Error("You queried a tree grid position out of range! Did you expand the skill tree's max dimension?");
                    return;
                }

                pipes.Entity.LocalEnabled = true;
                pipes.SetFilled(item.Status == SkillTreeItem.NodeStatus.Purchased);
            }
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
                _buyText.Text = $"Purchased {item.Config.GetDisplayName()}";
                _buyButton.Interactable = false;
                break;
            case SkillTreeItem.NodeStatus.Attainable:
                if (_localPlayer.Coins >= item.Config.UpgradeCost)
                {
                    _buyText.Text = $"Buy {item.Config.GetDisplayName()}";
                    _buyButton.Interactable = true;
                }
                else
                {
                    _buyText.Text = $"Earn {item.Config.UpgradeCost - _localPlayer.Coins} Coins!";
                    _buyButton.Interactable = false;
                }
                
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