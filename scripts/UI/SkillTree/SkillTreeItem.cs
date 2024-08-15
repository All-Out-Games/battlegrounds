using AO;
namespace Assembly.scripts.UI.SkillTree;

public class SkillTreeItem : Component
{
    public enum NodeStatus
    {
        Locked, // Prerequisite not satisfied
        Attainable, // Prerequisite satisfied
        Purchased, // Bought to Maximum level. (Max Level is either 1 or 4, depending on whether the skill is upgradable by gems)
        Upgradable // Purchased, but now upgraded to max level
    }
    [Serialized] public UIButton ItemButton;

    [Serialized] private Entity _lockedBorder;
    [Serialized] private Entity _equippedBorder;
    [Serialized] private Entity _boughtBorder;
    [Serialized] private Entity _hightlightBorder;
    
    [Serialized] private Entity[] _stars;

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
        
        int lvl = skillTree.GetSkillLevel(Config.SkillKey);
        ShowStars(0);
        if (lvl > 0)
        {
            if (Config.MaximumLevel > 1)
            {
                // Upgradeable
                ShowStars(lvl-1);
            }
            
            if (lvl == Config.MaximumLevel)
            {
                // Level full
                Status = NodeStatus.Purchased;
                _boughtBorder.LocalEnabled = true;
            }
            else
            {
                // [lvl >= 1 && MaxiumLevel != 1] -> Upgradable skill
                Status = NodeStatus.Upgradable;
            }
            
        }
        // Check 2: Parent Level
        else if (CheckAttainable(skillTree))
        {
            Status = NodeStatus.Attainable;
            _boughtBorder.LocalEnabled = false;
            _lockedBorder.LocalEnabled = false;
        }
        // Otherwise...
        else
        {
            Status = NodeStatus.Locked;
            _lockedBorder.LocalEnabled = true;
            _boughtBorder.LocalEnabled = false;
        }
        
        // Check equipped
        _equippedBorder.LocalEnabled = equippedKeys.Contains(Config.SkillKey);

    }

    /// <summary>
    /// Check if the player is eligible to buy the skill that this node represents.
    /// Does not check coins. Coin check moved to the parent page (SkillTreePage.cs)
    /// </summary>
    /// <param name="skillTree"></param>
    /// <returns></returns>
    public bool CheckAttainable(FightPlayerSkillTree skillTree)
    {
        // if (!skillTree.CheckAffordable(Config.UpgradeCost))
        // {
        //     // Not enough money
        //     return false;
        // }
        if (Config.UnlockLevel > skillTree.GetFightPlayer().Level)
        {
            // Not enough level
            return false;
        }
        bool attainable = true;
        foreach (string k in Config.GetParentNodeKeys())
        {
            if (skillTree.GetSkillLevel(k) < 1)
            {
                attainable = false;
                break;
            }
        }
        
        return attainable;
    }

    public void OnAbilityUpgradeReturn(bool confirmed)
    {
        if (confirmed && (Status == NodeStatus.Attainable || Status == NodeStatus.Upgradable))
        {
            TestServerRPC.CallServer_LogSomethingOnServer($"Callback received. Requesting to upgrade {Config.SkillKey}");

            FightPlayer fp = (FightPlayer)Network.LocalPlayer;
            fp.GetSkillTree().CallServer_RequestUpgradeSkill(Config.SkillKey);
        }
        UIManager.Instance.OpenUniqueUIWindow(UniqueWindowKeys.SkillTreePath); // This should refresh everything including the status of this node & its child nodes
    }

    public void OnItemClicked()
    {
        TreePage.OnItemSelected(this);
        _hightlightBorder.LocalEnabled = true;
        
    }

    public void Unselect()
    {
        _hightlightBorder.LocalEnabled = false;
    }

    public void ShowStars(int star)
    {
        for (int i = 0; i < 4; i++)
        {
            _stars[i].LocalEnabled = i < star;
        }
    }
}