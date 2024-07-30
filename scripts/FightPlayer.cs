using System.Collections;
using AO;
using Assembly.scripts;
using Assembly.scripts.UI;
using StreamReader = AO.StreamReader;

/// <summary>
/// Model class of the player. Stores data and handle actions using RPC
/// </summary>
public partial class FightPlayer : Player
{
    // Player status. Note that we need to keep a list in FightClubGameManager for combat hit detection
    [Serialized] public PlayerStatus PlayerStatus = PlayerStatus.Safe;
    [Serialized] protected FightPlayerEffectManager EffectManager; 
    [Serialized] protected FightPlayerUI PlayerUi;
    [Serialized] protected FightPlayerSkillTree SkillTree;
    [Serialized] protected FightPlayerSkillSlotsManager SkillSlotsManager;

    protected Circle_Collider Collider; // MAIN Collider used for damage
    protected Box_Collider PunchCollider;
    protected CameraControl CameraInterface;
    
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
                currentShield.Set(value);
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
                maxShield.Set(value);
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

    private SyncVar<int> _totalCoins = new();

    public int TotalCoins
    {
        get { return _totalCoins.Value;}
        set
        {
            if (Network.IsServer)
            {
                _totalCoins.Set(value);
                Save.SetInt(this, "TotalCoins", value); 
                Save.OrderedSet("TotalCoins", $"{this.UserId}", value);
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

    /// <summary>
    /// Note: This flag doesn't actually make player immune to damage. You need to apply an effect that inherits FightEffectWithImmunity
    /// which removes damage and flinch event from TakeDamage(). This flag is used in projectiles / traps to make them ignore invincible players.
    /// </summary>
    protected List<string> InvincibleReasons = new List<string>();
    

    public bool Damageable()
    {
        return CurrentHealth > 0 && InvincibleReasons.Count == 0 && IsValidTarget;
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
        Coins = Save.GetInt(this, "Coins", 10);
        TotalEliminations = Save.GetInt(this, "TotalEliminations");
        TotalDamageDealt = Save.GetInt(this, "TotalDamageDealt");
    }
    
    #region EventFunctions
    
    public override void Awake()
    {
        FightClubGameManager.Instance.OnPlayerJoin(this);

        if (Network.IsServer)
        {
            EffectManager = Entity.AddComponent<FightPlayerEffectManager>();
            PlayerUi = Entity.AddComponent<FightPlayerUI>();
            SkillTree = Entity.AddComponent<FightPlayerSkillTree>();
            SkillSlotsManager = Entity.AddComponent<FightPlayerSkillSlotsManager>();
        }
        
        EffectManager = Entity.GetComponent<FightPlayerEffectManager>();
        PlayerUi = Entity.GetComponent<FightPlayerUI>();
        SkillTree = Entity.GetComponent<FightPlayerSkillTree>();
        SkillSlotsManager = Entity.GetComponent<FightPlayerSkillSlotsManager>();

        //Log.Debug($"Client Awake!");
        //SkillSlotsManager.InitKeybind();
        if (IsLocal)
        {
            ResourceOverlayWindow resourceWindow =
                UIManager.Instance.OpenOverlayWindow(UniqueWindowKeys.ResourcesOverlayWindowPath) as ResourceOverlayWindow;
            // NOTE: Action is value type. You have to pass them as ref.
            resourceWindow.HookupEvents(ref CoinUpdateEvent, ref TotalDamageUpdateEvent, ref TotalElminationUpdateEvent);
        }

        _preDamageEffects = new List<FightEffect>();
    }

    public override void Start()
    {
        //Log.Debug($"Client Start!");
        
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
                CameraInterface.Zoom = 1.4f;
                
                // First ui update need to be triggered manually (Save reading happens before this point)
                CoinUpdateEvent.Invoke(_coins); 
                TotalDamageUpdateEvent.Invoke(TotalDamageDealt);
                TotalElminationUpdateEvent.Invoke(TotalEliminations);
                _totalDamageDealt.OnSync += (oldi, newi) => { TotalDamageUpdateEvent(newi); }; // Hook up sync var
                _totalEliminations.OnSync += (oldi, newi) => { TotalElminationUpdateEvent(newi); };
            }
            
        }
        // Colliders
        var collisionPrefab = Assets.GetAsset<Prefab>("FatPlayerCollision.prefab"); // Player Collider
        CollisionEntity = collisionPrefab.Instantiate();
        CollisionEntity.GetComponent<PlayerCollisionChild>().Player = this;
        CollisionEntity.LocalScale = new Vector2(1.5f, 1.5f); // Make players easier to hit by giving them Michelin Man hitbox
        CollisionEntity.SetParent(Entity, false);
        // CollisionEntity.LocalPosition =
        //     new Vector2(CollisionEntity.LocalPosition.X, CollisionEntity.LocalPosition.Y);
        Collider = CollisionEntity.GetComponent<Circle_Collider>();

        var punchColliderEntity = CollisionEntity.TryGetChildByName("PunchCollider");
        if (punchColliderEntity != null)
        {
            //Log.Debug("Found Punch Collider!");
            PunchCollider = punchColliderEntity.GetComponent<Box_Collider>();
        }
        else
        {
            Log.Error("Shin: Punch Collider NOT FOUND");
        }
        
        // See FightPlayerAnimation.cs
        InitializeStateMachine();
    }

    public override void Update()
    {
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
            CameraInterface.Position = Vector2.Lerp(new Vector2(CameraInterface.Position.X , CameraInterface.Position.Y + 0.5f), Entity.Position, 0.75f);
        }
        
    }
    
