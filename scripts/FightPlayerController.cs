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

    protected void UseSkillSlot(SkillSlot slot)
    {
        if (slot.IsUsable())
        {
            slot.UseSkill();
        }
        else if (slot.IsEmpty())
        {
            UIManager.Instance.SetPopup($"Slot {slot.GetMainKey()}, Is EMPTY!", 2.0f, this);
        }
        else
        {
            UIManager.Instance.SetPopup($"Slot {slot.GetMainKey()}, Key {slot.GetCurrentSkillKey()} Is NOT usable", 2.0f, this);
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