
using AO;
using Assembly.scripts.UI;

public class FightClubGameManager : System<FightClubGameManager> {

    #region Attributes
    
    public static SceneReferenceHolder References;
    public static int DamageNumberLayer = 5;

    // Server Only Event. Client Related events should go in FightPlayerEvents, and be sent to the player client.
    public Action<FightPlayer, FightPlayer, FightPlayer.DamageInfo> PlayerEliminationEvent;
    public Action<FightPlayer, FightPlayer, FightPlayer.DamageInfo> PlayerDamageEvent;
    

    public Action<FightPlayer> PlayerLeaveEvent;

    public Action AfkTick;
    private float _afkLastTickElapsed;
    #endregion
    

    public override void Awake()
    {
        Chat.RegisterChatCommandHandler(RunChatCommand);
        Analytics.EnableAutomaticAnalytics("d19a5e187e989175aa53e371525016b3", "aeefb0db200260a7863498e8a67d25e2a41795e0");
    }

    public override void Start()
    {
        PlayerEliminationEvent += OnPlayerElimination;
        
        Leaderboard.RegisterSortCallback((Player[] players) =>
        {
            Array.Sort(players, (a, b) =>
            {
                return ((FightPlayer)b).TotalEliminations.CompareTo(((FightPlayer)a).TotalEliminations);
            });
        });
        
        Leaderboard.Register("Kills", (Player[] players, string[] scores) =>
        {
            for (int i = 0; i < players.Length; i++)
            {
                var player = (FightPlayer)players[i];
                scores[i] = $"{player.TotalEliminations:N0}";
            }
        });
        
        Leaderboard.Register("Level", (Player[] players, string[] scores) =>
        {
            for (int i = 0; i < players.Length; i++)
            {
                var player = (FightPlayer)players[i];
                scores[i] = $"{player.Level+1}";
            }
        });

        Game.SetVoiceEnabled(true);
    }

    public override void Update()
    {
        _afkLastTickElapsed += Time.DeltaTime;
        if (_afkLastTickElapsed > 60)
        {
            _afkLastTickElapsed = 0;
            AfkTick?.Invoke();
        }
    }

    public override void Shutdown()
    {
        PlayerEliminationEvent -= OnPlayerElimination;
    }

    public void OnPlayerJoin(Player player) 
    {
        Log.Info("Player joined!");
    }

    public void OnPlayerLeave(FightPlayer player)
    {
        PlayerLeaveEvent?.Invoke(player);
    }

    #region Server Events
    
