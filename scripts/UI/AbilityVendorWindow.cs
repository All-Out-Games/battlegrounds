
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
        
        InitializeSkillTreeItem(Network.LocalPlayer as FightPlayer);
    }

    public override void OnInstantiate()
    {
        base.OnInstantiate();
        //PlayerSkillTree ??= ((FightPlayer)Network.LocalPlayer).GetSkillTree();
        PlayerSkillTree ??= (Network.LocalPlayer.Entity.GetComponent<FightPlayer>()).GetSkillTree();
        //Log.Warn($"Local Component ID {PlayerSkillTree.Id}, LocalID {PlayerSkillTree.Entity.Id}");
        if (PlayerSkillTree.Initialized)
        {
            PlayerSkillTree.SkillUpgradeUIEvent += UpdateSkillNode;
        }
        
    }

    public override void OnDestroy()
    {
        PlayerSkillTree.SkillUpgradeUIEvent -= UpdateSkillNode;
        base.OnDestroy();
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

            //ItemSanityCheck(cfg);
            
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
    public void InitializeSkillTreeItem(FightPlayer localPlayer)
    {
        PlayerSkillTree ??= localPlayer.GetSkillTree();
        // We have all skill nodes at this point (after player initialization)
        // Just adjust all skill nodes status
        foreach (var kv in PlayerSkillTree.SkillLevelDict)
        {
            UpdateSkillNode(kv.Key, kv.Value);
        }
    }

    protected void UpdateSkillNode(string skillKey, int level)
    {
        // When a skill is updated, we want to update it as well as its children
        AbilityItems[skillKey].UpdateItem(PlayerSkillTree);
        foreach (string childKey in SkillConfig.STConfigQueryDict[skillKey].GetChildrenNodeKeys())
        {
            AbilityItems[childKey].UpdateItem(PlayerSkillTree);
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
        scrollRect.Insets = scrollRect.Insets with { Z = -(TabScrollHeight[tab] + 150) };
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