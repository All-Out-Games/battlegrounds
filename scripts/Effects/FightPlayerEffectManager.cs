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
    
    // Put some generic effects here. For effects related to abilities, see Effects/FightPlayerEffectCaster.cs
    
    // No Movement: Lower the players speed to zero, for example when they are doing an in-place animation
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
}