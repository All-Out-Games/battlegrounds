using AO;
/// <summary>
/// Model class of the player. Stores data and handle actions using RPC
/// </summary>
public partial class FightPlayer : Player
{
    #region Attributes

    protected SyncVar<int> TotalEliminations = new();
    protected SyncVar<int> TotalDamageDealt = new();

    private int currentHealth = 100;
    [Serialized] public int MaxHealth = 100;
    public int CurrentHealth 
    { 
        get { return currentHealth; } 
        set 
        {
            if (Network.IsServer) {
                CallClient_SetHealth(value);
            }
            currentHealth = value; 
        }
    }

    private int currentAttack = 5;
    public int CurrentAttack
    {
        get { return currentAttack; }
        set
        {
            if (Network.IsServer) {
                CallClient_SetAttack(currentAttack);
            }
            currentAttack = value; 
        }
    }

    // Player status. Note that we need to keep a list in FightClubGameManager for combat hit detection
    public EffectConfig.PlayerStatus PlayerStatus = EffectConfig.PlayerStatus.Combat;

    protected FightPlayerEffectManager EffectManager;
    protected FightPlayerUI PlayerUi;
    protected FightPlayerSkillTree SkillTree;
    protected FightPlayerSkillSlots SkillSlots;
    
    protected Circle_Collider Collider; // MAIN Collider used for bumping / damage
    protected Box_Collider PunchCollider;
    
    public Entity CollisionEntity;

    #endregion

    public T AddFightPlayerComponent<T>() where T : FightPlayerComponent
    {
        T fpc = Entity.AddComponent<T>();
        fpc.AssignPlayer(this);
        return fpc;
    }
    
    #region EventFunctions
    
    public override void Awake()
    {
        EffectManager = AddFightPlayerComponent<FightPlayerEffectManager>();
        
        PlayerUi = AddFightPlayerComponent<FightPlayerUI>();
        
        SkillTree = AddFightPlayerComponent<FightPlayerSkillTree>();

        SkillSlots = AddFightPlayerComponent<FightPlayerSkillSlots>();
    }

    public void OnLoad()
    {
        // Called after Playerdata loaded
        SkillTree.InitializeSkillTreeComp();
        SkillTree.HandleAllSkills();
    }
    
    public override void Start()
    {
        // Colliders
        
        var collisionPrefab = Assets.GetAsset<Prefab>("FatPlayerCollision.prefab"); // Player Collider
        CollisionEntity = collisionPrefab.Instantiate();
        CollisionEntity.GetComponent<PlayerCollisionChild>().Player = this;
        CollisionEntity.LocalScale = new Vector2(1.01f, 1.01f);
        CollisionEntity.SetParent(Entity, false);
        Collider = CollisionEntity.GetComponent<Circle_Collider>();

        var punchColliderEntity = Entity.TryGetChildByName_Internal(CollisionEntity.Id, "PunchCollider"); // TODO: No public API yet for get child by name
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
        
        ControllerUpdate();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }
    
    #endregion



    #region Health, Damage, Respawn

    [ClientRpc]
    public void DoRespawn()
    {
        // TODO: Ref Gunr
    }

    [ClientRpc]
    public void SetHealth(int health)
    {
        currentHealth = health;
    }

    [ClientRpc]
    public void SetAttack(int attack)
    {
        currentAttack = attack;
        
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;

        if (Network.IsServer) {
            CallClient_TakeDamage(CurrentHealth, damage);
        }
    }
    
    [ClientRpc]
    public void TakeDamage(int health, int damage)
    {
        CurrentHealth = health;

        SetAnimTrigger("flinch");

        if (CurrentHealth <= 0)
        {
            Log.Debug("Death Triggered By RPC");
            //MoveToLobbyArea();
        }
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
        velocity += Bump * deltaTime; // No bump for now, see EffectRollOut.cs
        return velocity;
    }

    // Bump
    public Vector2 Bump = Vector2.Zero;

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
            Log.Info("Adding Movement Limitations");
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

    public FightPlayerSkillSlots GetSkillSlots()
    {
        return SkillSlots;
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

    public Vector2 GetFacingDirection()
    {
        return PunchCollider.Entity.Position - Entity.Position;
    }

    #endregion

    #region Actions

    public void SetAnimTrigger(string variableName)
    {
        SpineAnimator.SpineInstance.StateMachine.SetTrigger(variableName);
    }

    #endregion
    
}