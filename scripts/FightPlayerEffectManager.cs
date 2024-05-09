using AO;

/// <summary>
/// Manage all effects added on a player; Most abilities in FightClub is implemented using AEffect
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

    /// <summary>
    /// Request the server to cast RollOut
    /// </summary>
    /// <param name="level"></param>
    [ServerRpc]
    public void CastRollOut(int level)
    {
        if (Network.IsServer)
        {
            EffectConfig.RollOutConfig cfg = EffectConfig.GetPlayerRollOutConfig(level);

            //AddEffect<EffectRollOut>(_player, cfg.Duration, initRollOutWithConfig);
            CallClient_ActivateRollOut(cfg);
        }
    }
    
    [ClientRpc]
    public void ActivateRollOut(EffectConfig.RollOutConfig cfg)
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

    /// <summary>
    /// Ask the server to cast NoMovement debuff on this player
    /// </summary>
    /// <param name="casterId"></param>
    /// <param name="duration"></param>
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

    #region Ef: Punch

    [ServerRpc]
    public void CastPunch(int level)
    {
        if (Network.IsServer)
        {
            EffectConfig.PunchConfig cfg = EffectConfig.GetPlayerPunchConfig(level);
            
            CallClient_ActivatePunch(cfg);
        }
    }

    [ClientRpc]
    public void ActivatePunch(EffectConfig.PunchConfig cfg)
    {
        Action<EffectPunch> initPunchWithConfig = (efp) =>
        {
            efp.AssignConfig(cfg);
        };
        
        AddEffect<EffectPunch>(_player, EffectConfig.PunchConfig.PunchAnimationTime, initPunchWithConfig);
        AddEffect<EffectNoMovement>(_player, EffectConfig.PunchConfig.PunchAnimationTime);
    }
    #endregion
}