using AO;
using TinyJson;

/// <summary>
/// Manage all effects added on a player
/// This component is attached to the player and:
/// When requested to handle a effect, fetch player data here and add effect
/// </summary>
public partial class FightPlayerEffectManager : Component
{
    private FightPlayer _player;
    
    public override void Awake()
    {
        _player = Entity.GetComponent<FightPlayer>();
    }

    public override void Start()
    {

    } 

    public bool AddEffect<T>(Player caster, float? duration = null, Action<T> preInit = null) where T : AEffect
    {
        var eff = _player.AddEffect<T>(caster, duration, preInit);
        return eff != null;
    }
    
    public bool RemoveEffect<T>(bool interrupt) where T : AEffect
    {
        return _player.RemoveEffect<T>(interrupt);
    }

    #region Ef: RollOut

    [ClientRpc]
    public void ActivateRollOut(AbilityConfig.RollOutConfig cfg)
    {
        EffectRollOut newRollout = new EffectRollOut();
        newRollout.AssignConfig(cfg, _player);
        AddEffect<EffectRollOut>(_player, cfg.Duration, null);
    }

    #endregion
}