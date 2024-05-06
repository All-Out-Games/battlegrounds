using AO;
using TinyJson;

/// <summary>
/// Manage all effects added on a player
/// This component is attached to the player and:
/// When requested to handle a effect, fetch player data here and add effect
/// </summary>
public partial class FightPlayerEffectManager : FightPlayerComponent
{
    

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

    [ServerRpc]
    public void CastRollOut(int level)
    {
        if (Network.IsServer)
        {
            AbilityConfig.RollOutConfig cfg = AbilityConfig.GetPlayerRollOutConfig(level);

            //AddEffect<EffectRollOut>(_player, cfg.Duration, initRollOutWithConfig);
            CallClient_ActivateRollOut(cfg);
        }
    }
    
    [ClientRpc]
    public void ActivateRollOut(AbilityConfig.RollOutConfig cfg)
    {
        // Note: RPC cannot pass class references. 
        // Use player.Entity.NetworkId if we need to pass the caster through server.
        // then Entity.FindByNetworkId() in client.
        Action<EffectRollOut> initRollOutWithConfig = (rollOut) =>
        {
            rollOut.AssignConfig(cfg);
        };
        AddEffect<EffectRollOut>(_player, cfg.Duration, initRollOutWithConfig);
    }
    
    #endregion

    #region Ef: No Movement

    [ServerRpc]
    public void CastNoMovement(ulong casterId, float duration)
    {
        if (Network.IsServer)
        {
            CallClient_ActivateNoMovement(casterId, duration);
        }
    }
    
    [ClientRpc]
    public void ActivateNoMovement(ulong casterId, float duration)
    {
        // Note: RPC cannot pass class references. 
        // Use player.Entity.NetworkId if we need to pass the caster through server.
        // then Entity.FindByNetworkId() in client.
        FightPlayer caster;
        var casterEntity = Entity.FindByNetworkId(casterId);
        if (casterEntity != null)
        {
            caster = casterEntity.GetComponent<FightPlayer>();
        }
        else
        {
            caster = null;
        }
        AddEffect<EffectNoMovement>(caster, duration);
    }

    #endregion
}