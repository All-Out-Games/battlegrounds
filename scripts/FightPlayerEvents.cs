using AO;
using Assembly.scripts;

// This class is for events that are scoped to the player.

public partial class FightPlayer
{
    public Action<int> CoinUpdateEvent;
    public Action<int> PlayerSwitchZoneEvent;
    

    // Reserved for effects related to post-damage (e.g. after elimination, add damage)
    public Action<FightPlayer, DamageInfo> OnDealDamage; // Triggered in global damage event. Will contain the ACTUAL damage dealt (i.e. the damage info might be modified by some effects like parry)
    public Action<FightPlayer, DamageInfo> OnReceiveDamage; // Triggered in CallClient_TakeDamage, after you receive damage
    public Action OnElimination; // Triggered in global elimination event;
    public Action<SkillActivationInfo> OnSkillActivate;

    private List<FightEffect> _preDamageEffects;
    
    #region Custom Data Pass to Client
    
    public struct DamageInfo
    {
        // Server Authoratative Data
        public DamageType DmgType = DamageType.Melee;
        public bool AwardCoin = true; // This is now used to determine if a attack should give EXP
        public int InterruptLevel = 0;
        public ulong SourceNetworkId;
        public bool SpawnDamageNumber = true;
        public Vector4 DamageNumberColor = GlobalData.DamageNumberColor;
        
        // Client & Server Data
        public DamageReactionInfo ReactionInfo = new DamageReactionInfo(); 
        public DamageInfo()
        {
            
        }

        public static int KnockBackInterruptLevel = 2000;
        public static int StunInterruptLevel = 5000;

        /// <summary>
        /// Default settings. Melee damage.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="type"></param>
        /// <param name="interruptLv">Interruption level. 0 means doesn't interrupt anything.</param>
        /// <returns></returns>
        public static DamageInfo CreateDamageInfo(int amount, DamageType type = DamageType.Melee, int interruptLv = 1000)
        {
            DamageInfo info = new DamageInfo();
            info.ReactionInfo.Amount = amount;
            info.DmgType = DamageType.Melee;
            info.InterruptLevel = interruptLv;
            return info;
        }

        /// <summary>
        /// Does not trigger damage reaction, does not reward coins.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static DamageInfo CreateSelfDamageInfo(int amount)
        {
            DamageInfo info = CreateDamageInfo(amount, DamageType.None, 0);
            info.AwardCoin = false;
            info.ReactionInfo.Flinch = false;
            return info;
        }

        public static DamageInfo CreateHealInfo(int amount)
        {
            if(amount < 0) Log.Error("You don't need to input a negative amount for healing. This function will do that for you.");
            else
            {
                amount = -amount;
            }

            DamageInfo info = CreateDamageInfo(amount, DamageType.Heal, 0);
            info.AwardCoin = false;
            info.DamageNumberColor = GlobalData.HealNumberColor;
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
    
    public struct SkillActivationInfo
    {
        public int InterruptLevel; // Generic Interruption
        public string SkillKey; // Specific Interruption

        public static SkillActivationInfo GetActivationInfo(int level, string skillKey)
        {
            return new SkillActivationInfo() { InterruptLevel = level, SkillKey = skillKey };
        }
    }

    #endregion
    
    // These are functions that handles local client events.
    // They are a type of events that usually invoked for UIs after receiving a SyncVar update 
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
        
        if (Network.IsClient && PlayerStatus == PlayerStatus.Combat)
        {
            // Damage numbers only render if the number is related to the local player
            if (IsLocal || source == Network.LocalPlayer.Entity) // Player takes the damage or deals damage
            {
                if (source == Network.LocalPlayer.Entity && info.DamageNumberColor == GlobalData.DamageNumberColor)
                {
                    info.DamageNumberColor = GlobalData.OutputDamageNumberColor;
                }
                FightClubGameManager.Instance.SpawnDamageNumber(Entity.Position + Vector2.Up, info.DamageNumberColor, int.Abs(info.ReactionInfo.Amount).ToString());
            }
            
        }
    }

    [ClientRpc]
    public void NotifyKillExp(int xp)
    {
        if (IsLocal && PlayerStatus == PlayerStatus.Combat)
        {
            FightClubGameManager.Instance.SpawnDamageNumber(Entity.Position - Vector2.Up, GlobalData.CritNumberColor, $"EXP+{xp}");
        }
    }
    

    #endregion

    /// <summary>
    /// [Server Only]
    /// Subscribe to global damage & elimination events.
    /// They are distributed by the server, so they are reliable.
    /// We use these to handle resources (xp, level, gems, coins, leaderboards etc)
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
                    Exp += LevelingData.GetMultipliedExp(LevelingData.XpForDamage);
                }
                // Send a callback to the source of damage. This need to reach client & server
                source.CallClient_NotifyDealDamage(info);
            }
        };

        FightClubGameManager.Instance.PlayerEliminationEvent += (source, victim) =>
        {
            if (source == this && victim != this)
            {
                // This player eliminated another player
                TotalEliminations += 1;
                int lvDifference = source.Level - victim.Level;
                int xp = LevelingData.XpForKill;
                // Adjust xp based on level differences
                if (lvDifference > 0)
                {
                    xp -= LevelingData.XpLowLevelPenalty * lvDifference;
                }
                else
                {
                    xp -= LevelingData.XpHighLevelReward * lvDifference;
                }

                xp = LevelingData.GetMultipliedExp(xp);
                
                Exp += xp;
                CallClient_NotifyKillExp(xp);
            }

            if (victim == this && source != this)
            {
                // This player died (from non-self damage)
            }
        };
    }

    public void RegisterPreDamageEvent(FightEffect pfe)
    {
        _preDamageEffects.Add(pfe);
    }

    public void RemovePreDamageEvent(FightEffect pfe)
    {
        _preDamageEffects.Remove(pfe);
    }
}