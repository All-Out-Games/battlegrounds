namespace Assembly.scripts.Effects.ActiveSkills;

public class EffectRage : FightEffect
{

    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => false;
    public override bool IsValidTarget => true;

    // General Atk boost buff
    protected EffectConfig.RageConfig Config;

    protected FightPlayerUI PlayerUI;
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        PlayerUI = FightPlayer.GetPlayerUIComp();
        AssignConfig(EffectConfig.RageConfig.GetDefault());
        
        
        FightPlayer.CurrentAttack += Config.AtkBoost;
        DurationRemaining = Config.Duration;
        
        PlayerUI.AddAura("Rage_Aura.prefab", 1.0f);
        
    }

    public override void OnEffectEnd(bool interrupt)
    {
        FightPlayer.CurrentAttack -= Config.AtkBoost;
        PlayerUI.RemoveAura("Rage_Aura.prefab");
    }

    protected void AssignConfig(EffectConfig.RageConfig cfg)
    {
        Config = cfg;
    }
    
}