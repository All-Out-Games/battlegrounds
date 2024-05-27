using AO;

public partial class FightPlayerEffectManager
{
    
    // All effects related to active skills should go here.
    // Use reflection to call Cast{SkillKey} functions on server (e.g. CallServer_CastPunch)
    // See EquipSkillSlot.cs


    // PART 1: The active effects which can be triggered by the player
    // IMPORTANT: We use reflections to call these RPCs. Make sure the skill handler's name is Cast{SkillKey}.
    // The cast function MUST take a single parameter which is the slot mainKey that's activating the skill.
    // The activate function MUST take 2 parameters which are a customizable struct for the skill & the slot key.
    
    // The slot key handles cooldown stuff, and the struct config will fetch the player's data on the server (and also 
    // check if the player can cast the skill)
    
    #region Ef: Punch

    [ServerRpc]
    public void CastPunch(string slotKey)
    {
        if (Network.IsServer)
        {
            if(!_player.SkillCastGeneralCheck()) return; // General Check
            if (_player.HasEffect<EffectPunch>()) return; // Avoid double cast
            
            // TODO: This can be some get level APIs from the skill tree. For now there's no skill level by design yet
            EffectConfig.PunchConfig cfg = EffectConfig.GetPlayerPunchConfig(1, _player.CurrentAttack); 
            
            CallClient_ActivatePunch(cfg, slotKey);
        }
    }

    [ClientRpc]
    public void ActivatePunch(EffectConfig.PunchConfig cfg, string slotKey)
    {
        // Punch has no cooldown so we don't use the slot key
        // Hint: How to handle cooldown
        // 1. For one-and-done skills, pass the slot key into init function. Call ApplyCooldown() from OnEffectStart
        // 2. For buff skills, pass the slot key into init function. Silent the slot at the start then unsilent
        // from OnEffectEnd, then apply cooldown
        Action<EffectPunch> initPunchWithConfig = (efp) =>
        {
            efp.AssignConfig(cfg);
        };
        
        AddEffect<EffectPunch>(_player, EffectConfig.PunchConfig.PunchAnimationTime, initPunchWithConfig);
    }
    #endregion
    
    
    #region Ef: RollOut

    /// <summary>
    /// Request the server to cast RollOut
    /// </summary>
    /// <param name="level"></param>
    [ServerRpc]
    public void CastRollOut(string slotKey)
    {
        if (Network.IsServer)
        {
            if(!_player.SkillCastGeneralCheck()) return; // General Check
            if (_player.HasEffect<EffectRollOut>()) return; // Avoid double cast
            
            EffectConfig.RollOutConfig cfg = EffectConfig.GetPlayerRollOutConfig(_player.CurrentAttack);
            
            CallClient_ActivateRollOut(cfg, slotKey);
        }
    }
    
    [ClientRpc]
    public void ActivateRollOut(EffectConfig.RollOutConfig cfg, string slotKey)
    {
        // Note: RPC cannot pass class references. 
        // Use player.Entity.NetworkId if we need to pass the caster through server.
        // then Entity.FindByNetworkId() in client.
        Action<EffectRollOut> initRollOutWithConfig = (rollOut) =>
        {
            rollOut.AssignConfig(cfg, slotKey);
        };
        AddEffect<EffectRollOut>(_player, cfg.Duration, initRollOutWithConfig);
    }
    
    #endregion


    #region Ef: ShoulderCrash

    [ServerRpc]
    public void CastShoulderCrash(string slotKey)
    {
        if (Network.IsServer)
        {
            if(!_player.SkillCastGeneralCheck()) return; // General Check
            if (_player.HasEffect<EffectShoulderCrash>()) return; // Avoid double cast
            
            var cfg = EffectConfig.GetPlayerShoulderCrashConfig(_player.CurrentAttack);
            
            CallClient_ActivateShoulderCrash(cfg, slotKey);
        }
    }

    [ClientRpc]
    public void ActivateShoulderCrash(EffectConfig.ShoulderCrashConfig cfg, string slotKey)
    {
        Action<EffectShoulderCrash> initShoulderCrashWithConfig = (sc) =>
        {
            sc.AssignConfig(cfg, slotKey);
        };
        AddEffect<EffectNoMovement>(_player, cfg.DashDuration);
        AddEffect<EffectShoulderCrash>(_player, cfg.DashDuration, initShoulderCrashWithConfig);
    }
    #endregion

    #region Ef: Shield

    [ServerRpc]
    public void CastShield(string slotKey)
    {
        if (Network.IsServer)
        {
            if(!_player.SkillCastGeneralCheck()) return; // General Check
            // Shield can be double cast. The new one will overwrite the old one
            var cfg = EffectConfig.GetPlayerShieldConfig();
            
            CallClient_ActivateShield(cfg, slotKey);
        }
    }

    [ClientRpc]
    public void ActivateShield(EffectConfig.ShieldConfig cfg, string slotKey)
    {
        EffectShield.RemoveShieldEffect(_player); // Overwrite existing shield with new one
        Action<EffectShield> initShieldWithConfig = (shield) =>
        {
            shield.AssignConfig(cfg, slotKey);
        };
        AddEffect(_player, cfg.Duration, initShieldWithConfig);
    }

    #endregion
    // PART 2: Effects inflicted by other players
}