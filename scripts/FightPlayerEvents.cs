using AO;
using Assembly.scripts;
using Assembly.scripts.SceneObjects;

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
        public enum DamageNumberOverrideType
        {
            None,
            Immune,
            Dodged,
            Parry
        }
        // Server Authoratative Data
        public DamageType DmgType = DamageType.Melee;
        public bool AwardCoin = true; // This is now used to determine if a attack should give EXP
        public int InterruptLevel = 0;
        public ulong SourceNetworkId;
        public DamageNumberOverrideType OverrideDamageNumber = DamageNumberOverrideType.None;

        public Vector4 DamageNumberColor = GlobalData.DamageNumberColor;
        public string SkillKey = "Punch"; // Usage: Fetch icon on the kill feed; Fetch special death animation
        public bool CrateImmediateDestroy = false;
        public bool SpecialDeathAnimation = false;

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
            info.DmgType = type;
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
            if (amount < 0) Log.Error("You don't need to input a negative amount for healing. This function will do that for you.");
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
    public void NotifyCoinUpdate(int _, int c)
    {
        if (IsLocal)
        {
            CoinUpdateEvent?.Invoke(c);
        }
    }


    [ClientRpc]
    public void NotifyDealDamage(FightPlayer victim, DamageInfo info)
    {
        // Check if victim is still alive before setting as priority target
        if (victim.Alive())
        {
            PriorityTarget = victim;
        }
        OnDealDamage?.Invoke(this, info);
    }

    [ClientRpc]
    public void NotifyReceiveDamage(FightPlayer source, DamageInfo info)
    {
        // Check if source is still alive before processing
        if (!source.Alive())
        {
            return;
        }

        OnReceiveDamage?.Invoke(source, info);
        //Log.Warn($"IsServer = {Network.IsServer} - DmgType = {info.DmgType.ToString()}");

        if (Network.IsClient && PlayerStatus == PlayerStatus.Combat)
        {

            // Damage numbers only render if the number is related to the local player
            if (IsLocal || source == Network.LocalPlayer) // Player takes the damage or deals damage
            {
                // Override types
                switch (info.OverrideDamageNumber)
                {
                    case DamageInfo.DamageNumberOverrideType.None:
                        break;
                    case DamageInfo.DamageNumberOverrideType.Immune:
                        FightClubGameManager.Instance.SpawnDamageNumber(Entity.Position + Vector2.Up, GlobalData.OutputDamageNumberColor, "Immune!");
                        return;
                    case DamageInfo.DamageNumberOverrideType.Dodged:
                        FightClubGameManager.Instance.SpawnDamageNumber(Entity.Position + Vector2.Up, GlobalData.OutputDamageNumberColor, "Dodged!");
                        return;
                    case DamageInfo.DamageNumberOverrideType.Parry:
                        FightClubGameManager.Instance.SpawnDamageNumber(Entity.Position + Vector2.Up, GlobalData.OutputDamageNumberColor, "Parried!");
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // Normal Type
                if (source == Network.LocalPlayer && info.DamageNumberColor == GlobalData.DamageNumberColor)
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

    [ClientRpc]
    public void NotifyAfkExp(int xp)
    {
        if (IsLocal && PlayerStatus == PlayerStatus.AFK)
        {
            FightClubGameManager.Instance.SpawnDamageNumber(Entity.Position - Vector2.Up, GlobalData.CritNumberColor, $"EXP+{xp}");
            _overlay.CalculateAfkExp();
            SFX.Play(SFXKeys.AFKAudio, new SFX.PlaySoundDesc());
        }
    }

    [ClientRpc]
    public void NotifyGlory(int gl)
    {
        if (IsLocal && PlayerStatus == PlayerStatus.Combat)
        {
            FightClubGameManager.Instance.SpawnDamageNumber(Entity.Position - Vector2.Up, GlobalData.CritNumberColor, $"Glory +{gl}");
        }
    }

    [ClientRpc]
    public void NotifySpectatorExp(int xp)
    {
        if (IsLocal && PlayerStatus == PlayerStatus.Spectator)
        {
            FightClubGameManager.Instance.SpawnDamageNumber(Entity.Position - Vector2.Up, GlobalData.CritNumberColor, $"EXP+{xp}");
            SFX.Play(SFXKeys.AFKAudio, new SFX.PlaySoundDesc());
        }
    }

    [ClientRpc]
    public void NotifyElimination(FightPlayer source, FightPlayer victim, DamageInfo info, string skillKey)
    {
        //Log.Warn($"{source.Name} Eliminated {victim.Name} with {skillKey}");
        if (IsLocal && (PlayerStatus == PlayerStatus.Combat || PlayerStatus == PlayerStatus.Spectator))
        {
            // Check if both source and victim are still alive before accessing their properties
            if (!source.Alive() || !victim.Alive())
            {
                return;
            }

            int sourceLv = 1;
            if (skillKey != "punch")
            {
                sourceLv = source.GetSkillTree().GetSkillLevel(skillKey);
            }
            else
            {
                sourceLv = source.PunchLevel;
            }

            _combatOverlay.AddKillFeed(source.Name, victim.Name, skillKey, sourceLv);

            if (source.IsLocal)
            {
                SFX.Play(SFXKeys.EliminationAudio, new SFX.PlaySoundDesc());
            }
        }
    }


    #endregion

    /// <summary>
    /// [Server Only]
    /// Subscribe to global damage & elimination events.
    /// They are distributed by the server, so they are reliable.
    /// We use these to handle resources (xp, level, gems, coins, leaderboards etc)
    /// IMPORTANT: RemoveGlobalEvent is called in OnDestroy! <see cref="FightPlayer"/>
    /// MUST REMOVE EVENTS THERE by mirroring subscription
    /// </summary>
    private void HookupGlobalEvents()
    {
        // Hook up elimination event and damage event.

        FightClubGameManager.Instance.PlayerDamageEvent += OnServerPlayerDamage;

        FightClubGameManager.Instance.PlayerEliminationEvent += OnServerPlayerElimination;

        FightClubGameManager.Instance.AfkTick += OnAfkTick;

    }

    /// <summary>
    /// [Server Only]
    /// </summary>
    private void RemoveGlobalEvents()
    {
        FightClubGameManager.Instance.PlayerDamageEvent -= OnServerPlayerDamage;

        FightClubGameManager.Instance.PlayerEliminationEvent -= OnServerPlayerElimination;

        FightClubGameManager.Instance.AfkTick -= OnAfkTick;

    }

    public void RegisterPreDamageEvent(FightEffect pfe)
    {
        _preDamageEffects.Add(pfe);
    }

    public void RemovePreDamageEvent(FightEffect pfe)
    {
        _preDamageEffects.Remove(pfe);
    }

    public void OnServerPlayerDamage(FightPlayer source, FightPlayer victim, DamageInfo info)
    {
        if (source == this && victim != this && info.DmgType != DamageType.Heal)
        {
            // TotalDamageDealt += info.ReactionInfo.Amount; // No longer collects this data
            if (info.AwardCoin)
            {
                Exp += LevelingData.XpForDamage * LevelingData.GetBoostedExpMultiplier(this);
            }
            // Send a callback to the source of damage. This need to reach client & server
            // Check if victim is still alive before calling RPC (victim may have been destroyed during event processing)
            if (victim.Alive())
            {
                CallClient_NotifyDealDamage(victim, info);
            }
        }
    }

    public void OnServerPlayerElimination(FightPlayer source, FightPlayer victim, DamageInfo info)
    {
        if (source == this && victim != this)
        {
            // This player eliminated another player
            TotalEliminations += 1;
            if (IsChampion)
            {
                if (victim.Level > 19)
                {
                    Gem += 5;
                    CallClient_NotifyGlory(5);
                }

            }
            else
            {
                int xp = LevelingData.GetTrueXpDampen(Level, victim.Level, LevelingData.XpForKill);

                xp *= LevelingData.GetBoostedExpMultiplier(this);

                Exp += xp;
                CallClient_NotifyKillExp(xp);
            }


            // Increment BP quest progress
            if (!Game.LaunchedFromEditor && Network.IsServer)
            {
                Battlepass.IncrementProgress(this, "6772ec6ae715d82e0b733b4e", 1);
            }
        }

        if (victim == this && source != this)
        {
            // This player died (from non-self damage)
        }

        // Check if all referenced players are still networked before calling RPC.
        if (this.Alive() && Entity.NetworkId != 0 && source.Alive() && victim.Alive())
        {
            CallClient_NotifyElimination(source, victim, info, info.SkillKey);
        }
    }

    public void OnAfkTick()
    {
        if (PlayerStatus == PlayerStatus.AFK)
        {
            // Give EXP on 60 sec tick
            int baseExp = GlobalData.AfkBaseExp;
            if (Level < GlobalData.AfkLowLevelThreshold)
            {
                baseExp += GlobalData.AfkLowLevelBonus;
            }

            if (Level < GlobalData.AfkMidLevelThreshold)
            {
                baseExp += GlobalData.AfkMidLevelBonus;
            }

            if (Scene.Components<FightPlayer>().Count() < 5)
            {
                baseExp += GlobalData.AfkUnpopulatedServerBonusExp;
            }

            if (IsExpBoosted())
            {
                baseExp *= 2;
            }

            CallClient_NotifyAfkExp(baseExp);
            Exp += baseExp;
        }

        if (IsExpBoosted())
        {
            ExpBoostTime -= 1;
        }
    }
}
