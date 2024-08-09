using AO;

namespace Assembly.scripts.UI.LevelUp;

/// <summary>
/// This is attached to a window that resides in ResourceOverlayWindow.prefab <see cref="ResourceOverlayWindow"/>
/// </summary>
public class LevelUpWindow : Component
{
    [Serialized] public UIText LevelText;
    [Serialized] public UIText CoinText;
    [Serialized] public UIText GemText;
    [Serialized] public LevelUpSkillUnlockItem Item1;
    [Serialized] public LevelUpSkillUnlockItem Item2;

    public void PopAtLevelUp(int lvl)
    {
        LevelText.Text = $"{lvl + 1}";
        
        CoinText.Text = $"{LevelingData.CoinRewards[lvl]} Coins Given!";
        
        if (LevelingData.GemRewards[lvl] != 0)
        {
            GemText.Entity.Parent.LocalEnabled = true;
            GemText.Text = $"{LevelingData.GemRewards[lvl]} Gems Awarded!";
        }
        else
        {
            GemText.Entity.Parent.LocalEnabled = false;
        }

        List<string> unlockedSkills = new List<string>();
        LevelUpSkillUnlockItem[] unlockItem = { Item1, Item2 };
        
        // Loop over skill configs to find skills that unlocks at this level
        foreach (string key in SkillConfig.GetAllSkillKeys())
        {
            if (SkillConfig.GetConfig(key).UnlockLevel == lvl)
            {
                unlockedSkills.Add(key);
            }
        }
        // Enable maximum two of skill unlock items.
        for (int i = 0; i < unlockItem.Length; i++)
        {
            var item = unlockItem[i];
            if (i < unlockedSkills.Count )
            {
                item.SetSkillUnlock(unlockedSkills[i]);
                item.Entity.LocalEnabled = true;
            }
            else
            {
                item.Entity.LocalEnabled = false;
            }
        }
    }
}