    public void OnPlayerElimination(FightPlayer killer, FightPlayer victim, FightPlayer.DamageInfo info)
    {
        //UIManager.CallClient_SetStatusPopup($"{killer.Name} killed {victim.Name}!", 2.5f, PlayerStatus.Combat);
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
                        var amount = 1000;
                        if (parts.Length >= 3)
                        {
                            int.TryParse(parts[3], out amount);
                        }

                        var fightTarget = (FightPlayer)target;
                        fightTarget.Coins += amount;
                        Chat.SendMessage(target, $"Coins Given = {amount}");
                        return;
                    }
                    case "exp":
                    {
                        var amount = 1000;
                        if (parts.Length >= 3)
                        {
                            int.TryParse(parts[3], out amount);
                        }

                        var fightTarget = (FightPlayer)target;
                        fightTarget.Exp += amount;
                        Chat.SendMessage(target, $"Exp Given = {amount}");
                        return;
                    }
                    case "gem":
                    {
                        var amount = 500;
                        if (parts.Length >= 3)
                        {
                            int.TryParse(parts[3], out amount);
                        }
                        var fightTarget = (FightPlayer)target;
                        fightTarget.Gem += amount;
                        Chat.SendMessage(target, $"Gem Given = {amount}");
                        return;
                    }
                    case "expbooster":
                    {
                        var amount = 5;
                        if (parts.Length >= 3)
                        {
                            int.TryParse(parts[3], out amount);
                        }
                        var fightTarget = (FightPlayer)target;
                        fightTarget.AddExpBoostTime(amount);
                        Chat.SendMessage(target, $"Booster Given = {amount}");
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
            case "resetplayer":
                if (!CheckAdmin(p)) return;
                if (parts.Length != 1)
                {
                    Chat.SendMessage(player, "Usage: /resetplayer");
                    return;
                }
                player.Rebirth();
                break;
            case "fetchstat":
                //if (!CheckAdmin(p)) return;
                if (parts.Length != 2)
                {
                    Chat.SendMessage(player, "Usage: /fetchstat <playerid>");
                    return;
                }
                target = Player.AllPlayers.FirstOrDefault(p => p.Name == parts[1]);
                if (parts[1] == "self" || parts[1] == "me")
                {
                    target = player;
                }
                FightPlayer fp = target as FightPlayer;
                if (fp == null)
                {
                    Chat.SendMessage(player, $"Fetchstat failed, player {parts[1]} not found.");
                    return;
                }
                
                Chat.SendMessage(player, $"Player {target.Name}: PunchLevel = {fp.PunchLevel}, " +
                                         $"PunchDmg = {EffectConfig.GetPlayerPunchConfig(fp.PunchLevel, fp.CurrentAttack).PunchDamage}");
                Chat.SendMessage(player, $"atk = {fp.CurrentAttack}, mhp = {fp.MaxHealth}, spd = {fp.CombatSpeedPercentage}");
                break;
        }
    }

    #endregion

    #region Utils

    public List<FightPlayer> GetCombatPlayers(Player exclude)
    {
        List<FightPlayer> fightPlayers = new List<FightPlayer>();
        foreach (var p in Player.AllPlayers)
        {
            if (p == exclude) continue;
            if(p is FightPlayer { PlayerStatus: PlayerStatus.Combat } fp) fightPlayers.Add(fp);
        }
        return fightPlayers;
    }

    // Difference:
    // Collision Entity we created on the player is considerably larger, they should be used for ray-cast based abilities to make them easier to hit
    // For AoE's just use the player's default collider.
    
    public Entity[] GetCombatPlayersAsEntities(Player exclude)
    {
        return GetCombatPlayers(exclude).Select(item => item.Entity).ToArray();
    }

    public Entity[] GetCombatPlayersCollisionEntities(Player exclude)
    {
        return GetCombatPlayers(exclude).Select(item => item.CollisionEntity).ToArray();
    }

    public List<FightPlayer> OverlapCircleForCombatPlayers(Vector2 center, float radius, Player exclude)
    {
        List<FightPlayer> hitPlayers = new List<FightPlayer>();
        foreach (var other in GetCombatPlayers(exclude))
        {
            if (Vector2.Distance(center, other.Entity.Position) < radius)
            {
                hitPlayers.Add(other);
            }
        }
        return hitPlayers;
    }

    /// <summary>
    /// Spawns a networked prefab.
    /// </summary>
    /// <param name="prefabPath"></param>
    /// <param name="position"></param>
    /// <param name="afterSpawn"></param>
    public void ServerSpawn(string prefabPath, Vector2 position, Action<Entity> afterSpawn = null)
    {
        Log.Debug($"Network Spawn called for: {prefabPath} on client.");
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

    public void ClientSpawn(string prefabPath, Vector2 position, Action<Entity> afterSpawn = null)
    {
        if(Network.IsServer) return;
        Prefab pf = Assets.GetAsset<Prefab>(prefabPath);
        if (pf == null)
        {
            Log.Error($"{prefabPath} does not exist!");
            return;
        }
        Entity expEntity = pf.Instantiate();
        expEntity.Position = position;
        if (afterSpawn != null)
        {
            afterSpawn(expEntity);
        }
    }
    
    public void ClientSpawn(Prefab prefab, Vector2 position, Action<Entity> afterSpawn = null)
    {
        if(Network.IsServer) return;
        Entity expEntity = prefab.Instantiate();
        expEntity.Position = position;
        if (afterSpawn != null)
        {
            afterSpawn(expEntity);
        }
    }
    
    public List<DamageNumbers> ActiveDamageNumbers = new();
    
    public void SpawnDamageNumber(Vector2 worldPosition, Vector4 color, string text)
    {
        var searchResult = new DamageNumbers();
        searchResult.Text = text;
        searchResult.Position = worldPosition;
        searchResult.Color = color;
        searchResult.T = 0;
        ActiveDamageNumbers.Add(searchResult);
    }

    #endregion
}
