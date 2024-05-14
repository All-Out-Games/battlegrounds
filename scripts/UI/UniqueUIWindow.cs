using AO;

/// <summary>
/// Inherit this class to create unique windows like vendor pages. UIManager manages this type of class.
/// When this type of class is open, a path to a prefab with an inherited class on is required:
/// They get instantiated if no previous instance exist (write a static public variable for each)
/// Or we open the existing instancce and close all other instances of UniqueUIWindow (See UIManager->OpenUniqueWindow)
/// </summary>
public class UniqueUIWindow : UIWindow
{
    
    public override void CloseWindow()
    {
        Entity.LocalEnabled = false;
        IsActive = false;
        OnWindowClose?.Invoke(this, Network.LocalPlayer);
    }

    public override void OpenWindow()
    {
        OnWindowOpen?.Invoke(this, Network.LocalPlayer);
        Entity.LocalEnabled = true;
        IsActive = true;
    }
}