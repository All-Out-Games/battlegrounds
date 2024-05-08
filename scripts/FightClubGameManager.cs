
using AO;

public class FightClubGameManager : System<FightClubGameManager> {

    #region Attributes

    public static Keybind PunchKeybind;
    
    protected List<Entity> CombatPlayer = new(); // Only damage players in combat
    protected List<Entity> SafePlayer = new(); // Not in use now. (waiting for AFK area stuff)

    public List<Entity> GetCombatPlayers()
    {
        return CombatPlayer;
    }

    #endregion
    public override void Awake()
    {
        PunchKeybind = Keybinds.RegisterKeybind("Punch", Input.UnifiedInput.KEYCODE_F);
    }

    public override void Start() 
    {
        Log.Info("Registering player events");
        Player.OnPlayerJoin += OnPlayerJoin;
        Player.OnPlayerLeave += OnPlayerLeave;
        Log.Info($"Current Directory: {System.IO.Directory.GetCurrentDirectory()}");
    }

    public override void Update() 
    {

    }

    public override void Shutdown() 
    {

    }

    public void OnPlayerJoin(Player player) 
    {
        Log.Info("Player joined!");
        //Save.SetInt(player, "test", Save.GetInt(player, "test", 0) + 1);
        CombatPlayer.Add(player.Entity);
        
    }

    public void OnPlayerLeave(Player player)
    {
        Log.Info("Player left!");
        FightPlayer fpl = (FightPlayer)player;
        if (fpl.PlayerStatus == EffectConfig.PlayerStatus.Combat)
        {
            var findIndex = CombatPlayer.FindIndex(entity => entity.Id == fpl.Entity.Id);
            if (findIndex == -1)
            {
                Log.Error($"The ID {findIndex} given is not found in the combat player list");
            }
            else
            {
                Log.Debug("Shin: Removed a player from combat player list.");
                CombatPlayer.RemoveAt(findIndex);
            }
        }
        else if (fpl.PlayerStatus == EffectConfig.PlayerStatus.Safe)
        {
            var findIndex = SafePlayer.FindIndex(entity => entity.Id == fpl.Entity.Id);
            if (findIndex == -1)
            {
                Log.Error($"The ID {findIndex} given is not found in the safe player list");
            }
            else
            {
                SafePlayer.RemoveAt(findIndex);
            }
        
        }
    }
}
