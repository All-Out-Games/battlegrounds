namespace Assembly.scripts.Effects.ActiveSkills;
using AO;

public class AbilityRage : FightAbility
{
    public override string SkillKey => "Rage";
    
    public override Type Effect => typeof(EffectRage);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => EffectConfig.RageConfig.Cooldown;
}

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

    public override void NetworkDeserialize(StreamReader reader)
    {
        base.NetworkDeserialize(reader);
        PlayerUI = FightPlayer.GetPlayerUIComp();
        PlayerUI.AddAura("Rage_Aura.prefab", 1.0f);
    }

}