
using AO;

public class AbilityVendorWindow : UniqueUIWindow
{
    [Serialized] public UIScrollView AbilityTreeScroll;
    [Serialized] public Entity AbilityNode; // The bg image for the scrollview. Add Ability Items as its children.

    protected Dictionary<string, AbilityItem> AbilityItems = new();
    protected static string AbilityItemPath = "AbilityItem.prefab";
    public override void Start()
    {
        base.Start();
        CreateAllSkillItems();
    }

    /// <summary>
    /// Create and connect all skill nodes that we currently have in the game.
    /// Should only be called once when this window instantiates.
    /// </summary>
    public void CreateAllSkillItems()
    {
        // TODO: Skill Tabs
        UIRect scrollRect = AbilityNode.GetComponent<UIRect>();
        float maxY = 0;
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
            
            maxY = float.Max(maxY, cfg.UIPosition.Y);
            
            item.Entity.SetParent(AbilityNode, false);
            AbilityItems.Add(kv.Key, item);
        }
        scrollRect.Insets = scrollRect.Insets with { Z = -(maxY + 150) };
    }
    
    
    public void UpdateSkillTree(UIWindow window, Player localPlayer)
    {
        FightPlayerSkillTree skillTree = ((FightPlayer)localPlayer).GetSkillTree();
        foreach (var kv in skillTree.SkillLevelDict)
        {
            // TODO: Update ability node status
        }
    }
}