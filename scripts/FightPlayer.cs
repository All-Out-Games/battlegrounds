using AO;

/// <summary>
/// Model class of the player. Stores data and handle actions using RPC
/// </summary>
public partial class FightPlayer : Player
{
    #region Attributes

    private SyncVar<int> TotalEliminations = new();
    private SyncVar<int> TotalDamageDealt = new();
    
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

    private FightPlayerEffectManager _effectManager;

    #endregion

    public override void Awake()
    {
        _effectManager = Entity.AddComponent<FightPlayerEffectManager>();
        _effectManager.AssignPlayer(this);
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
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
        var velocity = DefaultPlayerVelocityCalculation(currentVelocity, input, deltaTime, GetTotalVelocityMultiplier());
        return velocity;
    }

    #endregion

    #region EffectManager

    public FightPlayerEffectManager GetEffectMgr()
    {
        return _effectManager;
    }

    #endregion
    
}