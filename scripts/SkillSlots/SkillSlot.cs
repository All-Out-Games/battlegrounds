using AO;

/// <summary>
/// Skill Slot
/// A skill slot is a unique binding for a key/button on a player
/// An active skill can be placed in it.
/// This is a *client only* class. The skill info here are just used for the UI and for triggering server commands.
/// </summary>
public abstract class SkillSlot
{
    protected static Type EffectManagerType = Type.GetType("FightPlayerEffectManager");
    
    public Keybind SlotKeyBind;
    
    /// <summary>
    /// The first active skill's skillKey. This cannot be changed once defined.
    /// Used in the FightPlayerSkillSlots Component to find a skill slot.
    /// </summary>
    protected string MainSkillKey;
    
    /// <summary>
    /// The current skillKey for the slot. It can be changed when the player replace the skill in this slot.
    /// </summary>
    protected string CurrentSkillKey = "Empty";

    public int SkillLevel = 0;
    public bool IsSilent = false;
    // TODO: Cooldown should be syncvars
    protected float CurrentCooldown;
    protected float CurrentMaxCooldown;
    protected FightPlayerSkillSlotsManager SlotsMgr;

    public float CurrentCooldownTime()
    {
        return CurrentCooldown;
    }

    public float CurrentCooldown01()
    {
        return float.Clamp(CurrentCooldown / CurrentMaxCooldown, 0, 1);
    }

    public void InitSlot(string mainKey, FightPlayerSkillSlotsManager mgr)
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

    public string GetCurrentSkillKey()
    {
        return CurrentSkillKey;
    }

    public string GetMainKey()
    {
        return MainSkillKey;
    }
    
    public void ApplyCooldown(float maxCooldown)
    {
        // The parameter should be included in the skill's effect config
        CurrentCooldown = maxCooldown;
        CurrentMaxCooldown = maxCooldown;
    }

    public float ReduceCooldown(float delta)
    {
        CurrentCooldown -= delta;
        CurrentCooldown = CurrentCooldown < 0 ? 0 : CurrentCooldown;
        return CurrentCooldown;
    }

    public void SilentSlot(bool silent)
    {
        IsSilent = silent;
    }

    public bool IsEmpty()
    {
        return CurrentSkillKey == "Empty";
    }
    public bool IsUsable()
    {
        return !IsEmpty() && !IsSilent && CurrentCooldown <= 0;
    }
    
    public abstract void UseSkill();
}