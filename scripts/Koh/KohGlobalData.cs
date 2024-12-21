using AO;
using Assembly.scripts;

namespace Assembly.Koh;

public static class KohGlobalData
{
    // Round
    public static int PlayersRequiredToStart = 1;
    public static float RoundTime = 59;

    public static int CalculateCoinReward(int kingScore)
    {
        return kingScore / 2;
    }

    public static int CalculateExpReward(int roundScore, FightPlayer fp)
    {
        return LevelingData.GetBoostedExpMultiplier(fp) * roundScore / 2;
    }

    public static int CalculateGloryReward(int kingScore)
    {
        return (int)(0.1f * kingScore);
    }
    
    
    // Zone Health
    public static int ZoneMaxHealth = 100;
    public static int ZoneRecoverSpeed = 3;
    public static int CapturedDecay = 5;
    public static int CaptureSpeed = 10;
    
    // Assets
    public static Texture CrownGrey = Assets.KeepLoaded<Texture>("UI/KoH/crown_grey.png");
    public static Texture Crown = Assets.KeepLoaded<Texture>("UI/KoH/crown.png");
    public static Texture Clash = Assets.KeepLoaded<Texture>("UI/KoH/clash.png");
    public static Texture BackPlate = Assets.KeepLoaded<Texture>("UI/KoH/backplate.png");
    public static Texture Ribbon = Assets.KeepLoaded<Texture>("UI/KoH/ribbon1.png");
    
    // Scoring
    public static int KingScorePerSecond = 15;
    public static int KillScore = 45;
    
    // Texts
    public static string KingWinText = "Kneel to me for I am the KING!";
    public static string SoloWinText = "You dare not face me! O'Cowards!";
    public static string ScoreWinText = "Time is up. The highest scored player wins!";

    public static string[] Tips = new[]
    {
        "You can win the round by holding the zone for 120s total or holding it when the timer expires.",
        "The Basic tree does not provide stats in KoH mode. You get Punch III when you are the king.",
        "Skills don't need to be unlocked in this game mode, but you still need to do that to upgrade them.",
        "If nobody is holding the zone when the timer expires, the player with highest score wins the game.",
        "All upgrades you purchased in classic Battlegrounds will work here!",
        "At the end of the round, you get 1 coin for each second you hold the zone!",
        "If you win the round, a small amount of glory will be granted to you based on how long you held the zone."
    };

    public struct LastRoundReport
    {
        public bool HasReport;
        public string WinText;
        public string Winner;
        public int YourGlory;
        public int YourExp;
        public int YourCoin;
    }
}