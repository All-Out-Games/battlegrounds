using AO;

// This class is for events that are scoped to the player.

public partial class FightPlayer
{
    public Action ShieldBreakEvent;


    #region Custom Data Pass to Client

    public struct DamageReactionInfo
    {
        public bool ShieldBroken = false;

        public DamageReactionInfo()
        {
            
        }
    }

    #endregion
}