    public override void OnDestroy()
    {
        FightClubGameManager.Instance.OnPlayerLeave(this);
    }
    

    #endregion
    

    #region Health, Damage, Respawn
    
    /// <summary>
    /// [Server & Client, Contains server-only logic] 
    /// The damage function.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="info"></param>
    public void TakeDamage(FightPlayer source, DamageInfo info)
    {
        if (CurrentHealth <= 0) return; // Avoid damaging the dead
        info.SourceNetworkId = source.Entity.NetworkId;
        
        // Pre-damage event, chained invoke
        foreach (var pfe in _preDamageEffects)
        {
            pfe.PreDamageMod(ref info);
        }
        
        
        int damage = info.ReactionInfo.Amount;
        
        if (Network.IsServer) {
            // Actual damage stuff
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
                info.DamageNumberColor = Vector4.LightBlue;
            }
            else
            {
                CurrentHealth = CurrentHealth - damage > MaxHealth ? MaxHealth : CurrentHealth - damage;
            }

            FightClubGameManager.Instance.PlayerDamageEvent.Invoke(source, this, info);
            CallClient_NotifyReceiveDamage(source.Entity, info); // This info is reliable (server dispatched)
            // Player Death
            if (CurrentHealth <= 0)
            {
                //Coroutine.Start(this.Entity, PlayerRespawnCoroutine());
                FightClubGameManager.Instance.PlayerEliminationEvent.Invoke(source, this);
                CallClient_PlayerDeath();
                return;
            }
        }
        DamageReaction(CurrentHealth, info);  // All Client side damage reaction goes here
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
    public void PlayerDeath()
    {
        ClearAllEffects();
        EffectManager.AddEffect<EffectDeath>(null, GlobalData.RespawnTime, null);
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
        return _speedMultipliers.Count > 0 ? _speedMultipliers.Aggregate((x, y) =>  x*y ) : baseSpeed;
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

    public FightPlayerUI GetPlayerUIComp()
    {
        return PlayerUi;
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
        var proximityPlayers = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(Entity.Position, EffectConfig.PunchConfig.PunchTargetRange);
        proximityPlayers.Remove(this);
        return proximityPlayers.Count > 0 ? proximityPlayers[0].Entity.Position - Entity.Position : PunchCollider.Entity.Position - Entity.Position;
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
                //Teleport(Zone.GetRandomPointInZones(combatZone.ZoneId) + combatZone.Entity.Position);
                Teleport(combatZone.Entity.Position);
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
        FightClubGameManager.Instance.PlayerTeleportEvent.Invoke(this);
    }

    public void OnTeleportToCombatZone()
    {
        SkillSlotsManager.SkillSlotsPanelEnable(true);
    }

    public void OnTeleportToSafeZone()
    {
        SkillSlotsManager.SkillSlotsPanelEnable(false);
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
            TakeDamage(this, DamageInfo.CreateHealInfo(10));
            _healTimer = 0;
        }
    }
}