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
    private List<SkillSlotButton> _skillSlotButtons;

    [Serialized] private SkillSlotPunchButton _punchButton;

    public override void Awake()
    {
        _skillSlotButtons = new List<SkillSlotButton>()
        {
            _sl1,
            _sl2,
            _sl3,
            _sl4
        };
        base.Awake();
    }
    
}