using AO;
#region Gameplay Enums

public enum PlayerStatus
{
    Combat,
    Safe,
    AFK,
    Spectator
}

public enum DamageType
{
    Melee, // Default
    Ranged, // Projectile
    AOE, // Blasts
    Heal,
    None // NONE type will not trigger damage-type related reactions
}

#endregion

public static class GlobalData
{
    // Stats
    
    public static float RespawnTime = 3f;
    public static readonly float CombatSpeedModifier = 0.65f;
    public static readonly float SafeSpeedModifier = 1.25f;

    public static readonly int DefaultMaxHealth = 150;
    public static readonly int DefaultAtk = 15;
    
    // Coins & Exp - Coins were changed to be given by leveling
    public static int CoinForAttack = 10;
    public static int CoinForElimination = 30;
    public static int CoinForDeath = 15;
    
    public static readonly int AfkBaseExp = 3; // Exp given to players in AFK zone, per minute.
    public static readonly int AfkUnpopulatedServerBonusExp = 3; // When player count < 5 (entire server, including players in combat), bonus is given (encouraging VIPs to open private server) 
    public static readonly int AfkLowLevelThreshold = 5; // Players with level <= this value will be considered low-level and receive +14 from AFK
    public static readonly int AfkMidLevelThreshold = 15; // Players with level <= this value will receive +6 from AFK (Stack with the L bonus)
    public static readonly int AfkLowLevelBonus = 14;
    public static readonly int AfkMidLevelBonus = 6;
    
    

    
    // Damage Number Color

    public static Vector4 DamageNumberColor = Vector4.Red;
    public static Vector4 HealNumberColor = Vector4.Green;
    public static Vector4 CritNumberColor = new Vector4(1, 0.68f, 0, 1);
    public static Vector4 ShieldNumberColor = Vector4.LightBlue;
    public static Vector4 OutputDamageNumberColor = Vector4.White;
    
    // Skill Tree
    public static int GridMaxX = 2;
    public static int GridMaxY = 3;
    
    // KillFeed
    public static Vector4 SelfIdColor = new Vector4(1, 0.68f, 0, 1);
    public static Vector4 OtherIdColor = Vector4.White;
    public static float KillFeedLifeTime = 4f;
    
    // Crate Spawn
    public static float CrateSpawnTime = 10;
    public static float CrateLifeTime = 25;
    public static float CrateDropLifeTime = 6;
    
    // Ad Crate
    public static float AdCrateLifeTime = 25;
    public static float AdCrateSpawnTime = 30;
    public static float XpCrateChance = 0.25f;
    public static float XpBoostChance = 0.25f;
    public static float VgPotionChance = 0.75f;
    public static float SpectralSpawnChance = 0.2f;
    // TODO: Coins and Glory item
    
    // Spectator
    public static float SpectatorXpRadius = 10f;
    public static int LowLvSpectatorXp = 200;
    public static int HighLvSpectatorXp = 40;
}