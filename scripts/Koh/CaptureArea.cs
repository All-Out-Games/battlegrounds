using AO;

namespace Assembly.Koh;

/// <summary>
/// Singleton Object
/// </summary>
public partial class CaptureArea : Component
{
    public static Texture HealthBarBack_Neutral = Assets.KeepLoaded<Texture>("UI/Health_Bar/shield_back.png");
    public static Texture HealthBarBack_Captured = Assets.KeepLoaded<Texture>("UI/Health_Bar/health_bar_red/healthbar_back.png");
    public static Texture HealthBarFill_Neutral = Assets.KeepLoaded<Texture>("UI/Health_Bar/shield_fill.png");
    public static Texture HealthBarFill_Captured = Assets.KeepLoaded<Texture>("UI/Health_Bar/health_bar_red/healthbar_fill.png");
    public static Texture Pip_Neutral = Assets.KeepLoaded<Texture>("UI/Health_Bar/shield_pip.png");
    public static Texture Pip_Captured = Assets.KeepLoaded<Texture>("UI/Health_Bar/health_bar_red/healthbar_pip.png");
    
    [Serialized] private Sprite_Renderer _rdr;
    
    public enum CaptureStatus
    {
        Neutral,
        Contested,
        Captured
    }


    public Vector2 Center => Entity.Position;

    private SyncVar<int> _zoneHealth = new SyncVar<int>(100);

    public int ZoneHealth
    {
        get => _zoneHealth.Value;
        set
        {
            if (Network.IsServer)
            {
                _zoneHealth.Set(Int32.Min(KohGlobalData.ZoneMaxHealth, value));
            }
        }
    }

    private SyncVar<int> _zoneStatus = new SyncVar<int>((int)CaptureStatus.Neutral);

    public CaptureStatus ZoneStatus
    {
        get => (CaptureStatus)_zoneStatus.Value;
        set
        {
            if (Network.IsServer)
            {
                _zoneStatus.Set((int)value);
            }
        }
    }

    public CaptureStatus BeforeContestStatus = CaptureStatus.Neutral; // Server Only. Save the status before entering contest

    private SyncVar<string> _ownerId = new SyncVar<string>("Neutral");
    private SyncVar<string> _ownerName = new SyncVar<string>("Neutral");

    public string OwnerId
    {
        get => _ownerId.Value;
        set
        {
            if (Network.IsServer)
            {
                _ownerId.Set(value);
            }
        }
    }

    public string OwnerName
    {
        get => _ownerName.Value;
        set
        {
            if (Network.IsServer)
            {
                _ownerName.Set(value);
            }
        }
    }


    [ClientRpc]
    public void SetZonePosition(Vector2 pos)
    {
        Entity.Position = pos;
    }

    public static CaptureArea _instance;

    public static CaptureArea Instance
    {
        get
        {
            if (!_instance.Alive())
            {
                foreach (var c in Scene.Components<CaptureArea>())
                {
                    _instance = c;
                    _instance.Awaken();
                    break;
                }
            }

            return _instance;
        }
    }

    public List<FightPlayer> GetPlayersInside()
    {
        //Log.Warn($"{Center}");
        return FightClubGameManager.Instance.OverlapCircleForCombatPlayers(Center, 4.5f, null);
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        // All gameplay stuff handled by manager, this loop just draws the world space UI
        if (Network.LocalPlayer.Alive() && KohManager.Instance.State == GameState.Round)
        {
            DrawHealthBar();
        }
    }

    protected Rect DrawHealthBar()
    {
        var targetPlayerScreenPos = Camera.WorldToScreen(Center + new Vector2(0, -5f));
        // var killerPlayerScreenPos = Camera.WorldToScreen(Entity.Position + new Vector2(0, 0.5f));
        var healthRect = new Rect(targetPlayerScreenPos, targetPlayerScreenPos).Grow(20, 90, 0, 70);
        var clashRect = new Rect(targetPlayerScreenPos, targetPlayerScreenPos).Grow(50, 50, 35, 35);
        var statusRect = healthRect.Grow(36, 20, 0, 20);

        var borderRect = healthRect.Grow(5.5f, 4, 5.5f, 4).Offset(0, -2);

        var back = ZoneStatus == CaptureStatus.Captured ? HealthBarBack_Captured : HealthBarBack_Neutral;
        var fill = ZoneStatus == CaptureStatus.Captured ? HealthBarFill_Captured : HealthBarFill_Neutral;
        var pip = ZoneStatus == CaptureStatus.Captured ? Pip_Captured : Pip_Neutral;
        UI.PushScaleFactor(1.25f);
        // Draw bar background
        UI.Image(borderRect, back, Vector4.White, new UI.NineSlice());

        // Draw health percentage
        var healthPercent = ZoneHealth / (float)KohGlobalData.ZoneMaxHealth;
        var healthPercentRect = healthRect.SubRect(0, 0, healthPercent, 1, 0, 0, 0, 0);
        UI.Image(healthPercentRect, fill, Vector4.White, new UI.NineSlice());
        DrawPipOnHealthBar(healthRect, ZoneHealth, KohGlobalData.ZoneMaxHealth, 6, pip, 0.05f, 0.05f, Vector4.White);
        var ts = new UI.TextSettings()
        {
            Font = UI.Fonts.BarlowBold,
            Size = 32,
            VerticalAlignment = UI.VerticalAlignment.Top,
            HorizontalAlignment = UI.HorizontalAlignment.Center,
            Color = Vector4.White,
            Outline = true,
            OutlineColor = Vector4.Black
        };
        string statusTxt = "NEUTRAL";
        if (ZoneStatus == CaptureStatus.Contested)
        {
            statusTxt = "CONTESTED";
            UI.Image(clashRect, KohGlobalData.Clash, Vector4.White);
        }
        if (ZoneStatus == CaptureStatus.Captured) statusTxt = $"King: {OwnerName}";
        UI.TextAsync(statusRect, statusTxt, ts);
        UI.PopScaleFactor();
        return healthRect;
    }
    
    public void DrawPipOnHealthBar(Rect barRect, float currentHealth, float maxHealth, int totalPips, Texture pipTexture, float leftOffset, float rightOffset, Vector4 color, float shrinkFactor = 0.3f)
    {
        float healthPerPip = maxHealth / (float)totalPips;

        float totalOffset = leftOffset + rightOffset;
        float availableWidth = 1.0f - totalOffset;  // Total width available for all pips
        float pipWidth = availableWidth / totalPips; // Width of each pip

        for (int i = 0; i < totalPips; i++)
        {
            // Calculate the threshold for this pip
            float pipThreshold = (i + 1) * healthPerPip;

            float xMin = leftOffset + i * pipWidth; // Start after the left offset for all pips
            float xMax = leftOffset + (i + 1) * pipWidth; // Extend by pipWidth

            // Only draw the pip if the current health is greater than the threshold for this pip
            if (currentHealth >= pipThreshold)
            {
                float newWidth = pipWidth * shrinkFactor;

                // Center the smaller pip rectangle
                float newXMin = xMin + (pipWidth - newWidth) / 2;
                float newXMax = xMax - (pipWidth - newWidth) / 2;

                var smallerPipRect = barRect.SubRect(newXMin, 0, newXMax, 1, 0, 0, 0, 0);
                UI.Image(smallerPipRect, pipTexture, color, new UI.NineSlice());
            }
        }
    }
}