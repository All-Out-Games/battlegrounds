namespace Assembly.scripts.Effects.ActiveSkills;

public class EffectBattleCry : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => true;

    protected EffectConfig.BattleCryConfig Config;
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        AssignConfig(EffectConfig.BattleCryConfig.GetDefault(FightPlayer.CurrentAttack));

        //Stomp();
    }
    
    public override void OnEffectEnd(bool interrupt)
    {
        
    }

    public void AssignConfig(EffectConfig.BattleCryConfig cfg)
    {
        Config = cfg;
    }
}