using AO;
using Assembly.scripts.UI.Loadout;

namespace Assembly.scripts.UI.SkillTree;

public class AbilityInfoScreen : Component
{
    [Serialized] public UIButton InfoQuitBtn;

    [Serialized] private UIText _title;

    [Serialized] private UIImage _background;
    [Serialized] private UIText _description;
    [Serialized] private UIText _baseDmg;
    [Serialized] private UIText _cooldown;
    [Serialized] private UIText _range;

    [Serialized] private Entity _damageText;
    [Serialized] private Entity _nondamageText;

    [Serialized] private AbilityPreview _preview;

    public void SetDescription(string skillKey)
    {
        
        if (!SkillConfig.STConfigQueryDict.TryGetValue(skillKey, out var stConfig))
        {
            Log.Error($"{skillKey} is not valid! Did you forgot to input it into the stConfig dict?");
            Entity.LocalEnabled = false;
            return;
        }

        _title.Text = stConfig.GetDisplayName();
        if (!SkillConfig.STTabQueryDict.TryGetValue(stConfig.NTab, out var tabConfig))
        {
            Log.Error($"{tabConfig} is not valid! Did you forgot to add a new tab?");
            Entity.LocalEnabled = false;
            return;
        }

        _background.Sprite = Assets.GetAsset<Texture>(tabConfig.InfoScreenBg);

        _description.Text = stConfig.DescriptionTextKey == "%OVERRIDE%" ? SkillConfig.GetOverrideDescription(stConfig.SkillKey, FightClubUtils.GetLocalFightPlayer()) : stConfig.DescriptionTextKey;
        _cooldown.Text = stConfig.CooldownKey == "%OVERRIDE%" ? SkillConfig.GetOverrideCooldown(stConfig.SkillKey, FightClubUtils.GetLocalFightPlayer()) : stConfig.CooldownKey;
        _range.Text = stConfig.RangeDescriptionKey == "%OVERRIDE%" ? SkillConfig.GetOverrideRange(stConfig.SkillKey, FightClubUtils.GetLocalFightPlayer()) : stConfig.RangeDescriptionKey;;
        _preview.SetPreview(stConfig.AbilityPreviewPath);
        
        if (stConfig.BaseDamageKey == 0)
        {
            SetNonDamageSkill(true);
        }
        else
        {
            SetNonDamageSkill(false);
            int dmg = (stConfig.BaseDamageKey == SkillConfig._overrideValue_
                ? SkillConfig.GetOverrideBaseDamage(stConfig.SkillKey, FightClubUtils.GetLocalFightPlayer())
                : stConfig.BaseDamageKey) + FightClubUtils.GetLocalFightPlayer().CurrentAttack;
            _baseDmg.Text = dmg.ToString();
        }
    }

    public void OverrideDescription(string txt)
    {
        _description.Text = txt;
    }

    private void SetNonDamageSkill(bool enable)
    {
        _damageText.LocalEnabled = !enable;
        _nondamageText.LocalEnabled = enable;
    }

}