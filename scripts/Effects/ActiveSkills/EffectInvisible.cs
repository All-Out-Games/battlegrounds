using AO;
namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityInvisible : FightAbility
{
    public override string SkillKey => "Hypnotize";

    public override Type Effect => typeof(EffectInvisible);
    public override bool MonitorEffectDuration => true;
    
    public override TargettingMode TargettingMode => TargettingMode.Self;
    public override float Cooldown => EffectConfig.InvisibilityConfig.Cooldown;
}

public class EffectInvisible : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool IsValidTarget => false;
    protected override int InterruptLevel => 1; // Interrupted by any damage or skill activation

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        FightPlayer.AddInvisibilityReason("InvisSkill");
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.RemoveInvisibilityReason("InvisSkill");
    }
    
    
}