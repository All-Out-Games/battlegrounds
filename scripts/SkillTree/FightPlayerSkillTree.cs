
using AO;

public partial class FightPlayerSkillTree : FightPlayerComponent
{
    public Dictionary<string, bool> SkillUnlockDict; // [SkillKey : Unlocked], note that only active skills can be "unlocked"
    public Dictionary<string, int> SkillLevelDict; // [SkillKey: Level], level of all skills, 0 means not upgraded yet.



    #region EventFunctions

    public override void Awake()
    {
        foreach (var skill in SkillConfig.ActiveSkills)
        {
            SkillUnlockDict[skill] = false; // Add entries to all active skills
        }

        foreach (var skill in SkillConfig.GetAllSkillKeys())
        {
            SkillLevelDict[skill] = 0;
            // SkillLevelDict["Punch"] = 1; // Unlock punch by default
        }
    }
    

    #endregion

    #region Skill Handling

    public void UnlockSkill(string skillKey,bool unlock)
    {
        bool skillUnlocked = false;
        if (SkillUnlockDict.TryGetValue(skillKey, out skillUnlocked))
        {
            SkillUnlockDict[skillKey] = true;
        }
        else
        {
            Log.Error($"SkillTree: Skill {skillKey} NOT FOUND");
        }
    }

    public void UpgradeSkill(string skillKey, int maxLevel)
    {
        int currentLevel;
        if (SkillLevelDict.TryGetValue(skillKey, out currentLevel))
        {
            if (currentLevel < maxLevel)
            {
                SkillLevelDict[skillKey] = currentLevel + 1;
            }
            else
            {
                Log.Error($"Skill {skillKey} reached max level!");
            }
        }
        else
        {
            Log.Error($"SkillTree: Skill {skillKey} NOT FOUND");
        }
    }

    #endregion

    #region Player Workflow

    public bool IsSkillUnlocked(string skillKey)
    {
        bool skillUnlocked;
        if (SkillUnlockDict.TryGetValue(skillKey, out skillUnlocked))
        {
            return skillUnlocked;
        }
        else
        {
            Log.Error($"SkillTree: Skill {skillKey} NOT FOUND");
            return false;
        }
    }
    
    /// <summary>
    /// Called in player's start function.
    /// Get the Save of player and populate skill trees 
    /// </summary>
    /// <param name="player"></param>
    public void InitializeSkillTreeComp()
    {
        // TODO
    }

    /// <summary>
    /// 
    /// </summary>
    public void HandleAllSkills()
    {
        // TODO
        
        // Phase 1: Attr Boosts (and passives, which are essentially permanent effects)
        
        
        // Phase 2: Active Skill Unlocks
        
        
        // Phase 3: Active Skill Replacements
        
        
        // Phase 4: Active Skill Enhancements
    }

    #endregion
}