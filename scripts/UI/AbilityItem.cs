using AO;

public class AbilityItem : Component
{
    public enum NodeStatus
    {
        Locked,
        Attainable,
        Purchased
    }
    [Serialized] protected UIText SkillKeyText;
    [Serialized] protected UIText CostText;
    [Serialized] protected UIImage SkillIcon; // Unused for now
    [Serialized] protected UIButton ItemButton;

    protected SkillConfig.SkillTreeNodeConfig Config;
    protected UIRect Rect;
    protected NodeStatus Status;
    public SkillConfig.SkillTreeTabs NTab;
    
    protected void OpenUpgradeDialog()
    {
        // TODO: After click, popup a dialog to ask player if they want the upgrade
        Log.Debug($"{Config.SkillKey} clicked in the skill tree");
        switch (Status)
        {
            case NodeStatus.Purchased:
                // Node already unlocked
                break; 
            case NodeStatus.Attainable:
                AbilityUnlockDialog dialog = UIManager.Instance.OpenUniqueUIWindow(UniqueWindowKeys.AbilityUnlockDialogPath) as AbilityUnlockDialog;
                dialog.InitializeWithConfig(Config, OnAbilityUpgradeReturn);
                break;
            case NodeStatus.Locked:
                break;
        }
    }

    public void InitializeWithConfig(SkillConfig.SkillTreeNodeConfig cfg)
    {
        NTab = cfg.NTab;
        
        Rect = Entity.GetComponent<UIRect>();
        Rect.Offset = cfg.UIPosition;
        
        Config = cfg;
        SkillKeyText.Text = cfg.SkillKey;
        

        ItemButton.OnClicked += OpenUpgradeDialog;
    }

    public void UpdateItem(FightPlayerSkillTree skillTree)
    {
        // UI is updated after skillTree has been synced.
        // Every (related) item call this once the skill tree changes
        Log.Debug($"Ability Item: {Config.SkillKey}; Level = {skillTree.SkillLevelDict[Config.SkillKey]}");
        // Check 1: Self Level
        if (skillTree.SkillLevelDict[Config.SkillKey] > 0)
        {
            Status = NodeStatus.Purchased;
            CostText.Text = "Unlocked";
        }
        // Check 2: Parent Level
        else if (CheckAttainable(skillTree))
        {
            Status = NodeStatus.Attainable;
            CostText.Text = $"Cost: {Config.UpgradeCost}";
        }
        // Otherwise...
        else
        {
            Status = NodeStatus.Locked;
            CostText.Text = "Need Prerequisite";
            // TODO: No APIs to disable UI buttons yet
        }
        
    }

    public bool CheckAttainable(FightPlayerSkillTree skillTree)
    {
        bool attainable = true;
        foreach (string k in Config.GetParentNodeKeys())
        {
            if (skillTree.SkillLevelDict[k] <= 0)
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
            fp.GetSkillTree().CallServer_UpgradeSkill(Config.SkillKey, Config.MaximumLevel);
        }
    }
}