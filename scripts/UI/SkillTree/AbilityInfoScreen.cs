using AO;

namespace Assembly.scripts.UI.SkillTree;

public class AbilityInfoScreen : Component
{
    [Serialized] public UIButton InfoQuitBtn;

    [Serialized] private UIText _title;

    [Serialized] private UIImage _background;
    [Serialized] private UIText _description;
    [Serialized] private UIText _baseDmg, _atk;
    [Serialized] private UIText _cooldown;
    [Serialized] private UIText _range;

    [Serialized] private Entity _damageText;
    [Serialized] private Entity _nondamageText;

    public void SetDescription(string skillKey)
    {
        _title.Text = skillKey;
        if (!SkillConfig.STConfigQueryDict.TryGetValue(skillKey, out var stConfig))
        {
            Log.Error($"{skillKey} is not valid! Did you forgot to input it into the stConfig dict?");
            Entity.LocalEnabled = false;
            return;
        }

        if (!SkillConfig.STTabQueryDict.TryGetValue(stConfig.NTab, out var tabConfig))
        {
            Log.Error($"{tabConfig} is not valid! Did you forgot to add a new tab?");
            Entity.LocalEnabled = false;
            return;
        }

        _background.Sprite = Assets.GetAsset<Texture>(tabConfig.InfoScreenBg);
        _description.Text = stConfig.DescriptionTextKey;
        _cooldown.Text = stConfig.CooldownKey;
        _range.Text = stConfig.RangeDescriptionKey;
        
        
        if (stConfig.BaseDamageKey == 0)
        {
            SetNonDamageSkill(true);
        }
        else
        {
            SetNonDamageSkill(false);
            _baseDmg.Text = stConfig.BaseDamageKey.ToString();
            _atk.Text = FightClubUtils.GetLocalFightPlayer().CurrentAttack.ToString();
        }
    }

    private void SetNonDamageSkill(bool enable)
    {
        _damageText.LocalEnabled = !enable;
        _nondamageText.LocalEnabled = enable;
    }

}