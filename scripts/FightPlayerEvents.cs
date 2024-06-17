using AO;

// This class is for events that are scoped to the player.

public partial class FightPlayer
{
    public Action ShieldBreakEvent;
    public Action<int> CoinUpdateEvent;
    public Action<int> TotalElminationUpdateEvent;
    public Action<int> TotalDamageUpdateEvent;

    #region Custom Data Pass to Client
    
    public struct DamageInfo
    {
        // Server Only Data
        public DamageType DmgType = DamageType.Melee;
        public bool AwardCoin = true;
        
        // Client & Server Data
        public DamageReactionInfo ReactionInfo = new DamageReactionInfo(); 
        public DamageInfo()
        {
            
        }

        /// <summary>
        /// Default settings. Melee damage.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DamageInfo CreateDamageInfo(int amount, DamageType type = DamageType.Melee)
        {
            DamageInfo info = new DamageInfo();
            info.ReactionInfo.Amount = amount;
            info.DmgType = DamageType.Melee;
            return info;
        }

        /// <summary>
        /// Does not trigger damage reaction, does not reward coins.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static DamageInfo CreateSelfDamageInfo(int amount)
        {
            DamageInfo info = CreateDamageInfo(amount, DamageType.None);
            info.AwardCoin = false;
            info.ReactionInfo.Flinch = false;
            return info;
        }
    }

    public struct DamageReactionInfo
    {
        public bool ShieldBroken = false;
        public bool Flinch = true;
        public int Amount = 0;

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
            if (source == this && victim != this)
            {
                TotalDamageDealt += amt;
                Coins += GlobalData.CoinForAttack;
            }
        };

        FightClubGameManager.Instance.PlayerEliminationEvent += (source, victim) =>
        {
            if (source == this && victim != this)
            {
                TotalEliminations += 1;
                Coins += GlobalData.CoinForElimination; // Kills award 30 coins
            }

            if (victim == this)
            {
                Coins += GlobalData.CoinForDeath; // Death award 15 coins
            }
        };
    }
}