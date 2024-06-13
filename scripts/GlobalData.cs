#region Gameplay Enums

public enum PlayerStatus
{
    Combat,
    Safe,
    AFK,
    Spectating
}

public enum DamageType
{
    Melee, // Default
    Ranged, // Projectile
    AOE, // Blasts
    None // NONE type will not trigger damage-type related reactions
}

#endregion

public static class GlobalData
{
    public static float RespawnTime = 3f;
    
    #region Coins Related

    public static int CoinForAttack = 10;
    public static int CoinForElimination = 30;
    public static int CoinForDeath = 15;

    

    #endregion
}