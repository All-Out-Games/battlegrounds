using AO;

public class BaseUIWindow : UIWindow
{
    public override void CloseWindow()
    {
        Log.Debug($"{Entity.Name} Unique Window Closed");
        Entity.LocalEnabled = false;
        IsActive = false;
        OnWindowClose?.Invoke(this, Network.LocalPlayer);
    }

    public override void OpenWindow()
    {
        Log.Debug($"{Entity.Name} Unique Window Open");
        OnWindowOpen?.Invoke(this, Network.LocalPlayer);
        Entity.LocalEnabled = true;
        IsActive = true;
    }
    
    public virtual void OnInstantiate()
    {
        // Called after first prefab creation. [BEFORE START]
        // Unique windows don't typically get destroyed after that
        // So handle any update from the player using events.
        // If you need player data here, it's best to ensure the window is created after player load
        // i.e. the earlier time point you should call UIManager.Instance.OpenUniqueUIWindow is probably player's start
    }
}