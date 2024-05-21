using AO;

public class FightPlayerSkillSlotsManager : FightPlayerComponent
{
    protected SkillSlotsPanel SlotsPanel;
    public Dictionary<string, SkillSlot> ActiveSkillSlots = new(); // A slot becomes active if the player put in an active skill

    public override void Update()
    {
        foreach (var kv in ActiveSkillSlots)
        {
            if (kv.Value.CurrentCooldownTime() > 0)
            {
                kv.Value.ReduceCooldown(Time.DeltaTime);
            }
        }
    }


    public void InitKeybind()
    {
        // Technically we can use reflection to derive type here but that might cause cross-platform issues... So hard code for now
        // Slots are finite anyways.
        var punch = new PunchSkillSlot();
        punch.InitSlot("Punch", this);
        ActiveSkillSlots["Punch"] = punch;
        AssignKeybindToSlot(FightClubGameManager.PunchKeybind, "Punch");
        
        // TODO
        var slot1 = new EquipSkillSlot();
        slot1.InitSlot("Slot1", this);
        ActiveSkillSlots["Slot1"] = slot1;
        AssignKeybindToSlot(FightClubGameManager.Slot1Keybind, "Slot1");
    }
    
    

    public void AssignKeybindToSlot(Keybind slotKeybind, string mainKey)
    {
        SkillSlot targetSlot;
        ActiveSkillSlots.TryGetValue(mainKey, out targetSlot);
        if (targetSlot == null)
        {
            Log.Error($"Skill Slot named {mainKey} NOT FOUND");
            return;
        }

        targetSlot.SlotKeyBind = slotKeybind;
    }

    public void UpdateSlot(string mainKey, int level, string skillKey = null)
    {
        ActiveSkillSlots[mainKey].AssignSkill(skillKey ?? mainKey, level);
    }

    public void DeactivateSlot(string mainKey)
    {
        ActiveSkillSlots[mainKey].AssignSkill("Empty", 0);
    }

    public SkillSlot GetSkillSlots(string mainKey)
    {
        return ActiveSkillSlots[mainKey];
    }

    public FightPlayer GetPlayer()
    {
        return _player;
    }
}