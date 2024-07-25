using AO;
using Assembly.scripts.VFX;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityShadowStep : FightAbility
{
    public override string SkillKey => "ShadowStep";
    
    public override Type Effect => typeof(EffectShadowStep);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => EffectConfig.ShadowStepConfig.Cooldown;
}

public class EffectShadowStep : FightEffect
{
    public override bool IsActiveEffect => true;

    public override bool BlockAbilityActivation => true;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        var fx = VFXPrefabs.ShadowStepVFX.Instantiate();
        fx.Position = FightPlayer.Entity.Position;
        fx.GetComponent<SelectionVFX>()?.StartVFX("shadow_step_effect", false);
        
        DurationRemaining = 0.1f;
        if (Network.IsServer)
        {
            Vector2 dir = FightPlayer.Velocity.Length < 0.1f ? FightPlayer.GetFacingDirectionAsVector() : FightPlayer.Velocity.Normalized;
            FightPlayer.Teleport(FightPlayer.Entity.Position + dir * EffectConfig.ShadowStepConfig.MovementDistance);
        }
        
    }
}