
using AO;
using Assembly.scripts.Effects;
using Assembly.scripts.SceneObjects;
using UI = AO.UI;
namespace Assembly.Koh;

/// <summary>
/// Battlegrounds: King of the Hill mode specific logic
/// </summary>
public partial class KohManager : Component
{
    public static KohManager _instance;
    public static KohManager Instance
    {
        get
        {
            if (!_instance.Alive())
            {
                foreach (var c in Scene.Components<KohManager>())
                {
                    _instance = c;
                    _instance.Awaken();
                    break;
                }
            }
            return _instance;
        }
    }

    #region KoH Game SyncVars

    private SyncVar<int> _currentState = new();

    public GameState State
    {
        get => (GameState)_currentState.Value;
        set
        {
            if (Network.IsServer)
            {
                _currentState.Set((int)value);
            }
        }  
    }
    
    private SyncVar<float> _countdown = new();

    public float Countdown
    {
        get => _countdown.Value;
        set
        {
            if (Network.IsServer)
            {
                _countdown.Set(value);
            }
        }
    }

    private SyncVar<bool> _globalAbilityCanUse = new SyncVar<bool>();

    public bool GlobalAbilityCanUse
    {
        get => _globalAbilityCanUse;
        set
        {
            if (Network.IsServer)
            {
                _globalAbilityCanUse.Set(value);
            }
        }
    }

    private SyncVar<int> _roundTimer = new SyncVar<int>();
    public float ServerRoundTimer;
    public int RoundTimer
    {
        get => _roundTimer.Value;
        set
        {
            if (Network.IsServer)
            {
                _roundTimer.Set(value);
            }
        }
    }

    private SyncVar<bool> _roundTimerEnabled = new SyncVar<bool>();

    public bool RoundTimerEnabled
    {
        get => _roundTimerEnabled.Value;
        set
        {
            if (Network.IsServer)
            {
                _roundTimerEnabled.Set(value);
            }
        }
    }
    

    // Classes can be None, Random, Brawler, Ninja, ...
    // Everyone will be -None- when the round starts or when they join in progress
    // - Change from None to a class will be free
    // - Change classes before round starts will also be free
    // - You need to consume one "Book of the Forgotten" to switch class when the round is in progress
    // We need to sell this so we will keep player's selection on the server. They can join another server ofc, but they lose the progress

    public Dictionary<string, string> PlayerSkillPackages = new();

    private bool _countdownPlayed = false;

    public void RequestSkillPackageUpdate()
    {
        // TODO
    }
    
    
    
    #endregion

    #region KoH Scene References

    [Serialized] private Entity _loadoutRollerEntity; // AFK Portal replaced
    [Serialized] private Entity _combatPortalEntity; 
    [Serialized] private Entity _spectralPortalEntity;
    [Serialized] private Entity[] _zonePosition;

    #endregion

    #region KoH Constants

    public const int VignetteLayer = 100;
    public const int RoleNameLayer = 200;
    public const int HotbarLayer = 300;

    #endregion

    #region KoH Variables

    public float LastRoundTimerSyncTime = -1000;
    public float ServerKohPingTimer = 1; // Ping CaptureZone every second on the server
    public KohGlobalData.LastRoundReport RoundReport;
    private List<FightPlayer> _rewardedPlayer = new List<FightPlayer>();
    private FightPlayer _winPlayer;
    private bool _winByScore;

    #endregion
    
