using System.Collections;
using AO;
using Assembly.scripts;
using Assembly.scripts.Effects.ActiveSkills;
using Assembly.scripts.SceneObjects.Crates;

/// <summary>
/// Model class of the player. Stores data and handle actions using RPC
/// </summary>
public partial class FightPlayer : Player
{
    // Player status. Note that we need to keep a list in FightClubGameManager for combat hit detection
    [Serialized] public PlayerStatus PlayerStatus = PlayerStatus.Safe;
    [Serialized] protected FightPlayerEffectManager EffectManager; 
    [Serialized] protected FightPlayerLegacyUI PlayerLegacyUi;
    [Serialized] protected FightPlayerSkillTree SkillTree;
    [Serialized] protected FightPlayerSkillSlotsManager SkillSlotsManager;

    protected Circle_Collider Collider; // MAIN Collider used for damage
    protected CameraControl CameraInterface;
    protected FightPlayer PriorityTarget;
    
    public Entity CollisionEntity;
    
    #region Attributes
    
    // SyncVars must not be set during Awake(). Do these in Start()

    private SyncVar<int> currentHealth = new(GlobalData.DefaultMaxHealth);
    public int CurrentHealth 
    { 
        get => currentHealth.Value;
        set 
        {
            if (Network.IsServer) {
                currentHealth.Set(value);
            }
        }
    }

    private SyncVar<int> currentAttack = new(GlobalData.DefaultAtk);
    public int CurrentAttack
    {
        get { return currentAttack.Value; }
        set
        {
            if (Network.IsServer) {
                currentAttack.Set(value);
            }
        }
    }

    private SyncVar<int> maxHealth = new(GlobalData.DefaultMaxHealth);
    public int MaxHealth
    {
        get { return maxHealth.Value; }
        set
        {
            if (Network.IsServer)
            {
                maxHealth.Set(value);
                currentHealth.Set(value); // If max health changed, always recover the player to full
            }
        }
    }

    // Shields are temporary values. We do not sync them using syncvars.
    private SyncVar<int> currentShield = new(0);
    public int CurrentShield
    {
        get { return currentShield; }
        set
        {
            if (Network.IsServer)
            {
                currentShield.Set(int.Max(0, value));
            }
        }
    }

    // Current Max value of shield
    private SyncVar<int> maxShield = new(0);
    public int MaxShield
    {
        get { return maxShield.Value; }
        set
        {
            if (Network.IsServer)
            {
                maxShield.Set(int.Max(0, value));
            }
        }
    }

    private SyncVar<int> _totalEliminations = new();

    public int TotalEliminations
    {
        get { return _totalEliminations.Value; }
        set
        {
            if (Network.IsServer)
            {
                _totalEliminations.Set(value);
                Save.SetInt(this, "TotalEliminations", value);
                Save.OrderedSet("TotalEliminations", $"{this.UserId}", value);
            }
        }
    }
    private SyncVar<int> _totalDamageDealt = new();

    public int TotalDamageDealt
    {
        get { return _totalDamageDealt.Value; }
        set
        {
            if (Network.IsServer)
            {
                _totalDamageDealt.Set(value);
                Save.SetInt(this, "TotalDamageDealt", value);
                Save.OrderedSet("TotalDamageDealt", $"{this.UserId}", value);
            }
        }
    }
    
    private int _coins = 0;
    public int Coins
    {
        get => _coins;
        set
        {
            _coins = value; 
            if (Network.IsServer) 
            {
                Save.SetInt(this, "Coins", value);
                CallClient_NotifyCoinUpdate(value);
            }
        }
    }

    private SyncVar<int> _punchLvl = new(1);

    public int PunchLevel
    {
        get => _punchLvl;
        set
        {
            if (Network.IsServer)
            {
                _punchLvl.Set(value);
            }
        }
    }

    private SyncVar<int> _combatSpeedPercentage = new(100);

    public int CombatSpeedPercentage
    {
        get => _combatSpeedPercentage;
        set
        {
            if (Network.IsServer)
            {
                _combatSpeedPercentage.Set(value);
            }
        }
    }

    private SyncVar<int> _exp = new(0);

