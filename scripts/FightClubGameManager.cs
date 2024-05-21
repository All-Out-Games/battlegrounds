
using AO;

public class FightClubGameManager : System<FightClubGameManager> {

    #region Attributes

    public static Keybind PunchKeybind;
    public static Keybind Slot1Keybind;
    public static Keybind Slot2Keybind;
    public static Keybind Slot3Keybind;
    public static Keybind Slot4Keybind;
    
    
    protected List<Entity> CombatPlayer = new(); // Only damage players in combat
    protected List<Entity> AFKPlayer = new(); // AFK Area
    protected List<Entity> SafePlayer = new(); // Central

    public List<Entity> GetCombatPlayers()
    {
        return CombatPlayer;
    }

    #endregion
    public override void Awake()
    {
        PunchKeybind = Keybinds.RegisterKeybind("Punch", Input.UnifiedInput.KEYCODE_F);
        Slot1Keybind = Keybinds.RegisterKeybind("Skill 1", Input.UnifiedInput.KEYCODE_R);
        Slot2Keybind = Keybinds.RegisterKeybind("Skill 2", Input.UnifiedInput.KEYCODE_T);
        Slot3Keybind = Keybinds.RegisterKeybind("Skill 3", Input.UnifiedInput.KEYCODE_V);
        Slot4Keybind = Keybinds.RegisterKeybind("Skill 4", Input.UnifiedInput.KEYCODE_B);
    }

    public override void Start() 
    {
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
