
using AO;

namespace Assembly.scripts.UI;

public class ConfirmOrCancelDialog : UniqueUIWindow
{
    [Serialized] protected UIButton ConfirmBtn;
    [Serialized] protected UIButton CancelBtn;
    [Serialized] protected UIText MainTxt;
    
    protected SkillConfig.SkillTreeNodeConfig Config;
    protected Action<bool> OnDialogReturn;
    

    public void InitializeWithConfig(SkillConfig.SkillTreeNodeConfig cfg, Action<bool> callback, string txt = "Buy?")
    {
        Config = cfg;
        OnDialogReturn = callback;
        MainTxt.Text = txt;

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