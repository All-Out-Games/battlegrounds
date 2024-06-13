using AO;
using Assembly.scripts.Effects;


/// <summary>
/// Manage all effects added on a player; Most abilities in FightClub is implemented using AEffect
/// This component is attached to the player and:
/// When requested to handle a effect, fetch player data here and add effect
/// </summary>
public partial class FightPlayerEffectManager : FightPlayerComponent
{
    
    // [Effects] PART 1
    // Wrappers for player API
    // TODO: To be removed
    
    public bool AddEffect<T>(Player caster, float? duration = null, Action<T> preInit = null) where T : AEffect
    {
        var eff = _player.AddEffect<T>(caster, duration, preInit);
        return eff != null;
    }
    
    public bool RemoveEffect<T>(bool interrupt) where T : AEffect
    {
        return _player.RemoveEffect<T>(interrupt);
    }
    
    //[Effects] PART 2
    // Addon effects
    // Some effects inflicted by other players need to be called as ClientRPCs as the damage happens on the server only but the effects need to be on both sides
    
    
    

    /// <summary>
    /// Lower the players speed to zero, for example when they are doing an in-place animation.
    /// They are still able to be bumped.
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="duration"></param>
    public void AddNoMovement(Entity caster, float duration)
    {
        if (Network.IsServer)
        {
            CallClient_AddNoMovementInternal(caster, duration);
        }
    }
    
    /// <summary>
    /// Stun the player. Prevent them from casting ability and move.
    /// </summary>
    /// <param name="caster"></param>
    /// <param name="duration"></param>
    public void AddStun(Entity caster, float duration)
    {
        if (Network.IsServer)
        {
            CallClient_AddStunInternal(caster, duration);
        }
    }

    [ClientRpc]
    public void AddNoMovementInternal(Entity caster, float duration)
    {
        _player.AddEffect<EffectNoMovement>(caster.GetComponent<FightPlayer>(), duration < 0 ? null : duration);
    }
    
    [ClientRpc]
    public void AddStunInternal(Entity caster, float duration)
    {
        _player.AddEffect<EffectStun>(caster.GetComponent<FightPlayer>(), duration < 0 ? null : duration);
    }
}