    public int Exp
    {
        get => _exp;
        set
        {
            if (Network.IsServer)
            {
                int e = value;
                if (value > LevelingData.MaxXp)
                {
                    e = LevelingData.MaxXp;
                }
                _exp.Set(e);
                Save.SetInt(this, "Exp", e);
                if (_exp >= LevelingData.NextLevelXp[_level])
                {
                    TryLevelUp();
                }
            }
        }
    }

    private SyncVar<int> _level = new(0);

    public int Level
    {
        get => _level;
        set
        {
            if (Network.IsServer)
            {
                Save.SetInt(this, "Level", value);
                _level.Set(value);
            }
        }
    }

    private SyncVar<int> _gem = new(0);

    public int Gem
    {
        get => _gem;
        set
        {
            if (Network.IsServer)
            {
                _gem.Set(value);
                Save.SetInt(this, "Gem", value);
            }
        }
    }

    private SyncVar<int> _expBoostTime = new(0);

    public int ExpBoostTime
    {
        get => _expBoostTime;
        set
        {
            if (Network.IsServer)
            {
                _expBoostTime.Set(value);
                Save.SetInt(this, "ExpBoostTime", value);
            }
        }
    }

    private SyncVar<int> _expBoostMultiplier = new(1);

    public int ExpBoostMultiplier
    {
        get => _expBoostMultiplier;
        set
        {
            if (Network.IsServer)
            {
                _expBoostMultiplier.Set(value);
                Save.SetInt(this, "ExpBoostMultiplier", value);
            }
        }
    }

    public bool IsExpBoosted()
    {
        return ExpBoostTime > 0;
    }

    public void AddExpBoostTime(int minutes, int multiplier = 2)
    {
        ExpBoostTime += minutes;
        ExpBoostMultiplier = multiplier;
    }

    /// <summary>
    /// [Server Only] Try to update the level if the player has a XP that exceeds the next level's baseline.
    /// </summary>
    private void TryLevelUp()
    {
        // At this point Exp is more than NextLevelXp[_level].
        // Also note that MaxLevel is index based. The displayed level is _level+1 (i.e. MaxLevel = 29 means the max level is 30)
        if (Level >= LevelingData.MaxLevel)
        {
            Log.Warn("Max Level hit!");
            return;
        }
        
        int prevLevel = Level;
        // Find next level's xp ceiling, which is the first xp ceiling that's more than the current xp of the player
        int nextXp = LevelingData.NextLevelXp.FirstOrDefault(p => p > Exp, -1);
        if (nextXp == -1)
        {
            Log.Error($"{Name} Overflowed the max level! This shouldn't happen unless they are granted a large amount of xp");
            Level = LevelingData.MaxLevel;
            for (int j = prevLevel+1; j <= Level; j++)
            {
                Coins += LevelingData.CoinRewards[j];
                Gem += LevelingData.GemRewards[j];
            }

        }
        else if(Exp < LevelingData.NextLevelXp[_level+1])
        {
            // Usual case where we raise the player level by one
            Level += 1;
            Coins += LevelingData.CoinRewards[Level];
            int g = LevelingData.GemRewards[Level];
            if (g != 0)
            {
                Gem += g;
            }
        }
        else
        {
            
            // If the player exp exceeds even the next level's requirement... [Usually only happens with grant command]
            int newLevel = Level;
            for (int i = Level; i <= LevelingData.MaxLevel; i++)
            {
                if (LevelingData.NextLevelXp[i] == nextXp)
                {
                    newLevel = i;
                    Log.Warn($"Skipping happened to Player {Name} Level - From {prevLevel} to {newLevel}");
                }
            }

            for (int j = prevLevel+1; j <= newLevel; j++)
            {
                Coins += LevelingData.CoinRewards[j];
                Gem += LevelingData.GemRewards[j];
            }

            Level = newLevel;
        }
    }

    /// <summary>
    /// [Server Only] Remove XP, Skills & Level
    /// </summary>
    public void Rebirth()
    {
        Exp = 0;
        Level = 0;
        Coins = 0;
        Gem = 0;
        foreach (var kv in SkillTree.SkillLevelDict)
        {
            if (kv.Value > 0 && kv.Key != "Punch")
            {
                SkillTree.DepriveSkill(kv.Key);
            }
        }
    }

    /// <summary>
    /// Note: This flag doesn't actually make player immune to damage. You need to apply an effect that inherits FightEffectWithImmunity
    /// which removes damage and flinch event from TakeDamage(). This flag is used in projectiles / traps to make them ignore invincible players.
    /// </summary>
    protected List<string> InvincibleReasons = new List<string>();
    

