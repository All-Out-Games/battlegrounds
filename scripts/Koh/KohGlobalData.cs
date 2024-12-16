using AO;

namespace Assembly.Koh;

public static class KohGlobalData
{
    // Round
    public static int PlayersRequiredToStart = 1;
    public static float RoundTime = 59;
    
    
    // Zone Health
    public static int ZoneMaxHealth = 100;
    public static int ZoneRecoverSpeed = 3;
    public static int CapturedDecay = 5;
    public static int CaptureSpeed = 10;
    
    // Assets
    public static Texture CrownGrey = Assets.KeepLoaded<Texture>("UI/KoH/crown_grey.png");
    public static Texture Crown = Assets.KeepLoaded<Texture>("UI/KoH/crown.png");
    public static Texture Clash = Assets.KeepLoaded<Texture>("UI/KoH/clash.png");
}