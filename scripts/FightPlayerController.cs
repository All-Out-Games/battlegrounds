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
            HandleSkillSlotInput();
        }
    }
    
    
    #region Input Handling

    // All input handling functions must be wrapped within player.IsLocal condition!

    protected void HandleSkillSlotInput()
    {
        //TestServerRPC.CallServer_LogSomethingOnServer(SkillSlots.ActiveSkillSlots["Punch"].SlotKeyBind.ToString());
    }
    #endregion
}