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

    protected FightPlayerEffectManager _effectManager;
    protected FightPlayerUI _playerUI;
    protected Polygon_Collider _collider;

    #endregion

    public override void Awake()
    {
        _effectManager = AddFightPlayerComponent<FightPlayerEffectManager>();
        _playerUI = AddFightPlayerComponent<FightPlayerUI>();
        base.Awake();
    }

    public override void Start()
    {
        _collider = Entity.GetComponent<Polygon_Collider>();
        if (_collider == null)
        {
            Log.Error("Collider not found");
        }
    }

    public override void Update()
    {
        BumpDecay();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public T AddFightPlayerComponent<T>() where T : FightPlayerComponent
    {
        T fpc = Entity.AddComponent<T>();
        fpc.AssignPlayer(this);
        return fpc;
    }

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

        SpineAnimator.SpineInstance.StateMachine.SetTrigger("flinch");

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

    public void AddBump(Vector2 add, bool reset)
    {
        Bump += add;

        if (reset) {
            this.Entity.GetComponent<Rigidbody>().Velocity *= 0.001f;
        }
    }
    
    #endregion

    #region EffectManager

    public FightPlayerEffectManager GetEffectMgr()
    {
        return _effectManager;
    }

    #endregion

    #region Collision

    public void AddPlayerCollisionFunction(Action<Entity> collisionFunc)
    {
        _collider.OnCollisionEnter += collisionFunc;
    }

    public void RemovePlayerCollisionFunction(Action<Entity> collisionFunc)
    {
        _collider.OnCollisionEnter -= collisionFunc;
    }

    #endregion
    
}