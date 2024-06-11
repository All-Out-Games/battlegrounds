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
    
    // Guideline:
    // 1. If you want the player to add some addon effects to themselves, directly use them in your parent effect
    // for example, the player is invincible during ShoulderCrash, then you just use AddEffect<EffectNoMovementWithInvincibility> in EffectShoulderCrash
    // the abilities are server authoritative in nature so you don't need to worry about the effect not being added on clients
    
    // 2. If the effects are inflicted by other player's abilities, you need to call them in client RPCs. Because the damage is only
    // calculated on server, the debuffs or other effects are server only if you don't broadcast them to clients using RPC.
    // e.g. you want your punch to stun other players, call "CallClient_AddStun" along with the damage, otherwise the player won't receive the stun
    // feed back on client.
    
    // No Movement: Lower the players speed to zero, for example when they are doing an in-place animation
    [ClientRpc]
    public bool AddNoMovement(ulong casterID, float duration)
    {
        var eff = _player.AddEffect<EffectNoMovement>(Entity.FindByNetworkId(casterID).GetComponent<FightPlayer>(), duration < 0 ? null : duration);
        return eff != null;
    }

    [ClientRpc]
    public bool AddStun(ulong casterID, float duration)
    {
        var eff = _player.AddEffect<EffectStun>(Entity.FindByNetworkId(casterID).GetComponent<FightPlayer>(), duration < 0 ? null : duration);
        return eff != null;
    }
}