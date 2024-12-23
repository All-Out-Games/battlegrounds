
using AO;
using TinyJson;
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
    private SyncVar<string> _serializedPlayerClass = new SyncVar<string>();
    
    public string SerializedPlayerClass
    {
        get => _serializedPlayerClass.Value;
        set
        {
            if (Network.IsServer)
            {
                _serializedPlayerClass.Set(value);
            }
        }
    }

    public Dictionary<string, string> PlayerClass = new();
    

    public void UpdateClassOnServer()
    {
        // TODO: This will need a server RPC to call (Player has to request it from UI)
        SerializedPlayerClass = PlayerClass.ToJson();
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
                        Countdown = 11f;
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
                    if (Countdown <= 10 && countdownBefore > 10)
                    {
                        Game.SetMatchmakingPriority(1); // Low Prio
                    }
                    if (Countdown <= 0f)
                    {
                        State = GameState.StartRound;
                        goto case GameState.StartRound;
                    }
                    
                    break;
                }
                case GameState.StartRound:
                {
                    Game.SetMatchmakingPriority(1);
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
                            
                            // Grant EffectKing to the player who holds the zone
                            // They will get this effect even if they quit and rejoin (the same server)
                            var king = players.Find(fp => fp.UserId == zone.OwnerId);
                            if (king.Alive())
                            {
                                king.KingScore += 1;
                                king.RoundScore += KohGlobalData.KingScorePerSecond;
                                EffectKing.CallClient_GrantKing(king); 
                            }
                            else
                            {
                                EffectKing.CallClient_GrantKing(null); // This will remove KingEffect from players
                            }
                            
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
                        // TODO: Condition 3 - Only one player left
                        if (players.Count == -1)
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

                    if (_winPlayer.Alive()) winnerName = _winPlayer.Name;
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
                            if (fp == _winPlayer) fp.Gem += KohGlobalData.CalculateGloryReward(fp.KingScore);
                        }
                    }
                    // TODO: Do this after countdown
                    DestroyRound();

                    State = GameState.WaitingForPlayers;
                    RoundTimerEnabled = false;
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
            var midBarRect2 = AO.UI.ScreenRect.SubRect(0.5f, 0.2f, 0.5f, 0.2f);
            var rightBarRect = UI.ScreenRect.CutRight(200).CutTop(360).Offset(0, -400);

            var bottomBarRect = AO.UI.SafeRect.CutBottom(350);

            using var _ = AO.UI.PUSH_LAYER(RoleNameLayer);

            // TODO
            switch (State)
            {
                case GameState.WaitingForPlayers:
                {
                    UI.Text(bottomBarRect, $"Waiting for players ({players.Count}/{KohGlobalData.PlayersRequiredToStart})", GetTextSettings(52, 0f, null, UI.HorizontalAlignment.Center));
                    BattleReportBtn(rightBarRect);
                    break;
                }
                case GameState.CountingDown:
                {
                    UI.Text(bottomBarRect, ("Round starts in " + Math.Round(Countdown)) + " seconds...", GetTextSettings(42, 0f, null, UI.HorizontalAlignment.Center));
                    BattleReportBtn(rightBarRect);
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
                        UI.Text(timerRect, roundString, ts);
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
                    UI.Text(curKingRect1, "Current King", GetTextSettings(16));
                    Rect curKingRect2 = curKingRect.GrowBottom(36);
                    
                    if (EffectKing.KingInstance.Alive() && EffectKing.KingInstance.Player.Alive())
                    {
                        UI.Text(curKingRect2, EffectKing.KingInstance.Player.Name, GetTextSettings(20));
                    }
                    else
                    {
                        //UI.Image(curKingRect2, null, Vector4.White);
                        UI.Text(curKingRect2, "None", GetTextSettings(20));
                    }
                    // 0.25 Top King Score (80px)
                    Rect topKingRect = rightBarRect.SubRect(0f, 0.55f, 1f, 0.8f);
                    UI.Image(topKingRect,KohGlobalData.BackPlate, Vector4.Black);
                    UI.Image(topKingRect.CutLeft(35).FitAspect(1), KohGlobalData.Crown);
                    topKingRect = topKingRect.CenterRect();
                    // var topKing = kingScores.First();
                    Rect topKingRect1 = topKingRect.GrowTop(40);
                    UI.Text(topKingRect1, topKing.Item1, GetTextSettings(28));
                    Rect topKingRect2 = topKingRect.GrowBottom(40);
                    UI.Text(topKingRect2, topKing.Item2.ToString(), GetTextSettingsColor(28, topKing.Item2 > 100 ? GlobalData.CritNumberColor : Vector4.White));
                    
                    // 0.15 My King Score (64px)
                    Rect myKingRect = rightBarRect.SubRect(0.25f, 0.4f, 1f, 0.55f);
                    UI.Image(myKingRect, KohGlobalData.BackPlate, Vector4.Black);
                    UI.Text(myKingRect.SubRect(0f, 0f, 0.3f, 1f), "You", GetTextSettings(24));
                    UI.Text(myKingRect.SubRect(0.5f, 0f, 1f, 1f), $"{localPlayer.KingScore}", GetTextSettings(24));
                    
                    // 0.25 Top Round Score (80px)
                    // var topScore = roundScores.First();
                    Rect topRoundRect = rightBarRect.SubRect(0f, 0.15f, 1f, 0.4f);
                    UI.Image(topRoundRect, KohGlobalData.BackPlate, Vector4.Black);
                    UI.Image(topRoundRect.CutLeft(35).FitAspect(1), KohGlobalData.Clash);
                    topRoundRect = topRoundRect.CenterRect();
                    Rect topRoundRect1 = topRoundRect.GrowTop(40);
                    UI.Text(topRoundRect1, topScore.Item1, GetTextSettings(24));
                    Rect topRoundRect2 = topRoundRect.GrowBottom(40);
                    UI.Text(topRoundRect2, topScore.Item2.ToString(), GetTextSettings(26));
                    
                    // 0.15 My Round Score (64px)
                    Rect myRoundRect = rightBarRect.SubRect(0.25f, 0f, 1f, 0.15f);
                    UI.Image(myRoundRect, KohGlobalData.BackPlate, Vector4.Black);
                    UI.Text(myRoundRect.SubRect(0f, 0f, 0.3f, 1f), "You", GetTextSettings(20));
                    UI.Text(myRoundRect.SubRect(0.5f, 0f, 1f, 1f), $"{localPlayer.RoundScore}", GetTextSettings(24));
                    break;
                }
                case GameState.RoundEnd:
                {
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
            if (Scene.Components<FightPlayer>().ToList().Count >= 2)
            {
                Game.SetMatchmakingPriority(0); // low priority
            }

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
        zone.ZoneHealth = 100;
    }

    private void ResetAfterRound(FightPlayer fp)
    {
        fp.ClearAllEffects();
        fp.ClearSpeedModifier();
        fp.SetAnimTriggerWithReset("RESET", true); // Reset both layers
        
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

        if (winner == lp.Name)
        {
            RoundReport.YourGlory = KohGlobalData.CalculateGloryReward(lp.KingScore); // At most 12 per round
        }
    }

    [ClientRpc]
    public void AddWinnerZoomIn(FightPlayer fp, bool scoreWin)
    {
        fp.AddEffect(null, 5, (EffectWinner w) => w.IsScoreWin = scoreWin);
    }

    private void BattleReportBtn(Rect rBarRect)
    {
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
}

public enum GameState
{
    WaitingForPlayers,
    CountingDown,
    StartRound,
    Round,
    RoundEnd
}
