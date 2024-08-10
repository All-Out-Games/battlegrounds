using AO;
using Assembly.scripts;
using Assembly.scripts.UI;

/// <summary>
/// This component manages world space player UI.
/// 
/// </summary>
public partial class FightPlayer
{
    private ResourceOverlayWindow _overlay;
    protected void InitializeUI()
    {
        // TODO: Fetch levels and fill overhead level
        
        if (IsLocal)
        {
            _overlay =
                UIManager.Instance.OpenOverlayWindow(UniqueWindowKeys.ResourcesOverlayWindowPath) as ResourceOverlayWindow;

            if (_overlay == null)
            {
                Log.Error("Overlay Window is Null! Did you change the prefab path?");
                return;
            }
            // NOTE: Action is value type. You have to pass them as ref.
            _overlay.HookupEvents(ref CoinUpdateEvent);
            
            _level.OnSync += NotifyLevelUpdate;
            _exp.OnSync += NotifyExpUpdate;
            _gem.OnSync += NotifyGemUpdate;
        }
    }
    
    /// <summary>
    /// Called when _ex[ SyncVar is updated.
    /// Should be UI only. Handle level up / rebirth in SyncVar setters
    /// </summary>
    /// <param name="old"></param>
    /// <param name="exp"></param>
    private void NotifyExpUpdate(int old, int exp)
    {
        if (exp - old > 1000)
        {
            Log.Warn($"Player {Name} got an abnormal amount of EXP!");
        }
        if (IsLocal)
        {
            _overlay.UpdateCurExpTxt(exp);
        }
    }

    /// <summary>
    /// Called when _level SyncVar is updated.
    /// Should be UI only. 
    /// </summary>
    /// <param name="old"></param>
    /// <param name="lvl"></param>
    private void NotifyLevelUpdate(int old, int lvl)
    {
        if (lvl - old > 1)
        {
            Log.Warn($"Player {Name} got an abnormal amount of LV!");
        }
        if (IsLocal)
        {
            _overlay.UpdateLevelingTxt(lvl, LevelingData.NextLevelXp[lvl]);
        }
    }

    private void NotifyGemUpdate(int old, int gem)
    {
        if (IsLocal)
        {
            _overlay.UpdateGem(gem);
        }
    }
    
    
}