    public bool Damageable()
    {
        return CurrentHealth > 0 && InvincibleReasons.Count == 0 && IsValidTarget;
    }

    public bool Targetable()
    {
        return !HasEffect<EffectInvisible>() && Damageable();
    }

    public void AddInvincibilityReason(string reason)
    {
        InvincibleReasons.Add(reason);
    }

    public void RemoveInvincibilityReason(string reason)
    {
        InvincibleReasons.Remove(reason);
    }

    #endregion

    /// <summary>
    /// [Server Only] Use attribute setters to sync player save data to the client.
    /// </summary>
    public void ProcessSave()
    {
        Coins = Save.GetInt(this, "Coins");
        Gem = Save.GetInt(this, "Gem");
        TotalEliminations = Save.GetInt(this, "TotalEliminations");
        TotalDamageDealt = Save.GetInt(this, "TotalDamageDealt");
        Level = Save.GetInt(this, "Level");
        Exp = Save.GetInt(this, "Exp");
        ExpBoostTime = Save.GetInt(this, "ExpBoostTime");
        ExpBoostMultiplier = Save.GetInt(this, "ExpBoostMultiplier");
    }
    
    #region EventFunctions
    
    public override void Awake()
    {
        if (Network.IsServer)
        {
            EffectManager = Entity.AddComponent<FightPlayerEffectManager>();
            PlayerLegacyUi = Entity.AddComponent<FightPlayerLegacyUI>();
            SkillTree = Entity.AddComponent<FightPlayerSkillTree>();
            SkillSlotsManager = Entity.AddComponent<FightPlayerSkillSlotsManager>();
        }
        
        EffectManager = Entity.GetComponent<FightPlayerEffectManager>();
        PlayerLegacyUi = Entity.GetComponent<FightPlayerLegacyUI>();
        SkillTree = Entity.GetComponent<FightPlayerSkillTree>();
        SkillSlotsManager = Entity.GetComponent<FightPlayerSkillSlotsManager>();
        
        // Create state machine
        // See FightPlayerAnimation.cs
        InitializeStateMachine();

        NameOffset = 0.275f;

        //Log.Debug($"Client Awake!");
        //SkillSlotsManager.InitKeybind();
        
        _preDamageEffects = new List<FightEffect>();
        
        // Spawn Colliders
        var collisionPrefab = Assets.GetAsset<Prefab>("FatPlayerCollision.prefab"); // Player Collider
        CollisionEntity = collisionPrefab.Instantiate();
        CollisionEntity.GetComponent<PlayerCollisionChild>().Player = this;
        CollisionEntity.LocalScale = new Vector2(1.5f, 1.5f); // Make players easier to hit by giving them Michelin Man hitbox
        CollisionEntity.SetParent(Entity, false);
        // CollisionEntity.LocalPosition =
        //     new Vector2(CollisionEntity.LocalPosition.X, CollisionEntity.LocalPosition.Y);
        Collider = CollisionEntity.GetComponent<Circle_Collider>();
        
        
        InitializeUI();
        FightClubGameManager.Instance.OnPlayerJoin(this);
        UIManager.Instance.OnPlayerJoin(this);
        
        Teleport(FightClubGameManager.References.CentralHubZone.Position );
    }

    private bool _lazyInited;
    /// <summary>
    /// Previously named Start(). That function is REMOVED from the engine!
    /// </summary>
    public void LazyInit()
    {
        if (Network.IsServer)
        {
            // DO save related things here! You cannot sync stuff in Awake
            ProcessSave();
            SkillTree.InitializeSkillTreeComp();
            HookupGlobalEvents();
        }
        else
        {
            if (IsLocal)
            {
                // Stuff related to the local player goes here. e.g. Camera control & UI
                CameraInterface = Camera.CreateCameraControl(1);
                CameraInterface.Zoom = 1.0f;
                // First ui update need to be triggered manually (Save reading happens before this point)
                CoinUpdateEvent.Invoke(_coins); 
            }
            
        }
    }

    public override void Update()
    {
        if (Util.OneTime(true, ref _lazyInited))
        {
            LazyInit();
        }
        BumpDecay();
        DashDecay();
        if (Network.IsServer && PlayerStatus != PlayerStatus.Combat)
        {
            PeriodicalHeal(Time.DeltaTime);
        }
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        if (IsLocal)
        {
            CameraInterface.Position = Entity.Position + new Vector2(0, 0.5f);
        }

        PlayerLegacyUi.DrawUI();
    }

