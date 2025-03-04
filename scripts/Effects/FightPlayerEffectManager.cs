using AO;
using Assembly.scripts.Effects;
using Assembly.scripts.Effects.ActiveSkills;


/// <summary>
/// Manage all effects added on a player; Most abilities in FightClub is implemented using AEffect
/// This component is attached to the player and:
/// When requested to handle a effect, fetch player data here and add effect
/// </summary>
public partial class FightPlayerEffectManager : FightPlayerComponent
{
    
    // [Effects] PART 1
    // Wrappers for player API

    public bool  AddEffect<T>(Player caster, float? duration = null, Action<T> preInit = null) where T : AEffect
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
    // Some effects (typically debuffs) inflicted by other players need to be called as ClientRPCs
    
    
    

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

    public void AddBleed(Entity caster, float duration, int dps)
    {
        if (Network.IsServer)
        {
            CallClient_AddBleedInternal(caster, duration, dps);
        }
    }

    public void AddBurn(Entity caster, float duration, int dps)
    {
        if (Network.IsServer)
        {
            CallClient_AddBurnInternal(caster, duration, dps);
        }
    }

    public void AddBattleCryStun(Entity caster, float duration)
    {
        if (Network.IsServer)
        {
            CallClient_AddBattleCryStunInternal(caster, duration);
        }
    }

    public void AddLeapSlamKnockdown(Entity caster, float duration, float getupTime)
    {
        if (Network.IsServer)
        {
            CallClient_AddLeapSlamKnockdownInternal(caster, duration, getupTime);
        }
    }

    public void AddOvershield(Entity caster, float duration, int amt)
    {
        if (Network.IsServer)
        {
            CallClient_AddOverShieldInternal(caster, duration, amt);
        }
    }

    public void AddElectrocute(Entity caster, float duration)
    {
        if (Network.IsServer)
        {
            CallClient_AddElectrocuteInternal(caster, duration);
        }
    }

    public void AddSafePortalCooldown(Entity caster, float duration)
    {
        CallClient_AddSafePortalCooldownInternal(caster, duration);
    }

    public void AddMeteor()
    {
        CallClient_AddMeteorFromPassiveInternal();
    }

    [ClientRpc]
    public void AddNoMovementInternal(Entity caster, float duration)
    {
        _player.AddEffect<EffectNoMovement>(caster.GetComponent<FightPlayer>(), duration);
    }
    
    [ClientRpc]
    public void AddStunInternal(Entity caster, float duration)
    {
        _player.AddEffect<EffectStun>(caster.GetComponent<FightPlayer>(), duration);
    }

    [ClientRpc]
    public void AddBleedInternal(Entity caster, float duration, int dps)
    {
        _player.AddEffect<EffectBleed>(caster.GetComponent<FightPlayer>(), duration,
            bleed => { bleed.PerSecondDmg = dps;});
    }


    [ClientRpc]
    public void AddLeapSlamKnockdownInternal(Entity caster, float duration, float getupTime)
    {
        _player.AddEffect<EffectKnockDown>(caster.GetComponent<FightPlayer>(), duration,
            down => { down.GettingUpTime = getupTime;});
    }

    [ClientRpc]
    public void AddBattleCryStunInternal(Entity caster, float duration)
    {
        _player.AddEffect<EffectBattleCryStun>(caster.GetComponent<FightPlayer>(), duration);
    }

    [ClientRpc]
    public void AddSafePortalCooldownInternal(Entity caster, float duration)
    {
        _player.AddEffect<EffectSafePortalCooldown>(caster.GetComponent<FightPlayer>(), duration);
    }

    [ClientRpc]
    public void AddBurnInternal(Entity caster, float duration, int dps)
    {
        _player.AddEffect<EffectBurn>(caster.GetComponent<FightPlayer>(), duration,
            burn => { burn.PerSecondDmg = dps; });
    }

    [ClientRpc]
    public void AddElectrocuteInternal(Entity caster, float duration)
    {
        _player.AddEffect<EffectElectricShock>(caster.GetComponent<FightPlayer>(), duration);
    }

    [ClientRpc]
    public void AddAdWatchedEffect(string info, float duration)
    {
        _player.AddEffect<EffectAdWatched>(null, duration, watched => watched.Info = info);
    }

    [ClientRpc]
    public void AddOverShieldInternal(Entity caster, float duration, int amt)
    {
        _player.AddEffect<EffectOvershield>(caster.GetComponent<FightPlayer>(), 5f, overshield => overshield.AssignConfig(EffectConfig.ShieldConfig.GetOvershield(amt, duration)));
    }
    
    [ClientRpc]
    public void AddMeteorFromPassiveInternal()
    {
        if (_player.GetSkillTree().GetSkillLevel("MeteorStrike") != 5)
        {
            return;
        }

        Coroutine.Start(Entity, _player.MeteorEntry());
    }
}