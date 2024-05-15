
using AO;

public class AbilityVendorWindow : UniqueUIWindow
{
    [Serialized] public UIScrollView AbilityTreeScroll;
    [Serialized] public Entity AbilityNode; // The bg image for the scrollview. Add Ability Items as its children.
    [Serialized] public UIButton NextButton;
    [Serialized] public UIButton PrevButton;

    protected Dictionary<string, AbilityItem> AbilityItems = new();
    protected Dictionary<SkillConfig.SkillTreeTabs, float> TabScrollHeight; // Cached total scroll height of each tab.
    protected static string AbilityItemPath = "AbilityItem.prefab";
    
    protected SkillConfig.SkillTreeTabs CurrentTab = SkillConfig.SkillTreeTabs.Basic;
    protected int CurrentTabIndex = 0;
    protected int TabAmount = 0;
    public override void Start()
    {
        base.Start();

        TabAmount = SkillConfig.STConfigTabsList.Count;

        NextButton.OnClicked += NextTab;
        PrevButton.OnClicked += PreviousTab;
        
        CreateAllSkillItems();
        UpdateSkillTreeTab();
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
            item.InitializeWithConfig(cfg);
            
            item.Entity.SetParent(AbilityNode, false);
            AbilityItems.Add(kv.Key, item);
        }
        
    }
    
    /// <summary>
    /// Function called after player makes a change to the skill tree. (Called in Client RPC, after server uprate)
    /// </summary>
    /// <param name="window"></param>
    /// <param name="localPlayer"></param>
    public void UpdateSkillTree(UIWindow window, Player localPlayer)
    {
        FightPlayerSkillTree skillTree = ((FightPlayer)localPlayer).GetSkillTree();
        foreach (var kv in skillTree.SkillLevelDict)
        {
            // TODO: Update ability node status
        }
    }

    /// <summary>
    /// Update the current skill tree tab. Only display items within the current skill tree.
    /// TODO: Also generate the connection using IM.DrawLine()
    /// </summary>
    private void UpdateSkillTreeTab(SkillConfig.SkillTreeTabs tab = SkillConfig.SkillTreeTabs.Basic)
    {
        CurrentTab = tab;
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