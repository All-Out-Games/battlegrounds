using AO;

namespace Assembly.scripts;

public static class LevelingData
{
    public static readonly int MaxLevel = 29; // The displayed level is the actual level PLUS ONE
    public static readonly int XpForDamage = 5;
    public static readonly int XpForKill = 100;
    
    public static readonly int XpLowLevelPenalty = 3; // Elimination of lower level players will reward less xp
    public static readonly int XpHighLevelReward = 2; // Take down a higher level player will reward more xp

    public static readonly float LevelingWindowStayTime = 5f;
    public static readonly TimeZoneInfo PST = TimeZoneInfo.CreateCustomTimeZone("PST", TimeSpan.FromHours(-7), "PST", "PST");

    // Total XP a player needs for Level i
    public static readonly int[] BaselineXp = {
        0, 500, 1500, 3000, 5000, 
        7500, 10500, 14000, 18000, 22500,
        27500, 33000, 39000, 45500, 52500,
        60000, 68000, 76500, 85500, 95000,
        105000, 115500, 126500, 138000, 150000,
        162500, 175500, 189000, 203000, 217500
    };
    
    // Total XP a player needs for level i
    public static readonly int[] NextLevelXp = {
        500, 1500, 3000, 5000, 7500, 
        10500, 14000, 18000, 22500, 27500, 
        33000, 39000, 45500, 52500, 60000, 
        68000, 76500, 85500, 95000, 105000, 
        115500, 126500, 138000, 150000, 162500,
        175500, 189000, 203000, 217500, 232500
    };

    public static readonly int MaxXp = NextLevelXp[MaxLevel];
    
    // Player get this amount of coins when they reach level i
    public static readonly int[] CoinRewards = {
        0, 300, 300, 300, 1000,
        450, 450, 450, 450, 2000, 
        600, 600, 600, 600, 4000,
        900, 900, 900, 900, 8000,
        1200, 1200, 1200, 1200, 16000,
        1500, 1500, 1500, 1500, 32000
    };
    
    public static readonly int[] GemRewards = {
        0, 0, 0, 0, 500,
        0, 0, 0, 0, 650, 
        0, 0, 0, 0, 900,
        0, 0, 0, 0, 1200,
        0, 0, 0, 0, 1500,
        0, 0, 0, 0, 1800
    };

    /// <summary>
    /// [Server Only] Made for double XP events.
    /// We check the date and decide if we want to give out more XP 
    /// </summary>
    /// <param name="xp"></param>
    /// <returns></returns>
    public static int GetMultipliedExp(int xp)
    {
        //Log.Warn($"Day is: {DateTime.Now.Day}");
        if (DoubleXP(DateTime.UtcNow))
        {
            return (int)float.Ceiling(xp * 1.2f);
        }
        else
        {
            return xp;
        }
    }

    public static bool DoubleXP(DateTime timeNow)
    {
        //Log.Error(timeNow.Kind.ToString());
        var timeConverted = TimeZoneInfo.ConvertTimeFromUtc(timeNow, PST);
        return timeConverted.DayOfWeek == DayOfWeek.Saturday || timeConverted.DayOfWeek == DayOfWeek.Sunday;
    }

    public static int GetTrueXp(int level, int victimLevel, int xp)
    {
        int lvDifference = level - victimLevel;
        // Adjust xp based on level differences
        if (lvDifference > 0)
        {
            xp -= XpLowLevelPenalty * lvDifference;
        }
        else
        {
            xp -= XpHighLevelReward * lvDifference;
        }
        return xp;
    }
    
    public static int GetTrueXpDampen(int level, int victimLevel, int xp)
    {
        // Josh's version
        if (level == 0 || victimLevel == 0)
        {
            return GetTrueXp(level, victimLevel, xp); // Fallback to old version
        }
        float rewardCoef = 2 * (float)(victimLevel * victimLevel) / (level + victimLevel);
        rewardCoef /= victimLevel;

        xp = (int)(float.Ceiling(xp * rewardCoef) + 0.1);
        return xp;
    }
}