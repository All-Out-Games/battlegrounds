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
        foreach (var kv in SkillSlots.ActiveSkillSlots)
        {
            // TODO: More complex input handling. Wrap handler functions in slots
            if (GetKeybindDown(kv.Value.SlotKeyBind))
            {
                TestServerRPC.CallServer_LogSomethingOnServer("Skill Cast Input!");
                
                kv.Value.UseSkill();
            }
        }
    }
    #endregion
}