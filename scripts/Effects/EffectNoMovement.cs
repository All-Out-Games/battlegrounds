using AO;


/// <summary>
/// Blocks movement input (by setting a speed modifier)
/// Very useful for multiple occasions.
/// </summary>
public class EffectNoMovement : FightEffect
{
    
    public EffectNoMovement()
    {
        IsActiveEffect = false;
        IsValidTarget = false;
        BlockAbilityActivation = false;
        FreezePlayer = false; // Note: This effect WILL NOT freeze the player,
                              // it's just adding a 0 modifier to the player to prevent them from move from input
                              // Useful for when you want to block active movement but not passive ones (e.g. bumping)
    }
    
    public override void OnEffectStart()
    {
        FightPlayer = (FightPlayer)Player;
        FightPlayer.AddSpeedModifier(0);
    }

    public override void OnEffectEnd(bool interrupt)
    {
        FightPlayer.RemoveSpeedModifier(0);
    }
    
    public override bool IsActiveEffect { get; }
    public override bool BlockAbilityActivation { get; }
    public override bool IsValidTarget { get; }

    public override bool FreezePlayer
    {
        get;
    }
}