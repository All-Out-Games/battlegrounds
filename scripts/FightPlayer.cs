using System.Collections;
using AO;
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

    public SyncVar<PlayerStatus> test = new SyncVar<PlayerStatus>(PlayerStatus.Safe);

    protected Circle_Collider Collider; // MAIN Collider used for bumping / damage
    protected Box_Collider PunchCollider;
    protected CameraControl CameraInterface;
    
    public Entity CollisionEntity;
    
    #region Attributes
    
    // SyncVars must not be set during Awake(). Do these in Start()

    protected SyncVar<int> currentHealth = new(100);
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
    
    protected SyncVar<int> currentAttack = new(10);
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

    protected SyncVar<int> maxHealth = new(100);
    public int MaxHealth
    {
        get { return maxHealth.Value; }
        set
        {
            if (Network.IsServer)
            {
                maxHealth.Set(value);
            }
        }
    }

    // Shields are temporary values. We do not sync them using syncvars.
    [Serialized] private int currentShield = 0;
    public int CurrentShield
    {
        get { return currentShield; }
        set
        {
            currentShield = value;
            if (Network.IsServer)
            {
                CallClient_SetShield(currentShield);
            }
        }
    }

    // Current Max value of shield
    [Serialized] private int maxShield = 0;
    public int MaxShield
    {
        get { return maxShield; }
        set
        {
            maxShield = value;
            if (Network.IsServer)
            {
                CallClient_SetMaxShield(maxShield);
            }
        }
    }

    private SyncVar<int> totalEliminations = new();

    public int TotalEliminations
    {
        get { return totalEliminations.Value; }
        set
        {
            if (Network.IsServer)
            {
                totalEliminations.Set(value);
                Save.SetInt(this, "TotalEliminations", value);
            }
        }
    }
    private SyncVar<int> totalDamageDealt = new();

    public int TotalDamageDealt
    {
        get { return totalDamageDealt.Value; }
        set
        {
            if (Network.IsServer)
            {
                totalDamageDealt.Set(value);
                Save.SetInt(this, "TotalDamageDealt", value);
            }
        }
    }
    private int coins = 0;
    public int Coins
    {
        get { return coins; }
        set
        {
            coins = value; 
            if (Network.IsServer) 
            {
                Save.SetInt(this, "Coins", value);
                CallClient_NotifyCoinUpdate(value);
            }
        }
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
    }

    public override void Start()
    {
        //Log.Debug($"Client Start!");
        
        if (Network.IsServer)
        {
            // DO save related things here! You cannot sync stuff in Awake
            ProcessSave();
            SkillTree.InitializeSkillTreeComp();
            SkillTree.HandleAllSkills();
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
                CoinUpdateEvent.Invoke(coins); 
                TotalDamageUpdateEvent.Invoke(TotalDamageDealt);
                TotalElminationUpdateEvent.Invoke(TotalEliminations);
                totalDamageDealt.OnSync += (oldi, newi) => { TotalDamageUpdateEvent(newi); }; // Hook up sync var
                totalEliminations.OnSync += (oldi, newi) => { TotalElminationUpdateEvent(newi); };
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
        
    }

    public override void Update()
    {
        BumpDecay();
        DashDecay();
        
        // switch (PlayerStatus)
        // {
        //     case PlayerStatus.Combat:
        //         break;
        //     case PlayerStatus.Safe:
        //         break;
        //     case PlayerStatus.AFK:
        //         break;
        //     case PlayerStatus.Spectating:
        //         break;
        //     default:
        //         throw new ArgumentOutOfRangeException();
        // }
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
    

    [ClientRpc]
    public void SetShield(int shield)
    {
        currentShield = shield;
    }

    [ClientRpc]
    public void SetMaxShield(int shield)
    {
        maxShield = shield;
    }

    /// <summary>
    /// [Server Only] The damage function on the server side.
    /// TODO: Damage type and source + OnDamage Event for effects to register
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="source"></param>
    /// <param name="info"></param>
    public void TakeDamage(int damage, FightPlayer source, DamageInfo info)
    {
        if (CurrentHealth <= 0) return; // Avoid damaging the dead
        
        if (CurrentShield > 0)
        {
            CurrentShield -= damage;
            if (CurrentShield <= 0)
            {
                // Shield is not enough
                CurrentHealth += CurrentShield;
                CurrentShield = 0;
                info.ShieldBroken = true;
            }
        }
        else
        {
            CurrentHealth -= damage;
        }
        

        if (Network.IsServer) {
            CallClient_DamageReaction(CurrentHealth, damage, info); // All Client side damage reaction goes here
            FightClubGameManager.Instance.PlayerDamageEvent(source, this, damage);
            // Server only death routine (client-side handled in CallClient_TakeDamage)
            if (CurrentHealth <= 0)
            {
                //Coroutine.Start(this.Entity, PlayerRespawnCoroutine());
                FightClubGameManager.Instance.PlayerEliminationEvent.Invoke(source, this);
            }
        }
        
    }
    
    [ClientRpc]
    public void DamageReaction(int health, int damage, DamageInfo info)
    {
        // DO NOT use CurrentHealth SyncVar in this frame
        // It might not arrive yet at this point. Trust the info sent from the triggering function on server (i.e. the parameters) here
        if (info.ShieldBroken)
        {
            ShieldBreakEvent?.Invoke();
        }
        
        if (health <= 0)
        {
            // If the damage caused a death...
            Log.Debug("Death Triggered By RPC");
            PlayerDeath();
        }
        else
        {
            // Normal damage route
            if (info.Flinch)
            {
                SetAnimTrigger("flinch");
            }
            
        }
    }

    
    /// <summary>
    /// [Server & Client]
    /// </summary>
    protected void PlayerDeath()
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
        return _speedMultipliers.Count > 0 ? _speedMultipliers.Aggregate((x, y) =>  x*y ) : 1.0f;
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
            //Log.Info("Adding Movement Limitations");
            EffectManager.AddEffect<EffectNoMovement>(null, 0.75f);
        }
        
        Bump = add; // Changed from accumulation to directly set

        if (reset) {
            this.Entity.GetComponent<Rigidbody>().Velocity *= 0.001f;
        }
    }
    
    public void AddBumpFrom(FightPlayer caster, Vector2 add, bool reset)
    {
        if (Network.IsServer)
        {
            CallClient_AddBump(add,reset);
        }
        
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
        Dash = add;
        DashRemainingDuration = duration;
    }

    public void AddDash_Server(Vector2 add, float duration)
    {
        if (Network.IsServer)
        {
            CallClient_AddDash(add, duration);
        }
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

    public Vector2 GetPunchDirection()
    {
        return PunchCollider.Entity.Position - Entity.Position;
    }

    #endregion

    #region Actions

    
    public void SetAnimTrigger(string variableName)
    {
        SpineAnimator.SpineInstance.StateMachine.SetTrigger(variableName);
    }
    [ClientRpc]
    public void SetAnimTriggerBroadcast(string variableName)
    {
        if (!IsLocal)
        {
            SpineAnimator.SpineInstance.StateMachine.SetTrigger(variableName);
        }
    }

    /// <summary>
    /// [Server Only]
    /// Put all general conditions of casting an active skill here.
    /// They must all be satisfied before the server dispatch a skill cast.
    /// We only check conditions related to the player here. Cooldown & silent are checked in SkillSlot.
    /// </summary>
    /// <returns></returns>
    public bool SkillCastGeneralCheck()
    {
        return PlayerStatus == PlayerStatus.Combat;
    }
    
    public FightAbility GetFightAbility<T>() where T : Ability
    {
        return GetAbility<T>() as FightAbility;
    }

    public FightAbility GetFightAbility(Type t)
    {
        return AbilityInstances.FirstOrDefault<Ability>((Func<Ability, bool>) (a => a.GetType() == t)) as FightAbility;
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
    
}