    public override void Awake()
    {
        _roundTimer.OnSync += (old, value) =>
        {
            LastRoundTimerSyncTime = Time.TimeSinceStartup;
        };

        // This part shall be used to sync client/server status.
        // If you did something after changing the state on the server that you want the client to do the same
        // Don't use an RPC, use this
        _currentState.OnSync += (old, value) =>
        {
            Log.Warn($"State Changed from {(GameState)old} to {(GameState)value}");
            SetupPortals(value == (int)GameState.Round);
            var players = Scene.Components<FightPlayer>().ToList();
            switch (State)
            {
                case GameState.WaitingForPlayers:
                {
                    break;
                }
                case GameState.CountingDown:
                {
                    break;
                }
                case GameState.StartRound:
                {
                    break;
                }
                case GameState.Round:
                {
                    break;
                }
                case GameState.RoundEnd:
                {
                    foreach (var fp in players)
                    {
                        ResetAfterRound(fp);
                    }
                    break;
                }
            }
        };
        Game.SetMatchmakingPriority(1); // Default prio for a new server (lower than round end, higher than game in progress)
    }
    public override void Update()
    {
        if (Util.OneTime(!_start, ref _start))
        {
            LazyInitialize();
        }
        var players = Scene.Components<FightPlayer>().ToList();
        var playerCount = players.Count;

        #region Server Update

        if (Network.IsServer)
        {
            switch (State)
            {
                case GameState.WaitingForPlayers:
                {
                    if (playerCount >= KohGlobalData.PlayersRequiredToStart)
                    {
                        State = GameState.CountingDown;
                        Countdown = 30f;
                    }
                    break;
                }
                case GameState.CountingDown:
                {
                    if (Countdown > 0f && !Game.LaunchedFromEditor)
                    {
                        if (playerCount < KohGlobalData.PlayersRequiredToStart)
                        {
                            State = GameState.WaitingForPlayers;
                            break;
                        }
                    }

                    var countdownBefore = Countdown;
                    Countdown -= Time.DeltaTime;

                    if (Countdown <= 0f)
                    {
                        State = GameState.StartRound;
                        goto case GameState.StartRound;
                    }
                    
                    break;
                }
                case GameState.StartRound:
                {
                    GlobalAbilityCanUse = true;
                    Countdown = 0;
                    ServerRoundTimer = KohGlobalData.RoundTime;
                    RoundTimer = (int)ServerRoundTimer;
                    State = GameState.Round;
                    RoundTimerEnabled = true;
                    
                    SetupRound();
                    foreach (var fp in players)
                    {
                        fp.KingScore = 0;
                        fp.RoundScore = 0;
                    }
                    _rewardedPlayer.Clear();
                    _winPlayer = null;
                    _winByScore = false;
                    Game.SetMatchmakingPriority(2); // Lower prio when round started
                    break;
                }
                case GameState.Round:
                {
                    if (ServerRoundTimer > 0)
                    {
                        ServerRoundTimer -= Time.DeltaTime;
                        RoundTimer = (int)ServerRoundTimer;
                    }
                    else
                    {
                        ServerRoundTimer = 0;
                        RoundTimer = 0;
                        State = GameState.RoundEnd; // Round End Condition 1 - Time's up
                        if (EffectKing.KingInstance.Alive())
                        {
                            _winPlayer = EffectKing.KingInstance.GetKing();
                        }
                    }

                    #region Zone Update
                    ServerKohPingTimer -= Time.DeltaTime;
                    if (ServerKohPingTimer < 0)
                    {
                        ServerKohPingTimer = 1; // Ping the zone per second to update
                        // CaptureArea Logic
                        var zone = CaptureArea.Instance;
                        var zonePlayers = zone.GetPlayersInside();
                        int zonePlayerCount = zonePlayers.Count;
                        // Log.Warn($"Pinged zone, got {zonePlayers.Count} Players!");
                        
                        // Set zone state
                        if (zonePlayerCount > 1 && zone.ZoneStatus != CaptureArea.CaptureStatus.Contested)
                        {
                            zone.BeforeContestStatus = zone.ZoneStatus;
                            zone.ZoneStatus = CaptureArea.CaptureStatus.Contested;
                            // The following branches have player count 0 or 1
                        }
                        
                        
                        if (zone.ZoneStatus == CaptureArea.CaptureStatus.Contested)
                        {
                            if (zonePlayerCount < 2)
                            {
                                zone.ZoneStatus = zone.BeforeContestStatus; // No longer contested
                            }
                            else
                            {
                                bool ownerInside = false;
                                foreach (var zp in zonePlayers)
                                {
                                    if (zp.UserId == zone.OwnerId)
                                    {
                                        ownerInside = true;
                                    }
                                }

                                if (!ownerInside && zone.BeforeContestStatus == CaptureArea.CaptureStatus.Captured)
                                {
                                    // Owner not inside contested zone -> reduce health
                                    zone.ZoneHealth -= KohGlobalData.CaptureSpeed;
                                    // Neutralized during contest
                                    if (zone.ZoneHealth <= 0)
                                    {
                                        zone.BeforeContestStatus = CaptureArea.CaptureStatus.Neutral;
                                        zone.OwnerId = "Neutral";
                                        zone.OwnerName = "Neutral";
                                        zone.ZoneHealth = KohGlobalData.ZoneMaxHealth / 2;
                                    }
                                }
                            }
                            
                        }

                        if (zone.ZoneStatus == CaptureArea.CaptureStatus.Neutral)
                        {
                            if (zonePlayerCount == 1 && zonePlayers[0].Alive())
                            {
                                zone.ZoneHealth -= KohGlobalData.CaptureSpeed;
                                if (zone.ZoneHealth < 0)
                                {
                                    // Captured from Neutral
                                    zone.ZoneStatus = CaptureArea.CaptureStatus.Captured;
                                    zone.OwnerId = zonePlayers[0].UserId;
                                    zone.OwnerName = zonePlayers[0].Name;
                                    zone.ZoneHealth = KohGlobalData.ZoneMaxHealth / 2;

                                }
                            }
                            else // No Players inside - recover health
                            {
                                zone.ZoneHealth += KohGlobalData.ZoneRecoverSpeed;
                            }
                        }
                        else if(zone.ZoneStatus == CaptureArea.CaptureStatus.Captured)
                        {
                            if (zonePlayerCount == 0)
                            {
                                zone.ZoneHealth -= KohGlobalData.CapturedDecay;
                            }

                            if (zonePlayerCount == 1)
                            {
                                if (zonePlayers[0].UserId == zone.OwnerId)
                                {
                                    zone.ZoneHealth += KohGlobalData.ZoneRecoverSpeed;
                                }
                                else
                                {
                                    zone.ZoneHealth -= KohGlobalData.CaptureSpeed;
                                }
                            }
                            
                            if (zone.ZoneHealth < 0)
                            {
                                // Neutralized zone
                                zone.ZoneStatus = CaptureArea.CaptureStatus.Neutral;
                                zone.OwnerId = "Neutral";
                                zone.OwnerName = "Neutral";
                                zone.ZoneHealth = KohGlobalData.ZoneMaxHealth / 2;
                            }
                        }

                        // Handle King Effect.
                        if (zone.OwnerId != "Neutral") // This covers two cases: Contested with owner & captured
                        {
                            // Grant EffectKing to the player who holds the zone.
                            var king = players.Find(fp => fp.UserId == zone.OwnerId);
                            if (king.Alive())
                            {
                                king.KingScore += 1;
                                king.RoundScore += KohGlobalData.KingScorePerSecond;
                                EffectKing.CallClient_GrantKing(king);
                                if (zonePlayers.Contains(king) && king.CurrentHealth <= king.MaxHealth-KohGlobalData.KingHealInZone)
                                {
                                    king.CurrentHealth += KohGlobalData.KingHealInZone; // Heal king inside the zone
                                }
                            }
                            else
                            {
                                EffectKing.CallClient_GrantKing(null); // This will remove KingEffect from players
                            }
                        }
                        else
                        {
                            EffectKing.CallClient_GrantKing(null);
                        }
                        
                        List<(string, int)> crownTimeSorted = new List<(string, int)>();
                        // Scores (Right side of the screen)
                        foreach (var fp in players)
                        {
                            if (fp.Alive())
                            {
                                crownTimeSorted.Add((fp.Name, fp.KingScore));
                            }
                        }

                        crownTimeSorted.Sort((x, y) => y.Item2.CompareTo(x.Item2)); // Sort descending
                        
                        var topKing = crownTimeSorted[0];
                        
                        // Condition 2 - One player held King Effect for more than 120s
                        if (topKing.Item2 >= 120)
                        {
                            ServerRoundTimer = 0;
                            RoundTimer = 0;
                            State = GameState.RoundEnd;
                            if (EffectKing.KingInstance.Alive())
                            {
                                _winPlayer = EffectKing.KingInstance.GetKing();
                                _winByScore = false;
                            }
                        }
                        
                        if (players.Count == 1)
                        {
                            ServerRoundTimer = 0;
                            RoundTimer = 0;
                            State = GameState.RoundEnd;
                            _winPlayer = players[0];
                        }
                        else if (players.Count == 0)
                        {
                            ServerRoundTimer = 0;
                            RoundTimer = 0;
                            State = GameState.RoundEnd;
                        }

                        
                    }

                    #endregion
                    break;
                }
                case GameState.RoundEnd:
                {
                    GlobalAbilityCanUse = false;
                    
                    List<(string, int)> roundScoreSorted = new List<(string, int)>();
                    foreach (var fp in players)
                    {
                        if (fp.Alive())
                        {
                            roundScoreSorted.Add((fp.Name, fp.RoundScore));
                        }
                    }
                    roundScoreSorted.Sort((x, y) => y.Item2.CompareTo(x.Item2));
                    var topScore = roundScoreSorted[0];
                    
                    string winText = KohGlobalData.ScoreWinText;
                    string winnerName = "None";
                    _winByScore = !_winPlayer.Alive();
                    
                    if (_winByScore)
                    {
                        _winPlayer = players.Find(fpw => fpw.Name == topScore.Item1);
                    }

                    if (_winPlayer.Alive())
                    {
                        winnerName = _winPlayer.Name;
                        _winPlayer.RoundWins += 1;
                    }
                    CallClient_GenerateReport(winnerName, winText);
                    CallClient_AddWinnerZoomIn(_winPlayer, _winByScore);


                    //Zone hubZone = FightClubGameManager.References.CentralHubZone;
                    foreach (var fp in players)
                    {
                        ResetAfterRound(fp);
                        if (fp.PlayerStatus != PlayerStatus.Safe)
                        {
                            fp.SwitchStatus((int)PlayerStatus.Safe);
                        }

                        if (!_rewardedPlayer.Contains(fp))
                        {
                            _rewardedPlayer.Add(fp);
                            //Reward player for performance in this round
                            fp.Coins += KohGlobalData.CalculateCoinReward(fp.KingScore);
                            fp.Exp += KohGlobalData.CalculateExpReward(fp.RoundScore, fp);
                            if (fp == _winPlayer) fp.Gem += KohGlobalData.CalculateGloryReward(fp.KingScore, fp);
                        }
                    }
                    // TODO: Do this after countdown
                    DestroyRound();

                    State = GameState.RoundConclusion;
                    RoundTimerEnabled = false;
                    Countdown = 5f;
                    Game.SetMatchmakingPriority(0); // High prio when the round hasn't started
                    break;
                }
                case GameState.RoundConclusion:
                {
                    Countdown -= Time.DeltaTime;
                    if (Countdown <= 0)
                    {
                        // End of the round routine
                        State = GameState.WaitingForPlayers;
                        
                        // Generate skill packages again
                        PlayerSkillPackages.Clear();
                        foreach (var fp in players)
                        {
                            // Refresh for everyone one the server
                            string skillPkgJson = fp.RefreshSkillPackage();
                            PlayerSkillPackages[fp.Name] = skillPkgJson;
                        }
                    }
                    break;
                }
            }
        }

        #endregion

        #region C/S Update

        // TODO
        // switch (State)
        // {
        //     case GameState.WaitingForPlayers:
        //     {
        //         break;
        //     }
        //     case GameState.CountingDown:
        //     {
        //         break;
        //     }
        //     case GameState.StartRound:
        //     {
        //         break;
        //     }
        //     case GameState.Round:
        //     {
        //         break;
        //     }
        //     case GameState.RoundEnd:
        //     {
        //         break;
        //     }
        // }

        #endregion

        #region Client Only

        var localPlayer = (FightPlayer)Network.LocalPlayer; // Draw UIs here
        if (localPlayer.Alive())
        {
            var timerRect = AO.UI.ScreenRect.CutTop(100).Offset(0, -100);
            var topBarRect = timerRect.BottomRect().GrowBottom(40).Offset(0, 3);
            var midBarRect = AO.UI.ScreenRect.SubRect(0.5f, 0.8f, 0.5f, 0.8f);
            
            var rightBarRect = UI.ScreenRect.CutRight(200).CutTop(360).Offset(0, -400);

            var bottomBarRect = AO.UI.SafeRect.CutBottom(350);
            var bottomBarRect2 = AO.UI.SafeRect.CutBottom(200).SubRect(0.15f, 0f, 0.85f, 1f);
            Texture gamePassIcon = Assets.GetAsset<Texture>("UI/KoH/GamePass.png");

            using var _ = AO.UI.PUSH_LAYER(RoleNameLayer);
            
            switch (State)
            {
                case GameState.WaitingForPlayers:
                {
                    UI.TextAsync(bottomBarRect, $"Waiting for players ({players.Count}/{KohGlobalData.PlayersRequiredToStart})", GetTextSettings(52, 0f, null, UI.HorizontalAlignment.Center));
                    if (RoundReport.HasReport)
                    {
                        BattleReportBtn(rightBarRect);
                    }
                    break;
                }
                case GameState.CountingDown:
                {
                    int secondsLeft = (int)Math.Round(Countdown);
                    UI.TextAsync(bottomBarRect, $"Round starts in {secondsLeft} seconds...", GetTextSettings(42, 0f, null, UI.HorizontalAlignment.Center));
                    if (RoundReport.HasReport)
                    {
                        BattleReportBtn(rightBarRect);
                    }

                    if (Util.OneTime(secondsLeft == 2, ref _countdownPlayed))
                    {
                        SFX.Play(SFXKeys.CountdownAudio, new SFX.PlaySoundDesc());
                    }
                    break;
                }
                case GameState.StartRound:
                {
                    RoundReport.HasReport = false;
                    break;
                }
                case GameState.Round:
                {
                    if (RoundTimerEnabled)
                    {
                        var roundString = $"{(RoundTimer / 60).ToString("D2")}:{(RoundTimer % 60).ToString("D2")}";
                        var textColor = Vector4.White;
                        if (RoundTimer <= 60)
                        {
                            textColor = Vector4.Lerp(Vector4.Red, Vector4.White, Ease.T(Time.TimeSinceStartup - LastRoundTimerSyncTime, 1f));
                        }
                        var ts = GetTextSettingsColor(40, textColor, 0f, null);
                        UI.TextAsync(timerRect, roundString, ts);
                    }

                    List<(string, int)> crownTimeSorted = new List<(string, int)>();
                    List<(string, int)> roundScoreSorted = new List<(string, int)>();
                    // Scores (Right side of the screen)
                    foreach (var fp in players)
                    {
                        if (fp.Alive())
                        {
                            crownTimeSorted.Add((fp.Name, fp.KingScore));
                            roundScoreSorted.Add((fp.Name, fp.RoundScore));
                        }
                    }

                    crownTimeSorted.Sort((x, y) => y.Item2.CompareTo(x.Item2)); // Sort descending
                    roundScoreSorted.Sort((x, y) => y.Item2.CompareTo(x.Item2));
                    
                    var topKing = crownTimeSorted[0];
                    var topScore = roundScoreSorted[0];
                    // 0.2 Current King (72px)
                    Rect curKingRect = rightBarRect.SubRect(0.1f, 0.8f, 1f, 1f);
                    UI.Image(curKingRect, KohGlobalData.BackPlate, Vector4.Black);
                    UI.Image(curKingRect.CutLeft(35).FitAspect(1), KohGlobalData.Crown);
                    curKingRect = curKingRect.CutRight(150).CenterRect();
                    Rect curKingRect1 = curKingRect.GrowTop(36);
                    UI.TextAsync(curKingRect1, "Current King", GetTextSettings(16));
                    Rect curKingRect2 = curKingRect.GrowBottom(36);
                    
                    if (EffectKing.KingInstance.Alive() && EffectKing.KingInstance.Player.Alive())
                    {
                        UI.TextAsync(curKingRect2, EffectKing.KingInstance.Player.Name, GetTextSettings(20));
                    }
                    else
                    {
                        //UI.Image(curKingRect2, null, Vector4.White);
                        UI.TextAsync(curKingRect2, "None", GetTextSettings(20));
                    }
                    // 0.25 Top King Score (80px)
                    Rect topKingRect = rightBarRect.SubRect(0f, 0.55f, 1f, 0.8f);
                    UI.Image(topKingRect,KohGlobalData.BackPlate, Vector4.Black);
                    UI.Image(topKingRect.CutLeft(35).FitAspect(1), KohGlobalData.Crown);
                    topKingRect = topKingRect.CenterRect();
                    // var topKing = kingScores.First();
                    Rect topKingRect1 = topKingRect.GrowTop(40);
                    UI.TextAsync(topKingRect1, topKing.Item1, GetTextSettings(28));
                    Rect topKingRect2 = topKingRect.GrowBottom(40);
                    UI.TextAsync(topKingRect2, topKing.Item2.ToString(), GetTextSettingsColor(28, topKing.Item2 > 100 ? GlobalData.CritNumberColor : Vector4.White));
                    
                    // 0.15 My King Score (64px)
                    Rect myKingRect = rightBarRect.SubRect(0.25f, 0.4f, 1f, 0.55f);
                    UI.Image(myKingRect, KohGlobalData.BackPlate, Vector4.Black);
                    UI.TextAsync(myKingRect.SubRect(0f, 0f, 0.3f, 1f), "You", GetTextSettings(24));
                    UI.TextAsync(myKingRect.SubRect(0.5f, 0f, 1f, 1f), $"{localPlayer.KingScore}", GetTextSettings(24));
                    
                    // 0.25 Top Round Score (80px)
                    // var topScore = roundScores.First();
                    Rect topRoundRect = rightBarRect.SubRect(0f, 0.15f, 1f, 0.4f);
                    UI.Image(topRoundRect, KohGlobalData.BackPlate, Vector4.Black);
                    UI.Image(topRoundRect.CutLeft(35).FitAspect(1), KohGlobalData.Clash);
                    topRoundRect = topRoundRect.CenterRect();
                    Rect topRoundRect1 = topRoundRect.GrowTop(40);
                    UI.TextAsync(topRoundRect1, topScore.Item1, GetTextSettings(24));
                    Rect topRoundRect2 = topRoundRect.GrowBottom(40);
                    UI.TextAsync(topRoundRect2, topScore.Item2.ToString(), GetTextSettings(26));
                    
                    // 0.15 My Round Score (64px)
                    Rect myRoundRect = rightBarRect.SubRect(0.25f, 0f, 1f, 0.15f);
                    UI.Image(myRoundRect, KohGlobalData.BackPlate, Vector4.Black);
                    UI.TextAsync(myRoundRect.SubRect(0f, 0f, 0.3f, 1f), "You", GetTextSettings(20));
                    UI.TextAsync(myRoundRect.SubRect(0.5f, 0f, 1f, 1f), $"{localPlayer.RoundScore}", GetTextSettings(24));
                    
                    // Redeployment timer (if applicable)
                    if (localPlayer.PlayerStatus == PlayerStatus.Safe)
                    {
                        var respBlocker = localPlayer.GetEffect<EffectSafePortalCooldown>();
                        if (respBlocker.Alive())
                        {
                            UI.TextAsync(bottomBarRect, $"Redeployment in {float.Round(respBlocker.DurationRemaining, 1)}s...", GetTextSettings(52));
                            Rect iconRect = bottomBarRect2.CutLeft(200).SubRect(0.25f, 0.25f, 0.75f, 0.75f);
                            if (!KohGlobalData.OwnKoHGamePass(localPlayer))
                            {
                                UI.TextAsync(bottomBarRect2, "Buy the gamepass to reduce your respawn timer by 1s permanently!", GetTextSettings(40));
                            }
                            else
                            {
                                UI.TextAsync(bottomBarRect2, "You have the gamepass! Your redeployment is accelerated.", GetTextSettings(40));
                            }
                            UI.Image(iconRect, gamePassIcon);
                        }

                        
                        PointToWorldPosition(_combatPortalEntity.Position, Vector4.LightGreen);
                    }
                    else if(localPlayer.PlayerStatus == PlayerStatus.Combat && EffectKing.KingInstance.Alive() && localPlayer != EffectKing.KingInstance.Player)
                    {
                        PointToWorldPosition(CaptureArea.Instance.Position, Vector4.Red, true);
                    }
                    
                    break;
                }
                case GameState.RoundEnd:
                {
                    UIManager.Instance.CloseAllUniqueWindow();
                    _countdownPlayed = false;
                    break;
                }
                case GameState.RoundConclusion:
                {
                    if (RoundReport.HasReport)
                    {
                        UI.TextAsync(bottomBarRect, $"{RoundReport.Winner} won the round! Starting the next countdown in {Math.Round(Countdown)}s.", GetTextSettings(52));
                        BattleReportBtn(rightBarRect);
                    }
                    else
                    {
                        UI.TextAsync(bottomBarRect, $"Last round just ended. Next countdown starts in {Math.Round(Countdown)}. ", GetTextSettings(52));
                    }
                    
                    break;
                }
            }
        }
        

        #endregion
    }

