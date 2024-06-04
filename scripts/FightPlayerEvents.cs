using AO;

// This class is for events that are scoped to the player.

public partial class FightPlayer
{
    public Action ShieldBreakEvent;
    public Action<int> CoinUpdateEvent;
    public Action<int> TotalElminationUpdateEvent;
    public Action<int> TotalDamageUpdateEvent;

    #region Custom Data Pass to Client

    public struct DamageReactionInfo
    {
        public bool ShieldBroken = false;

        public DamageReactionInfo()
        {
            
        }
    }

    #endregion
    
    #region Local Client Events

    // All clients will receive this event, but we only update the ui if the player is local
    [ClientRpc]
    public void NotifyCoinUpdate(int c)
    {
        if (IsLocal)
        {
            Coins = c;
            CoinUpdateEvent?.Invoke(c);
        }
    }

    #endregion

    private void HookupGlobalEvents()
    {
        // Hook up elimination event and damage event
        FightClubGameManager.Instance.PlayerDamageEvent += (source, victim, amt) =>
        {
            if (source == this)
            {
                TotalDamageDealt += amt;
            }
        };

        FightClubGameManager.Instance.PlayerEliminationEvent += (source, victim) =>
        {
            if (source == this)
            {
                TotalEliminations += 1;
            }
        };
    }
}