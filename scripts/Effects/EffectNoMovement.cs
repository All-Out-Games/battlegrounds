using AO;

/// <summary>
/// Blocks movement input (by setting a speed modifier) / WILL NOT fix player in place (e.g. not immune to bumping).
/// Very useful for multiple occasions.
/// </summary>
public partial class EffectNoMovement : FightEffect
{
    protected override bool PreventMovement => true;
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => false;
}