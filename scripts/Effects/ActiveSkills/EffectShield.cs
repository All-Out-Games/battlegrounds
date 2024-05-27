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
        EffectShield otherShield;
        FightPlayer.TryGetEffect(out otherShield);
        if (otherShield != null)
        {
            // Overwrite existing effects
            FightPlayer.GetEffectMgr().RemoveEffect<EffectShield>(true);
        }
        
        FightPlayer.MaxShield = Config.ShieldAmt;
        FightPlayer.CurrentShield = Config.ShieldAmt;
        SkillSlot.SilentSlot(true);
    }

    public void AssignConfig(EffectConfig.ShieldConfig cfg, string slotKey)
    {
        Config = cfg;
        SlotKey = slotKey;
    }
    
    public override void OnEffectEnd(bool interrupt)
    {
        FightPlayer.CurrentShield = 0;
        FightPlayer.MaxShield = 0;
        SkillSlot.SilentSlot(false);
        SkillSlot.ApplyCooldown(Config.Cooldown);
    }

    public override bool IsActiveEffect { get; }
    public override bool BlockAbilityActivation { get; }
    public override bool IsValidTarget { get; }
}