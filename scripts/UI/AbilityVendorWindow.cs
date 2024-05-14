
using AO;

public class AbilityVendorWindow : UniqueUIWindow
{
    [Serialized] public UIScrollView AbilityTreeScroll;

    public override void Start()
    {
        base.Start();
        CreateAllSkillNodes();
    }

    /// <summary>
    /// Create and connect all skill nodes that we currently have in the game.
    /// Should only be called once when this window instantiates.
    /// </summary>
    public void CreateAllSkillNodes()
    {
        foreach (var kv in SkillConfig.STConfigQueryDict)
        {
            SkillConfig.SkillTreeNodeConfig cfg = kv.Value;
            // TODO: Ability item initialize
            Log.Debug($"Node {cfg.SkillKey}: Position: {cfg.UIPosition}");
        }
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