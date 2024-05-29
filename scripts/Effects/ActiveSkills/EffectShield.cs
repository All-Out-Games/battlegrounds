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
        
        AssignConfig(EffectConfig.GetPlayerShieldConfig());
        DurationRemaining = Config.Duration;
        
        FightPlayer.MaxShield = Config.ShieldAmt;
        FightPlayer.CurrentShield = Config.ShieldAmt;
        FightPlayer.ShieldBreakEvent += PrematureBreak;
    }

    public void AssignConfig(EffectConfig.ShieldConfig cfg)
    {
        Config = cfg;
    }
    
    public override void OnEffectEnd(bool interrupt)
    {
        FightPlayer.CurrentShield = 0;
        FightPlayer.MaxShield = 0;
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