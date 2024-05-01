using AO;

public partial class FightPlayer : Player
{
    #region Attributes

    private SyncVar<int> TotalEliminations = new();
    private SyncVar<int> TotalDamageDealt = new();
    
    private int currentHealth = 100;
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

    #endregion

    public override void Awake()
    {
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

    
    
}