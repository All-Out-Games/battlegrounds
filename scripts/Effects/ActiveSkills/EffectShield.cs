using AO;

/// <summary>
/// Base class of a shield ability.
/// Shield abilities will overwrite each other. Only one of them may exist on a player.
/// </summary>
public class EffectShield : FightEffect
{
    protected EffectConfig.ShieldConfig Config;
    
    EffectShield()
    {
        IsActiveEffect = false;
        BlockAbilityActivation = false;
        IsValidTarget = true;
    }

    public override void OnEffectStart()
    {
        base.OnEffectStart();

        if (Network.IsServer)
        {
            // FightPlayer.CallClient_SetShield(Config.ShieldAmt);
            // FightPlayer.CallClient_SetMaxShield(Config.ShieldAmt);
            FightPlayer.MaxShield = Config.ShieldAmt;
            FightPlayer.CurrentShield = Config.ShieldAmt;
        }
        SkillSlot.SilentSlot(true);
        FightPlayer.ShieldBreakEvent += PrematureBreak;
    }

    public void AssignConfig(EffectConfig.ShieldConfig cfg, string slotKey)
    {
        Config = cfg;
        SlotKey = slotKey;
    }
    
    public override void OnEffectEnd(bool interrupt)
    {
        if (Network.IsServer)
        {
            FightPlayer.CurrentShield = 0;
            FightPlayer.MaxShield = 0;
        }
        SkillSlot.SilentSlot(false);
        SkillSlot.ApplyCooldown(Config.Cooldown);
        FightPlayer.ShieldBreakEvent -= PrematureBreak;
    }

    protected void PrematureBreak()
    {
        Log.Debug("Shield was broken!");
        FightPlayer.GetEffectMgr().RemoveEffect<EffectShield>(true);
    }

    public static void RemoveShieldEffect(FightPlayer fightPlayer)
    {
        EffectShield otherShield;
        fightPlayer.TryGetEffect(out otherShield);
        if (otherShield != null)
        {
            // Overwrite existing effects
            fightPlayer.GetEffectMgr().RemoveEffect<EffectShield>(true);
        }
    }
    public override bool IsActiveEffect { get; }
    public override bool BlockAbilityActivation { get; }
    public override bool IsValidTarget { get; }
}