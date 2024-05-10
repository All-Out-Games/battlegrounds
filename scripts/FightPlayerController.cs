using AO;

/// <summary>
/// Controller of the arena player
/// </summary>
public partial class FightPlayer
{
    public void ControllerUpdate()
    {
        if (IsLocal)
        {
            HandlePunchInput();
        }
    }
    
    
    #region Input Handling

    // All input handling functions must be wrapped within player.IsLocal condition!
    protected void HandlePunchInput()
    {
        // TODO: Unify input handler to be skill slot based.
        if (GetKeybindDown(FightClubGameManager.PunchKeybind))
        {
            EffectManager.CallServer_CastPunch(1);
        }
    }

    #endregion
}