using AO;
namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityLightFeet : FightAbility
{
    public override string SkillKey => "LightFeet";
    
    public override Type Effect => typeof(EffectLightFeet);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => EffectConfig.LightFeetConfig.Cooldown;
}

public class EffectLightFeet : FightEffect
{
    public override bool IsActiveEffect => false;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.AddSpeedModifier(EffectConfig.LightFeetConfig.SpeedModifier);
        DurationRemaining = EffectConfig.LightFeetConfig.BoostTime;
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.RemoveSpeedModifier(EffectConfig.LightFeetConfig.SpeedModifier);
    }
}