using AO;

// This class is for events that are scoped to the player.

public partial class FightPlayer
{
    public Action ShieldBreakEvent;
    public Action<int> CoinUpdateEvent;
    public Action<int> TotalElminationUpdateEvent;
    public Action<int> TotalDamageUpdateEvent;

    // Reserved for effects related to post-damage (e.g. after elimination, add damage)
    public Action<FightPlayer, DamageInfo> OnDealDamage; // Triggered in global damage event. Will contain the ACTUAL damage dealt (i.e. the damage info might be modified by some effects like parry)
    public Action<FightPlayer, DamageInfo> OnReceiveDamage; // Triggered in CallClient_TakeDamage
    public Action OnElimination; // Triggered in global elimination event;

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

    [ClientRpc]
    public void NotifyDealDamage(DamageInfo info)
    {
        OnDealDamage?.Invoke(this, info);
    }

    [ClientRpc]
    public void NotifyReceiveDamage(Entity source, DamageInfo info)
    {
        OnReceiveDamage?.Invoke(source.GetComponent<FightPlayer>(), info);
    }

    #endregion

    /// <summary>
    /// Subscribe to global damage & elimination events. Note that these events are client only.
    /// </summary>
    private void HookupGlobalEvents()
    {
        // Hook up elimination event and damage event
        FightClubGameManager.Instance.PlayerDamageEvent += (source, victim, info) =>
        {
            if (source == this && victim != this && info.DmgType != DamageType.Heal)
            {
                TotalDamageDealt += info.ReactionInfo.Amount;
                if (info.AwardCoin)
                {
                    Coins += GlobalData.CoinForAttack;
                }
                // Send a callback. This need to reach client & server
                source.CallClient_NotifyDealDamage(info);
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