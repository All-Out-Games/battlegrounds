using AO;
namespace Assembly.scripts.Effects.ActiveSkills;


public class AbilityPsionicBeam : FightAbility
{
    public override Type Effect => typeof(EffectPsionicBeam);
    public override string SkillKey => "PsionicBeam";
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => EffectConfig.PsionicBeamConfig.MaximumRange;

    public override float Cooldown => EffectConfig.PsionicBeamConfig.Cooldown;
}

public class EffectPsionicBeam : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;

    private List<Entity> _interactedEntity;

    private float _angleLow;
    private float _angleHigh;
    private Vector2 _eyePos;

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        DurationRemaining = EffectConfig.PsionicBeamConfig.CarveTime;
        _interactedEntity = new List<Entity>();
        float targetAngle = FightClubUtils.AngleBetween(Vector2.Right,AbilityPositionOrDirection);
        _angleHigh = targetAngle + EffectConfig.PsionicBeamConfig.Degrees;
        _angleLow = targetAngle - EffectConfig.PsionicBeamConfig.Degrees;
        _eyePos = FightPlayer.Entity.Position + new Vector2(0, EffectConfig.PsionicBeamConfig.EyeOffsetY);

    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        IM.PushZ(-1);
        DebugLine(FightClubUtils.PolarCirclePoint(_eyePos, EffectConfig.PsionicBeamConfig.MaximumRange, _angleLow), Vector4.Blue);
        DebugLine(FightClubUtils.PolarCirclePoint(_eyePos, EffectConfig.PsionicBeamConfig.MaximumRange, _angleHigh), Vector4.Red);
        IM.PopZ();
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
    }

    private void DebugLine(Vector2 point, Vector4 color)
    {
        
        IM.Line(Camera.WorldToScreen(_eyePos), Camera.WorldToScreen(point), 5, color, null, true);
    }
}