
using AO;

public class AbilityUnlockDialog : UniqueUIWindow
{
    [Serialized] protected UIButton ConfirmBtn;
    [Serialized] protected UIButton CancelBtn;
    [Serialized] protected UIText MainTxt;
    
    protected SkillConfig.SkillTreeNodeConfig Config;
    public override void CloseWindow()
    {
        base.CloseWindow();
        UIManager.Instance.OpenUniqueUIWindow(UniqueWindowKeys.AbilityVendorPath);
        
        // Pop the ability window back
    }

    public void InitializeWithConfig(SkillConfig.SkillTreeNodeConfig cfg)
    {
        Config = cfg;
    }

    protected void OnConfirm()
    {
        
    }

    protected void OnCancel()
    {
        
    }
}