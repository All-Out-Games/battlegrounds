
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
    
    public static Keybind Slot1Keybind;
    public static Keybind Slot2Keybind;
    public static Keybind Slot3Keybind;
    public static Keybind Slot4Keybind;
    public static Keybind Slot5Keybind;
    public static Keybind Slot6Keybind;
    public static SceneReferenceHolder References;
    
    
    protected List<Entity> CombatPlayer = new(); // Only damage players in combat
    protected List<Entity> SpectatingPlayer = new(); // TODO: Unused for now
    protected List<Entity> AFKPlayer = new(); // AFK Area players
    protected List<Entity> SafePlayer = new(); // Central Area players

    // Server Only Event. Client Related events should go in FightPlayerEvents, and be sent to the player client.
    public Action<FightPlayer> PlayerTeleportEvent;
    public Action<FightPlayer, FightPlayer> PlayerEliminationEvent;
    public Action<FightPlayer, FightPlayer, int> PlayerDamageEvent;
    public List<Entity> GetCombatPlayers()
    {
        return CombatPlayer;
    }

    #endregion
    

    public override void Awake()
    {
        // Slot1Keybind = Keybinds.RegisterKeybind("Ability 1", Input.UnifiedInput.KEYCODE_F);
        // Slot2Keybind = Keybinds.RegisterKeybind("Ability 2", Input.UnifiedInput.KEYCODE_R);
        // Slot3Keybind = Keybinds.RegisterKeybind("Ability 3", Input.UnifiedInput.KEYCODE_T);
        // Slot4Keybind = Keybinds.RegisterKeybind("Ability 4", Input.UnifiedInput.KEYCODE_V);
        // Slot5Keybind = Keybinds.RegisterKeybind("Ability 5", Input.UnifiedInput.KEYCODE_B);
        // Slot6Keybind = Keybinds.RegisterKeybind("Ability 6", Input.UnifiedInput.KEYCODE_G);
        Chat.RegisterChatCommandHandler(RunChatCommand);
    }

    public override void Start()
    {
        PlayerTeleportEvent += OnPlayerTeleport;
        PlayerEliminationEvent += OnPlayerElimination;
        PlayerDamageEvent += OnPlayerDamage;
    }

    public override void Update() 
    {

    }

    public override void Shutdown()
    {
        PlayerTeleportEvent -= OnPlayerTeleport;
        PlayerEliminationEvent -= OnPlayerElimination;
        PlayerDamageEvent -= OnPlayerDamage;
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

    #region Server Events

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
    public void OnPlayerElimination(FightPlayer killer, FightPlayer victim)
    {
        UIManager.CallClient_SetGlobalPopup($"{killer.Name} killed {victim.Name}!", 2.5f);
    }

    public void OnPlayerDamage(FightPlayer killer, FightPlayer victim, int amount)
    {
        Log.Debug($"{killer.Name} damaged {victim.Name} by {amount}");
    }
    
    #endregion

    #region Chat Command

    public bool CheckAdmin(Player player)
    {
        if (!player.IsAdmin)
        {
            Chat.SendMessage(player, "You must be an admin to use this command.");
            return false;
        }
        return true;
    }
    
    public void RunChatCommand(Player p, string command)
    {
        // TODO
        var parts = command.ToLowerInvariant().Split(' ');
        var cmd = parts[0];
        FightPlayer player = (FightPlayer)p;
        switch (cmd)
        {
            case "grant":
                // Add currency
                if (!CheckAdmin(p)) return;
                if (parts.Length < 3)
                {
                    Chat.SendMessage(player, "Usage: /grant <player> <item> [amount]");
                    return;
                }

                var target = Player.AllPlayers.FirstOrDefault(p => p.Name.ToLowerInvariant() == parts[1]);
                if (parts[1] == "self" || parts[1] == "me")
                {
                    target = player;
                }
                if (target == null)
                {
                    Chat.SendMessage(player, $"Grant failed, player {parts[1]} not found.");
                    return;
                }

                switch (parts[2])
                {
                    case "coins":
                    {
                        var amount = 1;
                        if (parts.Length >= 3)
                        {
                            int.TryParse(parts[3], out amount);
                        }

                        var fightTarget = (FightPlayer)target;
                        //fatTarget.Coins += amount;
                        return;
                    }
                }

                break;
            case "giveability":
                // Unlock ability by {skillkey}
                break;
            case "giveabilityforced":
                // Force unlock
                break;
            case "depriveability":
                // Remove a skill from player
                break;
        }
    }

    #endregion
}
