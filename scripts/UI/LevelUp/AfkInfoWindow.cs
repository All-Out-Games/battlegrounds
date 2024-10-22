using AO;

namespace Assembly.scripts.UI.LevelUp;

/// <summary>
/// A window that shows the AFK bonus the local players are eligible to receive.
/// </summary>
public class AfkInfoWindow : UniqueUIWindow
{
    [Serialized] public UIText AfkXpTotalNumber;
    [Serialized] public UIText LevelBonusNumber;
    [Serialized] public UIText ServerBonusNumber;
    [Serialized] public UIText BoosterBonusNumber;

    /// <summary>
    /// Update UI. Called on player teleport to AFK (from ResourceOverlayWindow)
    /// Or, when AFKTick is dispatched (from FightPlayerEvents)
    /// </summary>
    public int CalculateAfkInfo()
    {
        var fp = Network.LocalPlayer as FightPlayer;
        if (fp == null)
        {
            return 0;
        }
        int afk = GlobalData.AfkBaseExp;
        int levelBonus = 0;
        if (fp.Level < GlobalData.AfkLowLevelThreshold)
        {
            levelBonus += GlobalData.AfkLowLevelBonus;
        }
        if (fp.Level < GlobalData.AfkMidLevelThreshold)
        {
            levelBonus += GlobalData.AfkMidLevelBonus;
        }

        int serverBonus = Scene.Components<FightPlayer>().Count() < 5 ? GlobalData.AfkUnpopulatedServerBonusExp : 0;

        afk = afk + levelBonus + serverBonus;
        if (fp.IsExpBoosted())
        {
            BoosterBonusNumber.Text = afk.ToString();
            afk *= 2;
        }
        else
        {
            BoosterBonusNumber.Text = "0";
        }
        

        AfkXpTotalNumber.Text = $"{afk} EXP/min";
        LevelBonusNumber.Text = levelBonus.ToString();
        ServerBonusNumber.Text = serverBonus.ToString();
        return afk;
    }
}