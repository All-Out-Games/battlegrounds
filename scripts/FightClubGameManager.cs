
using AO;

public class FightClubGameManager : System<FightClubGameManager> {

    #region Attributes
    
    public static SceneReferenceHolder References;

    // Server Only Event. Client Related events should go in FightPlayerEvents, and be sent to the player client.
    public Action<FightPlayer> PlayerTeleportEvent;
    public Action<FightPlayer, FightPlayer> PlayerEliminationEvent;
    public Action<FightPlayer, FightPlayer, FightPlayer.DamageInfo> PlayerDamageEvent;

    #endregion
    

    public override void Awake()
    {
        Chat.RegisterChatCommandHandler(RunChatCommand);
    }

    public override void Start()
    {
        PlayerTeleportEvent += OnPlayerTeleport;
        PlayerEliminationEvent += OnPlayerElimination;
        //PlayerDamageEvent += OnPlayerDamage;
    }

    public override void Update() 
    {

    }

    public override void Shutdown()
    {
        PlayerTeleportEvent -= OnPlayerTeleport;
        PlayerEliminationEvent -= OnPlayerElimination;
        //PlayerDamageEvent -= OnPlayerDamage;
    }

    public void OnPlayerJoin(Player player) 
    {
        Log.Info("Player joined!");
    }

    public void OnPlayerLeave(FightPlayer player)
    {
        Log.Info("Player left!");
    }

    #region Server Events

    /// <summary>
    /// If the player teleported, call 'OnPlayerLeave' first to remove them from their current area
    /// Call this to move them to the new space player list
    /// </summary>
    /// <param name="fp"></param>
    public void OnPlayerTeleport(FightPlayer fp)
    {
        
    }
    public void OnPlayerElimination(FightPlayer killer, FightPlayer victim)
    {
        UIManager.CallClient_SetGlobalPopup($"{killer.Name} killed {victim.Name}!", 2.5f);
    }

    public void OnPlayerDamage(FightPlayer killer, FightPlayer victim, FightPlayer.DamageInfo info)
    {
        Log.Debug($"{killer.Name} damaged {victim.Name} by {info.ReactionInfo.Amount}");
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
        var parts = command.Split(' ');
        var cmd = parts[0].ToLowerInvariant();
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

                var target = Player.AllPlayers.FirstOrDefault(p => p.Name == parts[1]);
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
                        fightTarget.Coins += amount;
                        Chat.SendMessage(target, $"Coins Given = {amount}");
                        return;
                    }
                    default:
                        Chat.SendMessage(player, $"The item {parts[2]} is not found to be granted");
                        break;
                }

                break;
            case "requestability":
                // Unlock ability by {skillkey}
                if (parts.Length != 2)
                {
                    Chat.SendMessage(player, "Usage: /requestability <SkillKey>");
                    return;
                }
                string key = parts[1];
                player.GetSkillTree().RequestUpgradeSkill(key);
                break;
            case "depriveability":
                // Remove a skill from player
                if (!CheckAdmin(p)) return;
                if (parts.Length != 2)
                {
                    Chat.SendMessage(player, "Usage: /depriveability <SkillKey>");
                    return;
                }
                key = parts[1];
                player.GetSkillTree().DepriveSkill(key);
                break;
            case "serverspawn":
                // Spawn something at player position
                if (!CheckAdmin(p)) return;
                if (parts.Length != 2)
                {
                    Chat.SendMessage(player, "Usage: /serverspawn <PrefabPath.prefab>");
                    return;
                }
                key = parts[1];
                ServerSpawn(key, player.Entity.Position);
                break;
        }
    }

    #endregion

    #region Utils

    public List<FightPlayer> GetCombatPlayers()
    {
        List<FightPlayer> fightPlayers = new List<FightPlayer>();
        foreach (var p in Player.AllPlayers)
        {
            if(p is FightPlayer { PlayerStatus: PlayerStatus.Combat } fp) fightPlayers.Add(fp);
        }
        return fightPlayers;
    }

    // Difference:
    // Collision Entity we created on the player is considerably larger, they should be used for ray-cast based abilities to make them easier to hit
    // For AoE's just use the player's default collider.
    
    public Entity[] GetCombatPlayersAsEntities()
    {
        return GetCombatPlayers().Select(item => item.Entity).ToArray();
    }

    public Entity[] GetCombatPlayersCollisionEntities()
    {
        return GetCombatPlayers().Select(item => item.CollisionEntity).ToArray();
    }

    public List<FightPlayer> OverlapCircleForCombatPlayers(Vector2 center, float radius)
    {
        List<FightPlayer> hitPlayers = new List<FightPlayer>();
        foreach (var other in GetCombatPlayers())
        {
            if (Vector2.Distance(center, other.Entity.Position) < radius)
            {
                if (other != null)
                {
                    hitPlayers.Add(other);
                }
            }
        }
        return hitPlayers;
    }

    public void ServerSpawn(string prefabPath, Vector2 position, Action<Entity> afterSpawn = null)
    {
        if (Network.IsServer)
        {
            Prefab pf = Assets.GetAsset<Prefab>(prefabPath);
            if (pf == null)
            {
                Log.Error($"{prefabPath} does not exist!");
                return;
            }
            Entity expEntity = pf.Instantiate();
            expEntity.Position = position;
            Network.Spawn(expEntity);
            if (afterSpawn != null)
            {
                afterSpawn(expEntity);
            }
        }
    }

    #endregion
}
