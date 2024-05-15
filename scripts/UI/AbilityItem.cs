using AO;

public class AbilityItem : Component
{
    [Serialized] protected UIText SkillKeyText;
    [Serialized] protected UIText CostText;
    [Serialized] protected UIImage SkillIcon; // Unused for now
    [Serialized] protected UIButton ItemButton;

    protected SkillConfig.SkillTreeNodeConfig Config;
    protected UIRect Rect;
    public SkillConfig.SkillTreeTabs NTab;
    protected void OpenUpgradeDialog()
    {
        // TODO: After click, popup a dialog to ask player if they want the upgrade
        Log.Debug($"{Config.SkillKey} clicked in the skill tree");
    }

    public void InitializeWithConfig(SkillConfig.SkillTreeNodeConfig cfg)
    {
        NTab = cfg.NTab;
        
        Rect = Entity.GetComponent<UIRect>();
        Rect.Offset = cfg.UIPosition;
        
        Config = cfg;
        SkillKeyText.Text = cfg.SkillKey;
        CostText.Text = $"Cost: {cfg.UpgradeCost}";

        ItemButton.OnClicked += OpenUpgradeDialog;
    }

    public void UpdateItem()
    {
        
    }
}