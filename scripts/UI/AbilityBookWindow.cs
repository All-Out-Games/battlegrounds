using AO;
using Assembly.scripts.SkillSlots.Abilities;

namespace Assembly.scripts.UI;

public class AbilityBookWindow : UniqueUIWindow
{
    [Serialized] private UIButton SaveButton;
    [Serialized] private UIButton NextButton;
    [Serialized] private UIButton PrevButton;
    
    [Serialized] private UIImage Icon0; // Always punch. Cannot be changed
    [Serialized] private UIImage Icon1;
    [Serialized] private UIImage Icon2;
    [Serialized] private UIImage Icon3;
    [Serialized] private UIImage Icon4;
    [Serialized] private UIImage Icon5;

    [Serialized] private UIText AbilityText0;
    [Serialized] private UIText AbilityText1;
    [Serialized] private UIText AbilityText2;
    [Serialized] private UIText AbilityText3;
    [Serialized] private UIText AbilityText4;
    [Serialized] private UIText AbilityText5;

    private UIDirectionalLayout _skillList;
    private Dictionary<string, AbilityBookItem> _bookItems; // [skillkey : AbilityBookItem]
    private string AbilityBookItemPrefabPath = "AbilityBookItem.prefab";

    private FightPlayerSkillSlotsManager _slotsMgr;
    protected SkillConfig.SkillTreeTabs CurrentTab = SkillConfig.SkillTreeTabs.Basic;
    protected int CurrentTabIndex = 0;
    protected int TabAmount = 0;

    public override void OnInstantiate()
    {
        base.OnInstantiate();
        _slotsMgr ??= Network.LocalPlayer.Entity.GetComponent<FightPlayer>().GetSkillSlots();
    }

    public override void Start()
    {
        base.Start();
        List<FightAbility> faList = _slotsMgr.GetCurrentAbilities();
        
        // Fill Skill Slots on the UI with current abilities
        FightAbility punch = faList[0];
        Icon0.Sprite = punch.Icon;
        AbilityText0.Text = punch.SkillKey;
        FightAbility ab1 = faList[1];
        Icon1.Sprite = ab1.Icon;
        AbilityText1.Text = ab1.SkillKey;
        FightAbility ab2 = faList[2];
        Icon2.Sprite = ab2.Icon;
        AbilityText2.Text = ab2.SkillKey;
        FightAbility ab3 = faList[3];
        Icon3.Sprite = ab3.Icon;
        AbilityText3.Text = ab3.SkillKey;
        FightAbility ab4 = faList[4];
        Icon4.Sprite = ab4.Icon;
        AbilityText4.Text = ab4.SkillKey;
        FightAbility ab5 = faList[5];
        Icon5.Sprite = ab5.Icon;
        AbilityText5.Text = ab5.SkillKey;

        // Instantiate all ability book items

    }

    private void PreviousTab()
    {
        CurrentTabIndex -= 1;
        if (CurrentTabIndex == -1)
        {
            CurrentTabIndex = TabAmount - 1;
        }
        UpdateSkillTreeTab(SkillConfig.STConfigTabsList[CurrentTabIndex]);
    }

    private void NextTab()
    {
        CurrentTabIndex += 1;
        if (CurrentTabIndex >= TabAmount)
        {
            CurrentTabIndex = 0;
        }
        UpdateSkillTreeTab(SkillConfig.STConfigTabsList[CurrentTabIndex]);
    }

    private void UpdateSkillTreeTab(SkillConfig.SkillTreeTabs tab)
    {
        // All ability nodes are loaded upon startup. We just need to enable those belong to this tab
    }
}