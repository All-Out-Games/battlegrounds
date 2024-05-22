using AO;

public partial class FightPlayerSkillSlotsManager : FightPlayerComponent
{
    protected SkillSlotsPanel SlotsPanel;
    public Dictionary<string, SkillSlot> ActiveSkillSlots = new(); // A slot becomes active if the player put in an active skill

    public override void Update()
    {
        foreach (var kv in ActiveSkillSlots)
        {
            if (kv.Value.CurrentCooldownTime() > 0)
            {
                float cooldown = kv.Value.ReduceCooldown(Time.DeltaTime);
                SlotsPanel.SetSlotCooldown(kv.Key, cooldown);
            }
        }
    }

    public override void Start()
    {
        if (Network.IsClient)
        {
            SlotsPanel = UIWindow.InstantiateWindow(UniqueWindowKeys.SkillSlotsPanelPath) as SkillSlotsPanel; 
            SlotsPanel.Entity.SetParent(UIManager.Instance.FindCanvas().Entity, false);
            SkillSlotsPanelEnable(false);
        }
        
    }
    
    


    public void InitKeybind()
    {
        // Slots are finite and won't change at runtime.
        var punch = new PunchSkillSlot();
        punch.InitSlot("Punch", this);
        ActiveSkillSlots["Punch"] = punch;
        AssignKeybindToSlot(FightClubGameManager.PunchKeybind, "Punch");
        
        var slot1 = new EquipSkillSlot();
        slot1.InitSlot("Slot1", this);
        ActiveSkillSlots["Slot1"] = slot1;
        AssignKeybindToSlot(FightClubGameManager.Slot1Keybind, "Slot1");

        var slot2 = new EquipSkillSlot();
        slot2.InitSlot("Slot2", this);
        ActiveSkillSlots["Slot2"] = slot2;
        AssignKeybindToSlot(FightClubGameManager.Slot2Keybind, "Slot2");

        var slot3 = new EquipSkillSlot();
        slot3.InitSlot("Slot3", this);
        ActiveSkillSlots["Slot3"] = slot3;
        AssignKeybindToSlot(FightClubGameManager.Slot3Keybind, "Slot3");

        var slot4 = new EquipSkillSlot();
        slot4.InitSlot("Slot4", this);
        ActiveSkillSlots["Slot4"] = slot4;
        AssignKeybindToSlot(FightClubGameManager.Slot4Keybind, "Slot4");
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
        if (Network.IsClient)
        {
            SlotsPanel.SetSlotText(mainKey, skillKey ?? mainKey);
        }
    }

    public void RemoveSlot(string mainKey)
    {
        ActiveSkillSlots[mainKey].AssignSkill("Empty", 0);
    }

    [ClientRpc]
    public void SilentAllSlot(bool silent)
    {
        foreach (var kv in ActiveSkillSlots)
        {
            kv.Value.SilentSlot(silent);
        }
    }

    public SkillSlot GetSkillSlots(string mainKey)
    {
        return ActiveSkillSlots[mainKey];
    }

    public FightPlayer GetPlayer()
    {
        return _player;
    }

    public void SkillSlotsPanelEnable(bool enable)
    {
        if (Network.IsClient)
        {
            TestServerRPC.LogSomethingOnServer($"Set slot panel status to {enable}");
            SlotsPanel.Entity.LocalEnabled = enable;
        }
    }
}