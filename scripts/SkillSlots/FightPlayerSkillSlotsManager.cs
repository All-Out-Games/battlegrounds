using AO;

public partial class FightPlayerSkillSlotsManager : FightPlayerComponent
{
    protected SkillSlotsPanel SlotsPanel;
    public Dictionary<string, SkillSlot> ActiveSkillSlots = new(); // A slot becomes active if the player put in an active skill

    public bool AllSilent;
    public override void Update()
    {
        if (_player.IsLocal)
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
        
    }

    public override void Awake()
    {
        base.Awake();
        if (_player.IsLocal)
        {
            SlotsPanel = UIWindow.InstantiateWindow(UniqueWindowKeys.SkillSlotsPanelPath) as SkillSlotsPanel; 
        }
    }

    public override void Start()
    {
        if (_player.IsLocal)
        {
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
        if (_player.IsLocal)
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
        AllSilent = silent;
        /*foreach (var kv in ActiveSkillSlots)
        {
            kv.Value.SilentSlot(silent); // This is stupid
        }*/
    }

    public SkillSlot GetSkillSlot(string mainKey)
    {
        return ActiveSkillSlots[mainKey];
    }

    public FightPlayer GetPlayer()
    {
        return _player;
    }

    public void SkillSlotsPanelEnable(bool enable)
    {
        if (_player.IsLocal)
        {
            TestServerRPC.LogSomethingOnServer($"Set slot panel status to {enable}, compID = {Id}");
            SlotsPanel.Entity.LocalEnabled = enable;
        }
    }
}