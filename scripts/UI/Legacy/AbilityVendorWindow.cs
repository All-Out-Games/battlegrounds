
using AO;

public class AbilityVendorWindow : UniqueUIWindow
{
    [Serialized] public UIScrollView AbilityTreeScroll;
    [Serialized] public Entity AbilityNode; // The bg image for the scrollview. Add Ability Items as its children.
    [Serialized] public UIButton NextButton;
    [Serialized] public UIButton PrevButton;
    [Serialized] public UIText AbilityTabName; // the name of tab. Query from SkillTreeConfig
    
    protected Dictionary<SkillConfig.SkillTreeTabs, float> TabScrollHeight; // Cached total scroll height of each tab.
    protected SkillConfig.SkillTreeTabs CurrentTab = SkillConfig.SkillTreeTabs.Basic;
    protected int CurrentTabIndex = 0;
    protected int TabAmount = 0;

    protected FightPlayerSkillTree PlayerSkillTree;
    protected FightPlayer LocalPlayer;
    protected Dictionary<string, AbilityItem> AbilityItems = new();
    protected static string AbilityItemPath = "AbilityItem.prefab";
    public override void Start()
    {
        base.Start();

        TabAmount = SkillConfig.STConfigTabsList.Count;

        NextButton.OnClicked += NextTab;
        PrevButton.OnClicked += PreviousTab;
        
        CreateAllSkillItems();
        UpdateSkillTreeTab();
        
        InitializeTreeItems(Network.LocalPlayer as FightPlayer);
    }

    public override void OnInstantiate()
    {
        base.OnInstantiate();
        LocalPlayer = Network.LocalPlayer.Entity.GetComponent<FightPlayer>();
        PlayerSkillTree ??= LocalPlayer.GetSkillTree();
        //Log.Warn($"Local Component ID {PlayerSkillTree.Id}, LocalID {PlayerSkillTree.Entity.Id}");
        if (PlayerSkillTree.Initialized)
        {
            LocalPlayer.CoinUpdateEvent += UpdateAllSkillNode;
        }
        
    }

    public override void OnDestroy()
    {
        LocalPlayer.CoinUpdateEvent -= UpdateAllSkillNode;
        base.OnDestroy();
    }

    public override void OpenWindow()
    {
        base.OpenWindow();
        if (TabAmount > 0)
        {
            // Flush update, excl. the instantiation. [This function need to be called after Start()]
            InitializeTreeItems(Network.LocalPlayer as FightPlayer);
        }
        
    }


    /// <summary>
    /// Create and connect all skill nodes that we currently have in the game.
    /// Should only be called once when this window instantiates.
    /// </summary>
    public void CreateAllSkillItems()
    {
        foreach (var kv in SkillConfig.STConfigQueryDict)
        {
            SkillConfig.SkillTreeNodeConfig cfg = kv.Value;
            Log.Debug($"Node {cfg.SkillKey}: Position: {cfg.UIPosition}");
            Prefab abilityItemPrefab = Assets.GetAsset<Prefab>(AbilityItemPath);
            AbilityItem item = abilityItemPrefab.Instantiate().GetComponent<AbilityItem>();
            
            if (item == null)
            {
                Log.Error("Cannot find AbilityItem component on Ability Item prefab");
                break;
            }

            if (Network.LocalPlayer.IsAdmin)
            {
                // We run a sanity check function for Admin players to see if there's parent/children node misaligned.
                ItemSanityCheck(cfg);
            }
            
            item.InitializeWithConfig(cfg);
            item.Entity.SetParent(AbilityNode, false);
            AbilityItems.Add(kv.Key, item);
        }
        
    }

