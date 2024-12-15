using AO;

namespace Assembly.Koh;

/// <summary>
/// Singleton Object
/// </summary>
public partial class CaptureArea : Component
{
    public enum CaptureStatus
    {
        Neutral,
        Contested,
        Captured
    }
    [Serialized] public Vector2 Center;

    private SyncVar<int> _zoneHealth;

    public int ZoneHealth
    {
        get => _zoneHealth.Value;
        set
        {
            if (Network.IsServer)
            {
                _zoneHealth.Set(value);
            }
        }
    }

    private SyncVar<int> _zoneStatus;

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

    public void SetZonePosition(Vector2 pos)
    {
        if (Network.IsServer)
        {
            // Position is synced using the sync_position entity attribute
            Center = pos;
            Entity.Position = pos;
        }
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
        return FightClubGameManager.Instance.OverlapCircleForCombatPlayers(Center, 7, null);
    }

    public override void Update()
    {
        base.Update();
        // All gameplay stuff handled by manager, this loop just draws the world space UI
    }
}