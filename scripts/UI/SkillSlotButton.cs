using AO;

/// <summary>
/// UI for a single skill slot button.
/// <see cref="SkillSlotsPanel"/>
/// </summary>
public class SkillSlotButton : Component
{
    [Serialized] protected UIButton SkillButton; // Bind skill cast function & icon on this button 
    [Serialized] protected UIText SkillKeyText;
    [Serialized] protected UIText SkillSlotShortcut; // Unused. We don't display shortcuts on buttons as keycode supports are not here yet.
    [Serialized] protected UIText CooldownText; 

    protected Action CurrentOnClick;

    protected void CallCurrentOnClick()
    {
        CurrentOnClick?.Invoke();
    }

    public override void Start()
    {
        SkillButton.OnClicked += CallCurrentOnClick;
    }

    public override void OnDestroy()
    {
        SkillButton.OnClicked -= CallCurrentOnClick;
    }

    public void SetSlotShortcutText(string txt)
    {
        SkillSlotShortcut.Text = txt;
    }

    public void SetSlotSkillKeyText(string txt)
    {
        SkillKeyText.Text = txt;
    }

    public void BindSkillButtonCallback(Action onClick)
    {
        CurrentOnClick = onClick;
    }

    public void SetCoolDownText(string txt)
    {
        CooldownText.Text = txt;
    }
}