    /// <summary>
    /// Check skill tree node config.
    /// This will report error if it detects defects in config
    /// </summary>
    /// <param name="cfg"></param>
    protected void ItemSanityCheck(SkillConfig.SkillTreeNodeConfig cfg)
    {
        Log.Debug($"Skill Tree Sanity: Checking {cfg.SkillKey}...");
        // If a node has child, all its children must also appoint it as the parent
        
        foreach (var k in cfg.GetChildrenNodeKeys())
        {
            var childCfg = SkillConfig.STConfigQueryDict[k];
            Log.Debug($"Skill Tree Sanity: Checking Child {childCfg.SkillKey}...");
            if (!childCfg.GetParentNodeKeys().Contains(cfg.SkillKey))
            {
                Log.Error($"{childCfg.SkillKey}, Child of node {cfg.SkillKey} did not set it as parent!");
            }
        }

        foreach (var k in cfg.GetParentNodeKeys())
        {
            var parentCfg = SkillConfig.STConfigQueryDict[k];
            Log.Debug($"Skill Tree Sanity: Checking parent {parentCfg.SkillKey}...");
            if (!parentCfg.GetChildrenNodeKeys().Contains(cfg.SkillKey))
            {
                Log.Error($"{parentCfg.SkillKey}, Parent of node {cfg.SkillKey} did not set it as child!");
            }
        }
    }
    
    /// <summary>
    /// The first update function, after player skill dict fetched
    /// </summary>
    /// <param name="localPlayer"></param>
    public void InitializeTreeItems(FightPlayer localPlayer)
    {
        LocalPlayer ??= localPlayer;
        PlayerSkillTree ??= localPlayer.GetSkillTree();
        // We have all skill nodes at this point (after player initialization)
        // Just adjust all skill nodes status
        UpdateAllSkillNode(0);
    }
    

    protected void UpdateAllSkillNode(int _)
    {
        if (LocalPlayer.PlayerStatus == PlayerStatus.Combat)
        {
            return;
        }
        foreach (var item in AbilityItems)
        {
            item.Value.UpdateItem(PlayerSkillTree);
        }
    }

    /// <summary>
    /// Update the current skill tree tab. Only display items within the current skill tree.
    /// TODO: Also generate the connection using IM.DrawLine()
    /// </summary>
    private void UpdateSkillTreeTab(SkillConfig.SkillTreeTabs tab = SkillConfig.SkillTreeTabs.Basic)
    {
        CurrentTab = tab;
        AbilityTabName.Text = SkillConfig.STTabsNameQueryDict[tab];
        foreach (var kv in AbilityItems)
        {
            // a. Enable items in tab
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
        
        // b. Stretch the scroll view inlet
        if (TabScrollHeight == null)
        {
            // build the cache for every tab, if not built yet
            TabScrollHeight = new Dictionary<SkillConfig.SkillTreeTabs, float>();
            
            foreach (var tabkey in SkillConfig.STConfigTabsList)
            {
                TabScrollHeight.Add(tabkey, 0);
            }
            
            foreach (var kv in AbilityItems)
            {
                SkillConfig.SkillTreeNodeConfig cfg = SkillConfig.STConfigQueryDict[kv.Key];
                TabScrollHeight[cfg.NTab] = float.Max(TabScrollHeight[cfg.NTab], cfg.UIPosition.Y);
            }
            
            
        }
        UIRect scrollRect = AbilityNode.GetComponent<UIRect>();
        scrollRect.Insets = scrollRect.Insets with { Z = -(TabScrollHeight[tab])/2 };
        
    }

    private void PreviousTab()
    {
        CurrentTabIndex -= 1;
        if (CurrentTabIndex == -1)
        {
            CurrentTabIndex = TabAmount - 1;
        }
        UpdateSkillTreeTab(SkillConfig.STConfigTabsList[CurrentTabIndex]);
    }

    private void NextTab()
    {
        CurrentTabIndex += 1;
        if (CurrentTabIndex >= TabAmount)
        {
            CurrentTabIndex = 0;
        }
        UpdateSkillTreeTab(SkillConfig.STConfigTabsList[CurrentTabIndex]);
    }
}