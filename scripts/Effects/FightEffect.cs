using AO;



/// <summary>
/// Base class derived from AEffect to get FightPlayer 
/// </summary>
public abstract class FightEffect : AEffect
{
    protected FightPlayer FightPlayer;

    /// <summary>
    /// Get the owner as FightPlayer
    /// </summary>
    public override void OnEffectStart()
    {
        FightPlayer = (FightPlayer)Player;
    }
}