    private bool _start = false;
    private void LazyInitialize()
    {
        if (Network.IsServer)
        {
            State = GameState.WaitingForPlayers;

            Log.Info("Game ID: " + Game.GetGameID());
            DestroyRound();
        }
    }

    /// <summary>
    /// [Server Only]
    /// </summary>
    private void SetupRound()
    {
        Vector2 zonePos = _zonePosition.GetRandom(Random.Shared).Position;
        CaptureArea.Instance.CallClient_SetZonePosition(zonePos);
        
        
    }

    /// <summary>
    /// [Server Only] Called when the server first started and when round ends.
    /// </summary>
    private void DestroyRound()
    {
        var zone = CaptureArea.Instance;
        zone.ZoneStatus = CaptureArea.CaptureStatus.Neutral;
        zone.OwnerId = "Neutral";
        zone.OwnerName = "Neutral";
        zone.ZoneHealth = 100;
    }

    private void ResetAfterRound(FightPlayer fp)
    {
        fp.ClearAllEffects();
        fp.ClearSpeedModifier();
        fp.SetAnimTriggerWithReset("RESET", true); // Reset both layers
        fp.GetSkillSlots().KoHEquipClass(-1); // None Class

        // Scores will not reset here but when the round starts
    }

    private void SetupPortals(bool round)
    {
        _combatPortalEntity.LocalEnabled = round;
        _spectralPortalEntity.LocalEnabled = round;
    }