    public override void OnDestroy()
    {
        FightClubGameManager.Instance.OnPlayerLeave(this);
        RemoveGlobalEvents();
    }
    

    #endregion
    

    #region Health, Damage, Respawn
    
    /// <summary>
    /// [Server & Client, Contains server-only logic] 
    /// The damage function.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="info"></param>
    public DamageInfo.DamageNumberOverrideType TakeDamage(FightPlayer source, DamageInfo info)
    {
        if (CurrentHealth <= 0 || !source.Alive() || PlayerStatus != PlayerStatus.Combat) return DamageInfo.DamageNumberOverrideType.Immune; // Avoid damaging the dead, avoid dropped player
        info.SourceNetworkId = source.Entity.NetworkId;
        
        // Pre-damage event, chained invoke
        foreach (var pfe in _preDamageEffects)
        {
            pfe.PreDamageMod(ref info);
        }
        
        
        int damage = info.ReactionInfo.Amount;
        
        if (Network.IsServer)
        {
            bool isDamage = damage > 0;
            // Actual damage stuff
            if (isDamage)
            {
                // Shielded damage
                if (CurrentShield > 0)
                {
                    CurrentShield -= damage;
                    if (CurrentShield <= 0)
                    {
                        // Shield is not enough
                        CurrentHealth += CurrentShield;
                        CurrentShield = 0;
                        info.ReactionInfo.ShieldBroken = true;
                    }
                    info.DamageNumberColor = GlobalData.ShieldNumberColor;
                }
                else
                {
                    CurrentHealth = CurrentHealth - damage > MaxHealth ? MaxHealth : CurrentHealth - damage;
                }
            }
            else
            {
                // isHeal
                CurrentHealth = CurrentHealth - damage > MaxHealth ? MaxHealth : CurrentHealth - damage;
            }

            FightClubGameManager.Instance.PlayerDamageEvent.Invoke(source, this, info);
            CallClient_NotifyReceiveDamage(source, info); // This info is reliable (server dispatched)
            // Player Death
            if (CurrentHealth <= 0)
            {
                FightClubGameManager.Instance.PlayerEliminationEvent.Invoke(source, this, info);
            
                CallClient_PlayerDeath(info, info.SkillKey); // TODO: The engine does not support str serialization in structs yet
                return info.OverrideDamageNumber;
            }
        }
        DamageReaction(CurrentHealth, info);  // All Client side damage reaction goes here
        return info.OverrideDamageNumber;
    }
    
    public void DamageReaction(int health, DamageInfo info)
    {
        if (info.ReactionInfo.Flinch)
        {
            SetAnimTrigger("flinch");
        }
    }

    
    /// <summary>
    /// [Server & Client]
    /// </summary>
    [ClientRpc]
    public void PlayerDeath(DamageInfo damageInfo, string skillKey)
    {
        ClearAllEffects();
        EffectManager.AddEffect<EffectDeath>(null, GlobalData.RespawnTime, death =>
        {
            if (damageInfo.SpecialDeathAnimation)
            {
                death.DeathAnimationTrigger = EffectDeath.GetSpecialDeathAnimationTrigger(skillKey);
            }
        });
    }
    
    #endregion

    #region Movement

    // Basic Movement Speed Modifier (related to buff)
    private List<float> _speedMultipliers = new List<float>();
    private float GetTotalVelocityMultiplier()
    {
        float baseSpeed = PlayerStatus == PlayerStatus.Combat
            ? GlobalData.CombatSpeedModifier * _combatSpeedPercentage / 100f
            : GlobalData.SafeSpeedModifier;
        return _speedMultipliers.Count > 0 ? _speedMultipliers.Aggregate((x, y) =>  x*y ) * baseSpeed : baseSpeed;
    }

    public void AddSpeedModifier(float md)
    {
        _speedMultipliers.Add(md);
    }

    public void RemoveSpeedModifier(float md)
    {
        if (!_speedMultipliers.Remove(md))
        {
            Log.Error($"FightPlayer: The Modifier {md} is not found!");
        }
    }

