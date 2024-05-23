
using AO;


#region Gameplay Enums

public enum PlayerStatus
{
    Combat,
    Safe,
    AFK,
    Spectating
}

#endregion
public class FightClubGameManager : System<FightClubGameManager> {

    #region Attributes

    public static Keybind PunchKeybind;
    public static Keybind Slot1Keybind;
    public static Keybind Slot2Keybind;
    public static Keybind Slot3Keybind;
    public static Keybind Slot4Keybind;
    public static SceneReferenceHolder References;
    
    
    protected List<Entity> CombatPlayer = new(); // Only damage players in combat
    protected List<Entity> SpectatingPlayer = new(); // TODO: Unused for now
    protected List<Entity> AFKPlayer = new(); // AFK Area players
    protected List<Entity> SafePlayer = new(); // Central Area players

    public Action<FightPlayer> PlayerTeleportEvent;
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
        PlayerTeleportEvent += OnPlayerTeleport;
    }

    public override void Update() 
    {

    }

    public override void Shutdown()
    {
        PlayerTeleportEvent -= OnPlayerTeleport;
    }

    public void OnPlayerJoin(Player player) 
    {
        Log.Info("Player joined!");
        //Save.SetInt(player, "test", Save.GetInt(player, "test", 0) + 1);
        SafePlayer.Add(player.Entity);
        
    }

    public void OnPlayerLeave(FightPlayer player)
    {
        Log.Info("Player left!");
        RemovePlayerFromCurrentZone(player);
    }
    
    public void RemovePlayerFromCurrentZone(FightPlayer fpl)
    {
        if (fpl.PlayerStatus == PlayerStatus.Combat)
        {
            var findIndex = CombatPlayer.FindIndex(entity => entity.Id == fpl.Entity.Id);
            if (findIndex == -1)
            {
                Log.Error($"The ID {fpl.Name} given is not found in the combat player list");
            }
            else
            {
                Log.Debug("Shin: Removed a player from combat player list.");
                CombatPlayer.RemoveAt(findIndex);
            }
        }
        else if (fpl.PlayerStatus == PlayerStatus.Safe)
        {
            var findIndex = SafePlayer.FindIndex(entity => entity.Id == fpl.Entity.Id);
            if (findIndex == -1)
            {
                Log.Error($"The ID {fpl.Name} given is not found in the safe player list");
            }
            else
            {
                SafePlayer.RemoveAt(findIndex);
            }
        }
        else if (fpl.PlayerStatus == PlayerStatus.Spectating)
        {
            var findIndex = SpectatingPlayer.FindIndex(entity => entity.Id == fpl.Entity.Id);
            if (findIndex == -1)
            {
                Log.Error($"The ID {fpl.Name} given is not found in the spectating player list");
            }
            else
            {
                SpectatingPlayer.RemoveAt(findIndex);
            }
        }
        else if (fpl.PlayerStatus == PlayerStatus.AFK)
        {
            var findIndex = AFKPlayer.FindIndex(entity => entity.Id == fpl.Entity.Id);
            if (findIndex == -1)
            {
                Log.Error($"The ID {fpl.Name} given is not found in the afk player list");
            }
            else
            {
                AFKPlayer.RemoveAt(findIndex);
            }
        }
    }

    /// <summary>
    /// If the player teleported, call 'OnPlayerLeave' first to remove them from their current area
    /// Call this to move them to the new space player list
    /// </summary>
    /// <param name="fp"></param>
    public void OnPlayerTeleport(FightPlayer fp)
    {
        switch (fp.PlayerStatus)
        {
             case PlayerStatus.Combat:
                 CombatPlayer.Add(fp.Entity);
                 break;
             case PlayerStatus.Safe:
                 SafePlayer.Add(fp.Entity);
                 break;
             case PlayerStatus.Spectating:
                 SpectatingPlayer.Add(fp.Entity);
                 break;
             case PlayerStatus.AFK:
                 AFKPlayer.Add(fp.Entity);
                 break;
        }
    }
}
