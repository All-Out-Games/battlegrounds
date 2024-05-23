using AO;

public abstract class UIWindow : Component
{
    public Action<UIWindow, Player> OnWindowOpen;
    public Action<UIWindow, Player> OnWindowClose;
    public bool IsActive = false;

    public abstract void CloseWindow();
    public abstract void OpenWindow();

    public static UIWindow InstantiateWindow(string prefabPath)
    {
        Log.Info($"UI Window {prefabPath} instantiated!");
        var windowPrefab = Assets.GetAsset<Prefab>(prefabPath);
        UIWindow newWindow = windowPrefab.Instantiate().GetComponent<UIWindow>();
        return newWindow;
    }
}