    public void ClearSpeedModifier()
    {
        _speedMultipliers.Clear();
    }
    public override Vector2 CalculatePlayerVelocity(Vector2 currentVelocity, Vector2 input, float deltaTime)
    {
        if (CurrentHealth <= 0) {
            return Vector2.Zero;
        }
        
        var velocity = DefaultPlayerVelocityCalculation(currentVelocity, input, deltaTime, GetTotalVelocityMultiplier());
        velocity += Bump * deltaTime;
        velocity += Dash * deltaTime;
        return velocity;
    }

    // Bump
    protected Vector2 Bump = Vector2.Zero;
    /// <summary>
    /// Called each frame to decay bump
    /// </summary>
    protected void BumpDecay()
    {
        Bump = Vector2.Lerp(Bump, Vector2.Zero, Time.DeltaTime * 2.0f);
    }
    
    [ClientRpc]
    public void AddBump(Vector2 add, bool reset)
    {
        if (!reset)
        {
            EffectManager.AddEffect<EffectNoMovement>(null, 0.75f);
        }
        
        Bump = add; // Changed from accumulation to directly set

        if (reset) {
            this.Entity.GetComponent<Rigidbody>().Velocity *= 0.001f;
        }
    }
    
    public void AddBumpFrom(FightPlayer caster, Vector2 add, bool reset)
    {
        AddBump(add, reset);
    }
    
    // Dash
    public Vector2 Dash = Vector2.Zero;
    protected float DashRemainingDuration;
    protected const float DashDecayThreshold = 0.1f;

    protected void DashDecay()
    {
        DashRemainingDuration -= Time.DeltaTime;
        Dash = DashRemainingDuration > DashDecayThreshold
            ? Dash
            : Vector2.Lerp(Dash, Vector2.Zero, Time.DeltaTime * 10.0f);
        //Bump = Vector2.Lerp(Bump, Vector2.Zero, Time.DeltaTime * 2.0f);
    }
    
    [ClientRpc]
    public void AddDash(Vector2 add, float duration)
    {
        SetFacingDirection(add.X > 0);
        Dash = add;
        DashRemainingDuration = duration;
    }
    
    
    
    #endregion
    
    #region Sub Component Getters

    public FightPlayerEffectManager GetEffectMgr()
    {
        return EffectManager;
    }

    public FightPlayerSkillTree GetSkillTree()
    {
        return SkillTree;
    }

    public FightPlayerSkillSlotsManager GetSkillSlots()
    {
        return SkillSlotsManager;
    }

    public FightPlayerLegacyUI GetPlayerUIComp()
    {
        return PlayerLegacyUi;
    }

    public bool HasSkill(string key)
    {
        bool keyExist = SkillTree.SkillLevelDict.TryGetValue(key, out int lvl);
        if (!keyExist)
        {
            Log.Error($"{key} is not a valid skillKey! Did you forget to add it to the skill hashset in SkillConfigTable.cs?");
        }
        return lvl > 0;
    }

    #endregion

    #region Collision

    public void AddPlayerCollisionFunction(Action<Entity> collisionFunc)
    {
        Collider.OnCollisionEnter += collisionFunc;
    }

    public void RemovePlayerCollisionFunction(Action<Entity> collisionFunc)
    {
        Collider.OnCollisionEnter -= collisionFunc;
    }

    /// <summary>
    /// Auto aim towards the closest player, if no other players are detected, punch forward.
    /// </summary>
    /// <returns></returns>
    public Vector2 GetPunchDirection()
    {
        var proximityPlayers = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(Entity.Position, EffectConfig.PunchConfig.PunchTargetRange, this);
        proximityPlayers.Remove(this);
        if (proximityPlayers.Count > 0)
        {
            // Aim for priority target if you have one 
            if (PriorityTarget != null && proximityPlayers.Contains(PriorityTarget))
            {
                // PriorityTarget is set in NotifyDealDamage, which is essentially the player you recently damaged. 
                if (PriorityTarget.Targetable())
                {
                    return PriorityTarget.Position - Position;
                }
                else
                {
                    PriorityTarget = null; 
                }
            }
            
            foreach (var fp in proximityPlayers)
            {
                // Ignore invis player, ignore invincible/dead player
                if (!fp.Targetable())
                {
                    continue;
                }
                return fp.Position - Position;
            }
        }
        
        // If no player is detected, try find crates.
        var proximityCrates =
            CrateManager.Instance.OverlapCircleForCrates(Position, EffectConfig.PunchConfig.PunchTargetRange);
        if (proximityCrates.Count > 0)
        {
            foreach (var cr in proximityCrates)
            {
                // Damageable crate
                if (cr.Alive() && cr.Damageable())
                {
                    return cr.Entity.Position - Position;
                }
                
            }
        }
        return GetFacingDirectionAsVector();
    }

