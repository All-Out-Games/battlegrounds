using AO;
/// <summary>
/// Main UI Class for the skill slots panel. Class is managed in FightPlayerSkillSlotsManager
/// <see cref="FightPlayerSkillSlotsManager"/>
/// </summary>
public class SkillSlotsPanel : BaseUniqueWindow
{
    [Serialized] private SkillSlotButton _sl1;
    [Serialized] private SkillSlotButton _sl2;
    [Serialized] private SkillSlotButton _sl3;
    [Serialized] private SkillSlotButton _sl4;
    private Dictionary<string, SkillSlotButton> _skillSlotButtons;

    [Serialized] private SkillSlotPunchButton _punchButton;

    public override void Awake()
    {
        _skillSlotButtons = new Dictionary<string, SkillSlotButton>()
        {
            { "Punch", _punchButton },
            { "Slot1", _sl1 },
            { "Slot2", _sl2 },
            { "Slot3", _sl3 },
            { "Slot4", _sl4 }
        };
        base.Awake();
    }

    public void SetSlotCooldown(string key, float num)
    {
        _skillSlotButtons[key].SetCoolDownText($"{num}");
    }

    public void SetSlotText(string key, string txt)
    {
        _skillSlotButtons[key].SetSlotSkillKeyText(txt);
    }
}