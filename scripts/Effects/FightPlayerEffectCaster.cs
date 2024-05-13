using AO;

public partial class FightPlayerEffectManager
{
    
    // All effects related to active skills should go here.
    // TODO: Use reflection to call Cast{SkillKey} functions on server (e.g. CallServer_CastPunch)
    
    
    // PART 1: The active effects which can be triggered by the player
    // IMPORTANT: We use reflections to call these RPCs. Make sure the skill handler's name is Cast{SkillKey}.
    // The cast function MUST be 0-parameters as we want to read the player's skill data on server.
    // The activate function MUST take a single parameter which is a customizable struct for the skill.
    
    #region Ef: Punch

    [ServerRpc]
    public void CastPunch()
    {
        if (Network.IsServer)
        {
            if (_player.HasEffect<EffectPunch>()) return; // Avoid double cast
            
            EffectConfig.PunchConfig cfg = EffectConfig.GetPlayerPunchConfig(1); // TODO: This need to be some get level APIs from the skill tree
            
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
            if (_player.HasEffect<EffectRollOut>()) return; // Avoid double cast
            
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
    
    
    // PART 2: Effects inflicted by other players
}