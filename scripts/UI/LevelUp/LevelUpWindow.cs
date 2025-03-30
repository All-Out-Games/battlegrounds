using System.Collections;
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
    [Serialized] private UISpineSkeleton _sparkles;

    private UIRect _rect;
    private Coroutine _popCoroutine;

    public override void Awake()
    {
        base.Awake();
        _rect ??= GetComponent<UIRect>();
    }

    public void PopAtLevelUp(int lvl)
    {
        if (lvl <= 0 || lvl > LevelingData.MaxLevel)
        {
            return;
        }

        _rect ??= GetComponent<UIRect>();
        LevelText.Text = $"{lvl + 1}";
        
        CoinText.Text = $"{LevelingData.CoinRewards[lvl]} Coins Given!";
        
        if (LevelingData.GemRewards[lvl] != 0)
        {
            GemText.Entity.Parent.LocalEnabled = true;
            GemText.Text = $"{LevelingData.GemRewards[lvl]} Glory Awarded!";
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

        _popCoroutine = Coroutine.Start(Entity, PopOnOutThenRetract());
    }
    
    public void PopChampion()
    {
        _rect ??= GetComponent<UIRect>();
        LevelText.Text = "C";
        CoinText.Text = $"10000 Coins Given!";
        GemText.Entity.Parent.LocalEnabled = true;
        GemText.Text = $"10000 Glory Awarded!";

        List<string> unlockedSkills = new List<string>();
        LevelUpSkillUnlockItem[] unlockItem = { Item1, Item2 };
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

        _popCoroutine = Coroutine.Start(Entity, PopOnOutThenRetract());
        
        
    }

    private IEnumerator PopOnOutThenRetract()
    {
        float progress01 = 0;
        Entity.LocalEnabled = true;
        _sparkles.Instance.SetSkin("default");
        _sparkles.Instance.EnableSkin("default");
        _sparkles.Instance.RefreshSkins();
        _sparkles.Instance.SetAnimation("hatch", false); // Sparkles
        while (progress01 < 1)
        {
            _rect.Offset = _rect.Offset with { Y = -500 + 500 * progress01 };
            progress01 += 4 * Time.DeltaTime; // Pop out in 0.25 a sec
            yield return null;
        }

        yield return new WaitForSeconds(LevelingData.LevelingWindowStayTime);
        progress01 = 0;
        while (progress01 < 1)
        {
            _rect.Offset = _rect.Offset with { Y = -500 * progress01 };
            progress01 += 2 * Time.DeltaTime; // Retract in .5 sec
            yield return null;
        }

        Entity.LocalEnabled = false;
        
        yield return null;
    }

    private IEnumerator SparkleOnlyPopup()
    {
        Entity.LocalEnabled = true;
        _rect.Offset = _rect.Offset with { Y = -2500};
        
        _sparkles.Instance.SetSkin("default");
        _sparkles.Instance.EnableSkin("default");
        _sparkles.Instance.RefreshSkins();
        _sparkles.Instance.SetAnimation("hatch", false);

        yield return new WaitForSeconds(3);
        Entity.LocalEnabled = false;
        
        yield return null;
    }

    public void PlaySparkles()
    {
        if ((_popCoroutine.Alive() && _popCoroutine.Finished) || !_popCoroutine.Alive())
        {
            // Don't interfere
            Coroutine.Start(Entity, SparkleOnlyPopup());
        }
        
    }
}