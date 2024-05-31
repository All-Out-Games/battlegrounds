using AO;

/// <summary>
/// Inherit this class to create unique windows like vendor pages. UIManager manages this type of class.
/// When this type of class is open, a path to a prefab with an inherited class on is required:
/// They get instantiated if no previous instance exist (write a static public variable for each)
/// Or we open the existing instancce and close all other instances of UniqueUIWindow (See UIManager->OpenUniqueWindow)
/// </summary>
public class UniqueUIWindow : BaseUniqueWindow
{
    [Serialized] protected UIButton CloseButton; // Unique UI Window must have a close button.
    public override void Start()
    {
        Log.Debug("Start Function called in UniqueUIWindow");
        CloseButton ??= Entity.TryGetChildByName("CloseButton").GetComponent<UIButton>();
        if (CloseButton == null)
        {
            Log.Error($"UniqueUIWindow: Could not find \"CloseButton\" on {Entity.Name}");
            Entity.Destroy();
            return;
        }
        //base.Start();
        CloseButton.OnClicked += CloseWindow;
    }

    public virtual void OnInstantiate()
    {
        // Called after first prefab creation. Unique windows don't typically get destroyed after that
        // So handle any update from the player using events.
        // If you need player data here, it's best to ensure the window is created after player load
        // i.e. the earlier time point you should call UIManager.Instance.OpenUniqueUIWindow is probably player's start
    }
}