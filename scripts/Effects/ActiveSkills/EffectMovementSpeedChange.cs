namespace Assembly.scripts.Effects.ActiveSkills;

public class EffectMovementSpeedChange : FightEffect
{
    public override bool IsActiveEffect => false;

    public float SpdModifier = 1.0f;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.AddSpeedModifier(SpdModifier);
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.RemoveSpeedModifier(SpdModifier);
    }
}