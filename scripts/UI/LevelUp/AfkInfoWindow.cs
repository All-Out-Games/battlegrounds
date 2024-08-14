using AO;

namespace Assembly.scripts.UI.LevelUp;

/// <summary>
/// A window that shows the AFK bonus the local players are eligible to receive.
/// </summary>
public class AfkInfoWindow : UniqueUIWindow
{
    [Serialized] public UIText AfkXpNumber;
    [Serialized] public UIText LevelBonusNumber;

    public void CalculateAfkInfo()
    {
        
    }
}