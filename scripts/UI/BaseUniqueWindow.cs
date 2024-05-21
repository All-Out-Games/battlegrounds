using AO;

public class BaseUniqueWindow : UIWindow
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
}