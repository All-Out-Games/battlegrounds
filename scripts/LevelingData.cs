namespace Assembly.scripts;

public static class LevelingData
{
    public static readonly int MaxLevel = 29; // The displayed level is the actual level PLUS ONE
    public static readonly int XpForDamage = 5;
    public static readonly int XpForKill = 100;
    
    public static readonly int XpLowLevelPenalty = 3; // Elimination of lower level players will reward less xp
    public static readonly int XpHighLevelReward = 2; // Take down a higher level player will reward more xp
    
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
    
    // Player get this amount of coins when they reach level i
    public static readonly int[] CoinRewards = new int[] {
        0, 300, 300, 300, 1000,
        450, 450, 450, 450, 2000, 
        600, 600, 600, 600, 4000,
        900, 900, 900, 900, 8000,
        1200, 1200, 1200, 1200, 16000,
        1500, 1500, 1500, 1500, 32000
    };

}