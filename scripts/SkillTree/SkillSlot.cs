using AO;

/// <summary>
/// Skill Slot
/// A skill slot is a unique binding for a key/button on a player
/// An active skill can be placed in it.
/// This is a *client only* class. The skill info here are just used for the UI and for triggering server commands.
/// </summary>
public abstract class SkillSlot
{
    public Keybind SlotKeyBind;
    
    /// <summary>
    /// The first active skill's skillKey. This cannot be changed once defined.
    /// Used in the FightPlayerSkillSlots Component to find a skill slot.
    /// </summary>
    protected string MainSkillKey;
    
    /// <summary>
    /// The current skillKey for the slot. It can be changed when the player replace the skill in this slot.
    /// </summary>
    protected string CurrentSkillKey;

    public int SkillLevel;

    protected float CurrentCooldown;
    protected float CurrentMaxCooldown;
    protected FightPlayerSkillSlots SlotsMgr;

    public float CurrentCooldownTime()
    {
        return CurrentCooldown;
    }

    public float CurrentCooldown01()
    {
        return float.Clamp(CurrentCooldown / CurrentMaxCooldown, 0, 1);
    }

    public void InitSlot(string mainKey, FightPlayerSkillSlots mgr)
    {
        MainSkillKey = mainKey;
        SlotsMgr = mgr;
    }

    public void AssignSkill(string skillKey, int level)
    {
        Log.Debug($"Skill Slot {MainSkillKey} is assigned {skillKey} lv. {level}");
        CurrentSkillKey = skillKey;
        SkillLevel = level;
    }

    public void ApplyCooldown(float maxCooldown)
    {
        CurrentCooldown = maxCooldown;
        CurrentMaxCooldown = maxCooldown;
    }
    
    
    public abstract void UseSkill();
}