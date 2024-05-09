using AO;

public partial class FightPlayerSkillTree : FightPlayerComponent
{
    public Dictionary<string, bool> SkillUnlockDict; // [SkillKey : Unlocked], note that only active skills can be "unlocked"
    public Dictionary<string, int> SkillLevelDict; // [SkillKey: Level], level of all skills, 0 means not upgraded yet.

    #region EventFunctions

    public override void Awake()
    {
        // Initialize all entries before syncing happens
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
            SkillUnlockDict[skillKey] = unlock; // Unlock is usually always true

            if (SkillLevelDict[skillKey] != 0)
            {
                Log.Warn($"{skillKey} level is not 0 when being unlocked...");
            }
            
            SkillLevelDict[skillKey] = 1;
            Save.SetInt(_player, skillKey, 1);
        }
        else
        {
            Log.Error($"SkillTree: Skill {skillKey} NOT FOUND");
        }
    }

    [ServerRpc]
    public bool UpgradeSkill(string skillKey, int maxLevel)
    {
        if (Network.IsServer)
        {
            int currentLevel;
            if (SkillLevelDict.TryGetValue(skillKey, out currentLevel))
            {
                if (currentLevel < maxLevel)
                {
                    SkillLevelDict[skillKey] = currentLevel + 1;
                    RemoveSkill(skillKey);
                    AddSkill(skillKey, SkillLevelDict[skillKey]);
                    Save.SetInt(_player, skillKey, SkillLevelDict[skillKey]);
                    CallClient_SyncSkill(skillKey, SkillLevelDict[skillKey]);
                    return true;
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
        return false;
    }

    #endregion

    #region Player Workflow

    public bool IsActiveUnlocked(string skillKey)
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

    public int GetSkillLevel(string skillKey)
    {
        int skillLvl;
        if (SkillLevelDict.TryGetValue(skillKey, out skillLvl))
        {
            return skillLvl;
        }
        else
        {
            Log.Error($"SkillTree: Skill {skillKey} NOT FOUND");
            return -1;
        }
    }
    
    /// <summary>
    /// [Server Only]
    /// Called in player's Awake function. 
    /// Get the Save of player and populate skill trees. [Save API is only available on server]
    /// </summary>
    public void InitializeSkillTreeComp()
    {
        if (Network.IsServer)
        {
            // Populate all level data (On server & client)
            Player p = _player;
            foreach (string skillKey in SkillConfig.GetAllSkillKeys())
            {
                int lvl = Save.GetInt(p, skillKey, 0);
                
                SkillLevelDict[skillKey] = lvl;
                // CallClient_SyncSkill(skillKey, lvl); // DO NOT Sync skills here on the server. The initial sync happens in HandleAllSkills()
            }
        }
    }

    /// <summary>
    /// [Client Only]
    /// After Server initialized ST from Save, sync the skill levels to the player
    /// Will be called each time when the server upgrade a skill as well.
    /// </summary>
    [ClientRpc]
    public void SyncSkill(string skillKey, int level)
    {
        if (Network.IsClient)
        {
            Log.Debug($"Skill Level Get from Server. {skillKey} = {level}");
            if (SkillLevelDict[skillKey] != 0)
            {
                RemoveSkill(skillKey);
            }
            SkillLevelDict[skillKey] = level;
            AddSkill(skillKey, level);
        }
    }
    
    /// <summary>
    /// [Server Only] Execute the handler of the skill
    /// </summary>
    /// <param name="skillKey"> Key (ID) of the skill </param>
    /// <param name="level">The active level of the skill we are handling</param>
    public void HandleSkill(string skillKey, int level)
    {
        Log.Debug($"Skill Level Handled on Server. {skillKey} = {level}");
        if (SkillLevelDict[skillKey] != 0)
        {
            RemoveSkill(skillKey);
        }
        SkillLevelDict[skillKey] = level;
        AddSkill(skillKey, level);
    }

    /// <summary>
    /// [Server Only]
    /// Called in player start. Activate effects in 4 passes for the player.
    /// </summary>
    public void HandleAllSkills()
    {
        // The order here must be enforced due to skill enhancements must have their main skill (e.g. FireFist -> Punch)
        // unlocked to work.
        if (Network.IsServer)
        {
            Action<string> handleSkillWithSync = (key) =>
            {
                int lvl = SkillLevelDict[key];
                HandleSkill(key, lvl);
                CallClient_SyncSkill(key, lvl);
            };
            
            // Phase 1: Attr Boosts (and passives, which are essentially permanent effects)
            foreach (string abKey in SkillConfig.AttrBoostSkills)
            {
                handleSkillWithSync(abKey);
            }
        
            // Phase 2: Active Skill Unlocks
        
            foreach (string asKey in SkillConfig.ActiveSkills)
            {
                handleSkillWithSync(asKey);
            }
        
            // Phase 3: Active Skill Replacements

            foreach (string rpKey in SkillConfig.ReplacementSkills)
            {
                handleSkillWithSync(rpKey);
            }
        
            // Phase 4: Active Skill Enhancements
            foreach (var sbKey in SkillConfig.SkillBoostSkills)
            {
                handleSkillWithSync(sbKey);
            }
        }
        
    }



    /// <summary>
    /// [Client & Server]
    /// </summary>
    public void OnSkillUpgrade()
    {
        // TODO: Update Skill Tree UI after server unlock/upgrade
    }

    #endregion
}