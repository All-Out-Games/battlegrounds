using AO;

public class EffectNoMovement : AEffect
{
    
    private FightPlayer _player;


    public EffectNoMovement()
    {
        IsActiveEffect = false;
        IsValidTarget = false;
        BlockAbilityActivation = false;
        FreezePlayer = false; // Note: This effect WILL NOT freeze the player,
                              // it's just adding a 0 modifier to the player to prevent them from move from input
                              // Used in bump effects
    }
    
    public override void OnEffectStart()
    {
        _player = (FightPlayer)Player;
        _player.AddSpeedModifier(0);
    }

    public override void OnEffectEnd(bool interrupt)
    {
        _player.RemoveSpeedModifier(0);
    }
    
    public override bool IsActiveEffect { get; }
    public override bool BlockAbilityActivation { get; }
    public override bool IsValidTarget { get; }

    public override bool FreezePlayer
    {
        get;
    }
}