using AO;
namespace Assembly.scripts.UI.SkillTree;

public class SkillTreeItem : Component
{
    public enum NodeStatus
    {
        Locked,
        Attainable,
        Purchased
    }
    [Serialized] public UIButton ItemButton;

    [Serialized] private Entity _lockedBorder;
    [Serialized] private Entity _equippedBorder;
    [Serialized] private Entity _boughtBorder;
    [Serialized] private Entity _hightlightBorder;

    public SkillTreePage TreePage;
    public SkillConfig.SkillTreeNodeConfig Config;
    protected UIRect Rect;
    
    public NodeStatus Status;
    public SkillConfig.SkillTreeTabs NTab;

    public void Initialize(SkillTreePage parentPage, int gridX, int gridY, string skillkey)
    {
        Rect = GetComponent<UIRect>();
        
        TreePage = parentPage;
        Rect.Offset = SkillTreePage.GetGridPosition(gridX, gridY);
        Config = SkillConfig.STConfigQueryDict[skillkey];
        NTab = Config.NTab;
        
        FightClubUtils.SetButtonTexture(ItemButton, Config.IconPath);
    }
    
    public void UpdateItem(FightPlayerSkillTree skillTree, string[] equippedKeys)
    {
        // UI is updated after skillTree has been synced.
        // Every item calls this once the skill tree changes
        
        //Log.Debug($"Ability Item: {Config.SkillKey}; Level = {skillTree.SkillLevelDict[Config.SkillKey]}");
        
        // Check 1: Self Level
        if (skillTree.SkillLevelDict[Config.SkillKey] > 0)
        {
            Status = NodeStatus.Purchased;
        }
        // Check 2: Parent Level
        else if (CheckAttainable(skillTree))
        {
            Status = NodeStatus.Attainable;
        }
        // Otherwise...
        else
        {
            Status = NodeStatus.Locked;
        }
        
        // Check equipped
        _equippedBorder.LocalEnabled = equippedKeys.Contains(Config.SkillKey);

    }

    public bool CheckAttainable(FightPlayerSkillTree skillTree)
    {
        // if (!skillTree.CheckAffordable(Config.UpgradeCost))
        // {
        //     // Not enough money
        //     return false;
        // }
        
        bool attainable = true;
        foreach (string k in Config.GetParentNodeKeys())
        {
            if (skillTree.SkillLevelDict[k] < 1)
            {
                attainable = false;
                break;
            }
        }
        

        return attainable;
    }

    protected void OnAbilityUpgradeReturn(bool confirmed)
    {
        if (confirmed && Status == NodeStatus.Attainable)
        {
            // TODO: Request update from player skill tree
            TestServerRPC.CallServer_LogSomethingOnServer($"Callback received. Requesting to upgrade {Config.SkillKey}");

            FightPlayer fp = (FightPlayer)Network.LocalPlayer;
            fp.GetSkillTree().CallServer_RequestUpgradeSkill(Config.SkillKey);
        }
    }

    public void OnItemClicked()
    {
        _hightlightBorder.LocalEnabled = true;
        TreePage.OnItemSelected(this);
    }

    public void Unselect()
    {
        _hightlightBorder.LocalEnabled = false;
    }
}