    /// <summary>
    /// Generate round report at the end of the round.
    /// </summary>
    [ClientRpc]
    public void GenerateReport(string winner, string winText)
    {
        var lp = Network.LocalPlayer as FightPlayer;
        if (!lp.Alive())
        {
            RoundReport.HasReport = false;
            return;
        }
        RoundReport.HasReport = true;
        RoundReport.Winner = winner;
        RoundReport.WinText = winText;

        RoundReport.YourCoin = KohGlobalData.CalculateCoinReward(lp.KingScore);
        RoundReport.YourGlory = 0;
        RoundReport.YourExp = KohGlobalData.CalculateExpReward(lp.RoundScore, lp);

        RoundReport.TipIndex = KohGlobalData.Tips.GetRandomIndex(Random.Shared);

        if (winner == lp.Name)
        {
            RoundReport.YourGlory = KohGlobalData.CalculateGloryReward(lp.KingScore, lp); // At most 12 per round
        }
    }

    [ClientRpc]
    public void AddWinnerZoomIn(FightPlayer fp, bool scoreWin)
    {
        fp.AddEffect(null, 5, (EffectWinner w) => w.IsScoreWin = scoreWin);
    }

    private void BattleReportBtn(Rect rBarRect)
    {
        var wd = UIManager.Instance.GetUniqueWindow(UniqueWindowKeys.BattleReportPagePath);
        if (wd != null && wd.IsActive)
        {
            return;
        }
        Rect btnRect = rBarRect.CutTop(72);
        var res = UI.Button(btnRect, "Round Report", new UI.ButtonSettings() {Sprite = KohGlobalData.Ribbon}, GetTextSettings(16));
        if (res.Clicked)
        {
            UIManager.Instance.OpenUniqueUIWindow(UniqueWindowKeys.BattleReportPagePath);
        }
    }


