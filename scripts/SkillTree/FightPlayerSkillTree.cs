using AO;

public partial class FightPlayerSkillTree : FightPlayerComponent
{
    public Dictionary<string, int> SkillLevelDict = new(); // [SkillKey: Level], level of all skills, 0 means not upgraded yet.

    public SyncVar<bool> Initialized = new SyncVar<bool>();

    #region EventFunctions

    // This is only invoked on client (in SyncSkill RPC), which ensures the player update UI strictly after the server finishes upgrade and sync to player.
    public Action<string, int> SkillUpgradeUIEvent; 
    
    // The gameplay logic update for upgrading a skill is handled in SkillHandler.cs
    
    public override void Awake()
    {
        base.Awake();
        foreach (var skill in SkillConfig.GetAllSkillKeys())
        {
            SkillLevelDict[skill] = 0;
        }

        SkillUpgradeUIEvent += OnSkillUpgrade;
    }
    

    public override void OnDestroy()
    {
        
        SkillUpgradeUIEvent -= OnSkillUpgrade;
    }

    #endregion

    #region Skill Handling

    /// <summary>
    /// [Server RPC] request a skill upgrade. Server will be responsible of checking the currency and prerequisite requirements
    /// </summary>
    /// <param name="skillKey">unique skill key, see SkillConfigTables.cs</param>
    [ServerRpc]
    public void RequestUpgradeSkill(string skillKey)
    {
        if (!SkillConfig.STConfigQueryDict.TryGetValue(skillKey, out var cfg)) return;
        // Get parent nodes and check if they are unlocked

        // Lots of checks for the safety of this server RPC.
        // These shouldn't pop up on a non-hacked client as we shall stop clients from requesting this RPC when they don't have the resources.
        if (_player.Level < cfg.UnlockLevel)
        {
            UIManager.CallClient_SetPlayerPopup(_player.Entity.NetworkId,$"You need to be Level {cfg.UnlockLevel + 1} to purchase!", 2f);
            return;
        }
        foreach (string key in cfg.GetParentNodeKeys())
        {
            if (SkillLevelDict[key] == 0)
            {
                Log.Error($"The parent nodes of {skillKey} are not unlocked yet!");
                return;
            }
        }
        int prevLvl = GetSkillLevel(skillKey);
        if (prevLvl == 0 && _player.Coins < cfg.UpgradeCost)
        {
            UIManager.CallClient_SetPlayerPopup(_player.Entity.NetworkId,$"You don't have enough coin! {cfg.UpgradeCost} needed!", 2f);
            return;
        }
        if (prevLvl > 0 && _player.Gem < cfg.UpgradeGemCost[prevLvl - 1])
        {
            UIManager.CallClient_SetPlayerPopup(_player.Entity.NetworkId,$"You don't have enough gem! {cfg.UpgradeGemCost[prevLvl - 1]} needed!", 2f);
            return;
        }
        
        if (UpgradeSkill(skillKey, cfg.MaximumLevel))
        {
            Log.Info($"{skillKey} Upgrade Complete!");
            int lvl = GetSkillLevel(skillKey); // Lvl after upgrade
            if (lvl == 1)
            {
                // Unlock - Use coins
                _player.Coins -= cfg.UpgradeCost;
            }
            else
            {
                // Upgrade - Use gems
                _player.Gem -= cfg.UpgradeGemCost[lvl - 2];
            }
            
        }
        
    }
    
    /// <summary>
    /// [Server Only] Upgrade skill, without checking conditions
    /// </summary>
    /// <param name="skillKey">unique skill key, see SkillConfigTables.cs</param>
    /// <param name="maxLevel"></param>
    /// <returns></returns>
    public bool UpgradeSkill(string skillKey, int maxLevel)
    {
        if (Network.IsServer)
        {
            if (SkillLevelDict.TryGetValue(skillKey, out var currentLevel))
            {
                if (currentLevel == 0) // First Unlock
                {
                    SkillLevelDict[skillKey] = 1;
                    AddSkill(skillKey, SkillLevelDict[skillKey]);
                    Save.SetInt(_player, skillKey, 1);
                    CallClient_SyncSkill(skillKey, 1);
                    return true;
                }
                else if (currentLevel < maxLevel) // Upgrade, currently unused
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
                    Log.Warn($"Skill {skillKey} reached max level!");
                }
            }
            else
            {
                Log.Error($"SkillTree: Skill {skillKey} NOT FOUND");
            }
        }
        return false;
    }

    /// <summary>
    /// Deprive the player of a skill. Typically only used in admin commands
    /// </summary>
    /// <param name="skillKey"></param>
    public void DepriveSkill(string skillKey)
    {
        if (Network.IsServer)
        {
            if (SkillLevelDict.TryGetValue(skillKey, out var currentLevel))
            {
                SkillLevelDict[skillKey] = 0;
                RemoveSkill(skillKey);
                Save.SetInt(_player, skillKey, 0);
                CallClient_SyncSkill(skillKey, 0);
            }
            else
            {
                Log.Error($"SkillKey {skillKey} not found");
            }
        }
    }

    #endregion

    #region Player Workflow

    public bool IsActiveUnlocked(string skillKey)
    {
        if (SkillConfig.ActiveSkills.Contains(skillKey))
        {
            return SkillLevelDict[skillKey] > 0;
        }
        else
        {
            Log.Error($"SkillTree: Skill {skillKey} is not an ACTIVE Skill!");
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
    /// Called in player's Start function. 
    /// Get the Save of player and populate skill trees. [Save API is only available on server]
    ///
    /// Also: If you need to do any testing and modify player skill level manually, do it here.
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
            }
            
            // After fetching save, we sync those to the client and handle the skill effects
            
            Action<string> handleSkillWithSync = (key) =>
            {
                int lvl = SkillLevelDict[key];
                HandleSkill(key, lvl);
                CallClient_SyncSkill(key, lvl);
            };
            // Phase 0: Default unlock for all players
            UpgradeSkill("Punch", 1);
            
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
            foreach (var sbKey in SkillConfig.SkillEnhanceSkills)
            {
                handleSkillWithSync(sbKey);
            }
            
            Initialized.Set(true);
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
            Log.Debug($"ST Component ID {Id}: Skill Level Get from Server. {skillKey} = {level}");
            SkillLevelDict[skillKey] = level;
            // Add / Remove skill are called on server and automatically synced
            if(Initialized) SkillUpgradeUIEvent.Invoke(skillKey, level); // UI Event
            
        }
        
    }
    
    /// <summary>
    /// [Server Only] Execute the handler of the skill (Initialization from Save data)
    /// </summary>
    /// <param name="skillKey"> Key (ID) of the skill </param>
    /// <param name="level">The active level of the skill we are handling</param>
    public void HandleSkill(string skillKey, int level)
    {
        Log.Debug($"Skill Level Handled on Server. {skillKey} = {level}");
        SkillLevelDict[skillKey] = level;
        if (level != 0)
        {
            AddSkill(skillKey, level);
        }
        
    }

    public bool CheckAffordable(int cost)
    {
        return cost <= _player.Coins;
    }

    /// <summary>
    /// [Client & Server]
    /// </summary>
    public void OnSkillUpgrade(string skillKey, int level)
    {
        
    }

    #endregion
}