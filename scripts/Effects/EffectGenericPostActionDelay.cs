namespace Assembly.scripts.Effects;

/// <summary>
/// Trigger the post-action animation and add this effect immediately.
/// During that animation, the player cannot move or cast skill, and they can be attack but they'll not flinch.
/// </summary>
public class EffectGenericPostActionDelay : FightEffectWithNoFlinch
{
    protected override bool PreventMovement => true;
    public override bool BlockAbilityActivation => true;

    public override bool IsActiveEffect => true;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        DurationRemaining = MainLayer.GetCurrentStateLength();
    }

    public override void PreDamageMod(ref FightPlayer.DamageInfo info)
    {
        base.PreDamageMod(ref info);
        DurationRemaining = 0.1f;
    }
}