    public UI.TextSettings GetTextSettingsColor(float size, Vector4 textColor, float offset = 0, FontAsset font = null, UI.HorizontalAlignment halign = UI.HorizontalAlignment.Center, UI.VerticalAlignment valign = UI.VerticalAlignment.Center)
    {
        if (font == null)
        {
            font = UI.Fonts.BarlowBold;
        }
        var ts = new UI.TextSettings()
        {
            Font = font,
            Size = size,
            Color = textColor,
            DropShadow = true,
            DropShadowColor = new Vector4(0f, 0f, 0.02f, 0.5f),
            DropShadowOffset = new Vector2(0f, -3f),
            HorizontalAlignment = halign,
            VerticalAlignment = valign,
            WordWrap = false,
            WordWrapOffset = 0,
            Outline = true,
            OutlineThickness = 3,
            Offset = new Vector2(0, offset),
        };
        return ts;
    }
    
    public UI.TextSettings GetTextSettings(float size, float offset = 0, FontAsset font = null, UI.HorizontalAlignment halign = UI.HorizontalAlignment.Center, UI.VerticalAlignment valign = UI.VerticalAlignment.Center)
    {
        if (font == null)
        {
            font = UI.Fonts.BarlowBold;
        }
        var ts = new UI.TextSettings()
        {
            Font = font,
            Size = size,
            Color = Vector4.White,
            DropShadow = true,
            DropShadowColor = new Vector4(0f, 0f, 0.02f, 0.5f),
            DropShadowOffset = new Vector2(0f, -3f),
            HorizontalAlignment = halign,
            VerticalAlignment = valign,
            WordWrap = false,
            WordWrapOffset = 0,
            Outline = true,
            OutlineThickness = 3,
            Offset = new Vector2(0, offset),
        };
        return ts;
    }
    
