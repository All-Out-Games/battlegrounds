using AO;

namespace Assembly.scripts.Zones;

public class UniqueWindowInteractable : Component
{
    [Serialized] protected string WindowPrefabPath;
    // Required Component: A Trigger Box and Interactable
    protected Interactable WindowOpenInteractable;
    protected Circle_Collider ExitVendorRange;

    public override void Start()
    {
        base.Start();
        WindowOpenInteractable = Entity.GetComponent<Interactable>();
        ExitVendorRange = Entity.GetComponent<Circle_Collider>();
        if (WindowOpenInteractable == null || ExitVendorRange == null)
        {
            Log.Error("Vendor Interactable Must Have a Circle Collider and Interactable Component!");
            Entity.Destroy();
            return;
        }

        WindowOpenInteractable.OnInteract += OpenWindow;
        WindowOpenInteractable.Radius = 3;
        
        ExitVendorRange.OnCollisionExit += LeaveVendor;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        WindowOpenInteractable.OnInteract -= OpenWindow;
        ExitVendorRange.OnCollisionExit -= LeaveVendor;
    }

    protected void OpenWindow(Player p)
    {
        if (p.IsLocal)
        {
            Log.Debug("1");
            UIManager.Instance.OpenUniqueUIWindow(WindowPrefabPath);
        }
    }

    protected void LeaveVendor(Entity pEntity)
    {
        FightPlayer fightPlayer = pEntity.GetComponent<FightPlayer>();
        if (fightPlayer is { IsLocal: true })
        {
            UIManager.Instance.CloseAllUniqueWindow();
        }
    }
    
}