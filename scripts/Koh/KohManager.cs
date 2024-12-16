
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

    // Save player scores, classes and king-time on the server as dict, then jsonize them as sync var
    // These dicts are destroyed per round. When a player joins, their entries will be created, but it will not be destroyed when they leave (in case they want to join back)
    private SyncVar<string> _serializedPlayerScore = new SyncVar<string>();

    public string SerializedPlayerScore
    {
        get => _serializedPlayerScore.Value;
        set
        {
            if (Network.IsServer)
            {
                _serializedPlayerScore.Set(value);
            }
        }
    }
    public Dictionary<string, int> PlayerScore = new();

    public void UpdatePlayerScoreOnServer()
    {
        SerializedPlayerScore = PlayerScore.ToJson();
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

    public void UpdatePlayerClassOnServer()
    {
        // TODO: This will need a server RPC to call (Player has to request it from UI)
        SerializedPlayerClass = PlayerClass.ToJson();
    }
    
    private SyncVar<string> _serializedPlayerKingScore = new SyncVar<string>();

    public string SerializedPlayerKingScore
    {
        get => _serializedPlayerKingScore.Value;
        set
        {
            if (Network.IsServer)
            {
                _serializedPlayerKingScore.Set(value);
            }
        }
    }

    public Dictionary<string, int> PlayerKingScore = new();

    public void UpdatePlayerKingScoreOnServer()
    {
        SerializedPlayerKingScore = PlayerKingScore.ToJson();
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
                        Countdown = 14f;
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
                    }
                    
                    // TODO: Condition 2 - One player held King Effect for more than 120s
                    // TODO: Condition 3 - Only one player left
                    
                    // Capture stuff
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

                        if (zone.ZoneStatus == CaptureArea.CaptureStatus.Contested && zonePlayerCount < 2)
                        {
                            zone.ZoneStatus = zone.BeforeContestStatus;
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
                                    if (zone.ZoneHealth < 0)
                                    {
                                        // Neutralized zone
                                        zone.ZoneStatus = CaptureArea.CaptureStatus.Neutral;
                                        zone.OwnerId = "Neutral";
                                        zone.OwnerName = "Neutral";
                                        zone.ZoneHealth = KohGlobalData.ZoneMaxHealth / 2;
                                    }
                                }
                            }
                            
                            // Grant EffectKing to the player who holds the zone
                            var king = players.Find(fp => fp.UserId == zone.OwnerId);
                            EffectKing.CallClient_GrantKing(king);

                        }
                    }
                    break;
                }
                case GameState.RoundEnd:
                {
                    GlobalAbilityCanUse = false;
                    
                    //Zone hubZone = FightClubGameManager.References.CentralHubZone;
                    foreach (var fp in players)
                    {
                        ResetAfterRound(fp);
                        fp.SwitchStatus((int)PlayerStatus.Safe);
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
                break;
            }
        }

        #endregion

        #region Client Only

        var localPlayer = (FightPlayer)Network.LocalPlayer; // Draw UIs here
        if (localPlayer.Alive())
        {
            var timerRect = AO.UI.ScreenRect.CutTop(100).Offset(0, -100);
            var topBarRect = timerRect.BottomRect().GrowBottom(40).Offset(0, 3);
            var midBarRect = AO.UI.ScreenRect.SubRect(0.5f, 0.8f, 0.5f, 0.8f);
            var midBarRect2 = AO.UI.ScreenRect.SubRect(0.5f, 0.2f, 0.5f, 0.2f);

            var redScoreRect = AO.UI.ScreenRect.CutTop(100).Offset(-200, 0);
            var blueScoreRect = AO.UI.ScreenRect.CutTop(100).Offset(200, 0);

            var bottomBarRect = AO.UI.SafeRect.CutBottom(350);

            using var _ = AO.UI.PUSH_LAYER(RoleNameLayer);

            // TODO
            switch (State)
            {
                case GameState.WaitingForPlayers:
                {
                    UI.Text(bottomBarRect, $"Waiting for players ({players.Count}/{KohGlobalData.PlayersRequiredToStart})", GetTextSettings(52, 0f, null, UI.HorizontalAlignment.Center));
                    break;
                }
                case GameState.CountingDown:
                {
                    UI.Text(bottomBarRect, ("Round starts in " + Math.Round(Countdown)) + " seconds...", GetTextSettings(42, 0f, null, UI.HorizontalAlignment.Center));
                    break;
                }
                case GameState.StartRound:
                {
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
                    // Scores
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
    }

    private void SetupPortals(bool round)
    {
        _combatPortalEntity.LocalEnabled = round;
        _spectralPortalEntity.LocalEnabled = round;
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
