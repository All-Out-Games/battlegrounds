namespace Assembly.scripts.Effects;

/// <summary>
/// Trigger the post-action animation and add this effect immediately.
/// During that animation, the player cannot move or cast skill, and they can be attack but they'll not flinch.
/// </summary>
public class EffectGenericPostActionDelay : FightEffectWithNoFlinch
{
    protected override bool PreventMovement => true;
    public override bool BlockAbilityActivation => true;

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        DurationRemaining = MainLayer.GetCurrentStateLength();
    }
}