    public static void PointToWorldPosition(Vector2 position, Vector4 tint, bool hideBelowThreshold = false)
    {
        var worldOffset = (position - Network.LocalPlayer.Position);
        var sellAreaScreenPos = Camera.WorldToScreen(position);
        var playerScreenPos = Camera.WorldToScreen(Network.LocalPlayer.Position + new Vector2(0, 0.5f));
        var dir = (sellAreaScreenPos - playerScreenPos).Normalized;
        var pos = playerScreenPos;
        var distance = worldOffset.Length;
        float arrowSize = 50;
        var anim = (float)Math.Pow(Math.Abs(Math.Sin(Math.PI * Time.TimeSinceStartup)), 0.75);
        float distanceThreshold = 4.5f;
        if (distance >= (distanceThreshold + 0.5f))
        {
            var t = 1 - Ease.T(distance - distanceThreshold, 1);
            var arrowScreenPos = new Rect(pos, pos).Offset(dir.X * 300, dir.Y * 300).Center; // note(josh): using rects to scale by screen size
            arrowScreenPos = Vector2.Lerp(arrowScreenPos, sellAreaScreenPos, t);
            var rect = new Rect(arrowScreenPos, arrowScreenPos).Grow(arrowSize);
            var rotation = Math.Atan2(dir.Y, dir.X) * (180.0 / Math.PI);
            UI.Image(rect, Assets.GetAsset<Texture>("UI/KoH/Arrow_Side.png"), tint, default, (float)rotation);
        }
        else if(!hideBelowThreshold)
        {
            var rect = new Rect(sellAreaScreenPos, sellAreaScreenPos).Grow(arrowSize);
            rect = rect.Offset(0, anim * 50);
            UI.Image(rect, Assets.GetAsset<Texture>("UI/KoH/Arrow_Side.png"), tint, default, 270);
        }
    }
}

public enum GameState
{
    WaitingForPlayers,
    CountingDown,
    StartRound,
    Round,
    RoundEnd,
    RoundConclusion
}
