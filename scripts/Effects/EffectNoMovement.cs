using AO;

/// <summary>
/// Blocks movement input (by setting a speed modifier) / WILL NOT fix player in place (e.g. not immune to bumping).
/// Very useful for multiple occasions.
/// </summary>
public partial class EffectNoMovement : FightEffect
{
    
    
    public override void OnEffectStart()
    {
        FightPlayer = (FightPlayer)Player;
        FightPlayer.AddSpeedModifier(0f);
    }

    public override void OnEffectEnd(bool interrupt)
    {
        FightPlayer.RemoveSpeedModifier(0f);
    }

    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => false;
    public override bool IsValidTarget => true;
}