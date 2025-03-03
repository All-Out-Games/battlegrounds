
using AO;
using Assembly.scripts;
using Assembly.scripts.Effects;

public class EffectDeath : FightEffectWithImmunity
{
    protected override string InvincibilityReason => "Dead";
    public string DeathAnimationTrigger = "death";
    private bool _showAd;
    
    
    public static int InterstitialDeathCount = 8;

    public static string GetSpecialDeathAnimationTrigger(string skillKey)
    {
        // TODO: Death Audio
        string res = "death";
        switch (skillKey)
        {
            case "LeapSlam":
                res = "death_knock";
                break;
            case "ClawSlash":
                res = "death_swiped";
                break;
            case "SelfDestruct":
                res = "death_poof";
                break;
            case "PsionicBeam":
                res = "death_poof";
                break;
            case "Backstab":
                res = "death_swiped";
                break;
            case "Fireball":
                res = "death_poof";
                break;
            case "Thunderbolt":
                res = "death_knock";
                break;
            case "ChargingStation":
                res = "death_knock";
                break;
            case "FlashOfSteel":
                res = "death_swiped";
                break;
            case "FireTornado":
                res = "death_poof";
                break;
            case "MeteorStrike":
                res = "death_poof";
                break;
        }

        if (res == "death")
        {
            Log.Warn("You didn't specify the trigger for a special death animation! Write a case in EffectDeath!");
        }
        return res;
    }

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        // FightPlayer.SetAnimTrigger("RESET");
        FightPlayer.SetAnimTrigger(DeathAnimationTrigger);
        FightPlayer.AddDash(Vector2.Zero, 0);
        FightPlayer.AddBump(Vector2.Zero, true);
        
        if (Network.IsServer)
        {
            List<FightPlayer> spectators =
                FightClubGameManager.Instance.OverlapCircleForSpectators(Position, GlobalData.SpectatorXpRadius, Player);
            foreach (var fp in spectators)
            {
                if (fp.Alive())
                {
                    int xp = fp.Level < GlobalData.AfkMidLevelThreshold ? GlobalData.LowLvSpectatorXp : GlobalData.HighLvSpectatorXp;
                    xp *= LevelingData.GetBoostedExpMultiplier(fp);
                    fp.Exp += xp;
                    fp.CallClient_NotifySpectatorExp(xp);
                }
            }
        }

        /*if (FightPlayer.IsLocal && FightPlayer.DeathCount % InterstitialDeathCount == 0)
        {
            if (!FightPlayer.IsVIP && FightPlayer.TotalEliminations > 50)
            {
                _showAd = true;
                Notifications.Show("A short Ad will play after your respawn. Bypass this Ad by being a VIP!");
            }
        }*/
        FightPlayer.DeathCount += 1;
        
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if (MainLayer.CurrentState.Name == "Idle")
        {
            FightPlayer.SetAnimTrigger(DeathAnimationTrigger);
        }
        else
        {
            FightPlayer.UnsetAnimTrigger("death");
            FightPlayer.UnsetAnimTrigger(DeathAnimationTrigger);
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.SetAnimTrigger("RESET");
        FightPlayer.SetAnimTrigger("RESET_AL");
        FightPlayer.SwitchStatus((int)PlayerStatus.Safe); // teleport the player to central hub
        FightPlayer.CurrentHealth = FightPlayer.MaxHealth;

        var slots = FightPlayer.GetSkillSlots().GetCurrentAbilities();
        foreach (var slot in slots)
        {
            slot.CooldownRemaining = 1;
        }
        
        FightPlayer.ClearAllEffects();
        FightPlayer.ClearSpeedModifier();
        
        // Removed 250216 - Shin
        /*if (_showAd && Ads.IsInterstitialAdLoaded())
        {
            Ads.ShowInterstitial();
        }*/
    }

    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => false;

    protected override bool PreventMovement => true;

    public override bool FreezePlayer => false; // Set to true could mess up with Teleport()
}