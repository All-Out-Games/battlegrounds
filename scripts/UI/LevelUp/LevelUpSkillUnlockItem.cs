using AO;

namespace Assembly.scripts.UI.LevelUp;

public class LevelUpSkillUnlockItem : Component
{
    [Serialized] public UIImage SkillIcon;
    [Serialized] public UIImage SkillTreeIcon;
    [Serialized] public UIText UnlockText;
    [Serialized] public UIText CoinsText;

    public void SetSkillUnlock(string skillKey)
    {
        var cfg = SkillConfig.GetConfig(skillKey);
        SkillIcon.Sprite = Assets.GetAsset<Texture>(cfg.IconPath);
        SkillTreeIcon.Sprite =
            Assets.GetAsset<Texture>(SkillConfig.STTabQueryDict[cfg.NTab].TreeIcon);
        UnlockText.Text = $"You can now learn {cfg.GetDisplayName()}!";
        CoinsText.Text = cfg.UpgradeCost.ToString();
    }
}