    #endregion

    #region Actions

    /// <summary>
    /// [Server Only]
    /// Put all general conditions of casting an active skill here.
    /// They must all be satisfied before the server dispatch a skill cast.
    /// We only check conditions related to the player here. Cooldown & silent are checked in SkillSlot.
    /// </summary>
    /// <returns></returns>
    public bool SkillCastGeneralCheck()
    {
        return PlayerStatus == PlayerStatus.Combat && CurrentHealth > 0;
    }
    
    public FightAbility GetFightAbility<T>() where T : Ability
    {
        return GetAbility<T>() as FightAbility;
    }

    public FightAbility GetFightAbility(Type t)
    {
        return AbilityInstances.FirstOrDefault<Ability>((Func<Ability, bool>) (a => a.GetType() == t)) as FightAbility;
    }

    public Vector2 GetFacingDirectionAsVector()
    {
        return GetFacingDirection() ? Vector2.Right : Vector2.Left;
    }
    #endregion

    #region Zone Management

    [ClientRpc]
    public void SwitchStatus(int statusInt)
    {
        PlayerStatus status = (PlayerStatus)statusInt;
        PlayerStatus = status;
        if (status == PlayerStatus.Combat)
        {
            if (Network.IsServer)
            {
                Zone combatZone = FightClubGameManager.References.PvpZone;
                //Teleport(new Vector2(216.504f, 90.571f));
                Teleport(FightClubUtils.RandomPositionInCircle(combatZone.Entity.Position, combatZone.Entity.LocalScaleX));
            }
            OnTeleportToCombatZone();
        }
        else if (status == PlayerStatus.Safe)
        {
            if (Network.IsServer)
            {
                Zone hubZone = FightClubGameManager.References.CentralHubZone;
                //Teleport(Zone.GetRandomPointInZones(hubZone.ZoneId) + hubZone.Entity.Position);
                Teleport(hubZone.Entity.Position);
            }
            OnTeleportToSafeZone();
        }
        else if (status == PlayerStatus.AFK)
        {
            if (Network.IsServer)
            {
                Zone afkZone = FightClubGameManager.References.AfkZone;
                Teleport(afkZone.Entity.Position);
            }
            OnTeleportToAfkZone();
        }
    }

    public void OnTeleportToCombatZone()
    {
        SkillSlotsManager.SkillSlotsPanelEnable(true);
        PlayerSwitchZoneEvent?.Invoke((int)PlayerStatus.Combat);

        if (IsLocal)
        {
            UIManager.Instance.CloseAllUniqueWindow();
        }
    }

    public void OnTeleportToSafeZone()
    {
        SkillSlotsManager.SkillSlotsPanelEnable(false);
        PlayerSwitchZoneEvent?.Invoke((int)PlayerStatus.Safe);

        if (IsLocal)
        {
            UIManager.Instance.CloseAllUniqueWindow();
        }
    }

    public void OnTeleportToAfkZone()
    {
        SkillSlotsManager.SkillSlotsPanelEnable(false);
        PlayerSwitchZoneEvent?.Invoke((int)PlayerStatus.AFK);
        if (IsLocal)
        {
            UIManager.Instance.CloseAllUniqueWindow();
        }
    }

    #endregion

    public void AddScreenShake(float intensity, float duration)
    {
        if (IsLocal)
        {
            CameraInterface.Shake(intensity, duration);
        }
    }

    private float _healTimer;
    
    /// <summary>
    /// Server Only, Safezone heal
    /// </summary>
    /// <param name="deltaTime"></param>
    public void PeriodicalHeal(float deltaTime)
    {
        if (CurrentHealth >= MaxHealth)
        {
            return;
        }
        _healTimer += deltaTime;
        if (_healTimer > 1)
        {
            // This is called outside of combat, so we don't use takedamage. Instead, we heal directly
            CurrentHealth = Int32.Min(CurrentHealth + 20, MaxHealth);
            _healTimer = 0;
        }
    }
}