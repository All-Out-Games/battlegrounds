using AO;

public partial class FightPlayerEffectManager
{
    
    // All effects related to active skills should go here.
    // TODO: Use reflection to call Cast{SkillKey} functions on server (e.g. CallServer_CastPunch)
    
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
}