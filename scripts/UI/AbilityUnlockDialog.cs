
using AO;

public class AbilityUnlockDialog : UniqueUIWindow
{
    [Serialized] protected UIButton ConfirmBtn;
    [Serialized] protected UIButton CancelBtn;
    [Serialized] protected UIText MainTxt;
    
    protected SkillConfig.SkillTreeNodeConfig Config;
    protected Action<bool> OnDialogReturn;
    public override void CloseWindow()
    {
        base.CloseWindow();
        
        // Pop the ability window back
        UIManager.Instance.OpenUniqueUIWindow(UniqueWindowKeys.AbilityVendorPath);
    }
    

    public void InitializeWithConfig(SkillConfig.SkillTreeNodeConfig cfg, Action<bool> callback)
    {
        Config = cfg;
        OnDialogReturn = callback;
        MainTxt.Text = "Buy?";

        ConfirmBtn.OnClicked += OnConfirm;
        ConfirmBtn.OnClicked += CloseWindow;
        
        CancelBtn.OnClicked += OnCancel;
        CloseButton.OnClicked += OnCancel;
        
    }

    protected void OnConfirm()
    {
        OnDialogReturn?.Invoke(true);
        OnDialogReturn = null;
        
        Deactivate();
    }

    protected void OnCancel()
    {
        OnDialogReturn?.Invoke(false);
        OnDialogReturn = null;
        
        Deactivate();
    }

    protected void Deactivate()
    {
        ConfirmBtn.OnClicked -= OnConfirm;
        ConfirmBtn.OnClicked -= CloseWindow;
        CancelBtn.OnClicked -= OnCancel;
        CloseButton.OnClicked -= OnCancel;
    }
}