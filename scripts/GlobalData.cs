using AO;
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
    Heal,
    None // NONE type will not trigger damage-type related reactions
}

#endregion

public static class GlobalData
{
    public static float RespawnTime = 3f;
    public static float CombatSpeedModifier = 0.75f;
    public static float SafeSpeedModifier = 1.25f;

    public static int DefaultMaxHealth = 100;
    public static int DefaultAtk = 10;
    
    #region Coins Related

    public static int CoinForAttack = 10;
    public static int CoinForElimination = 30;
    public static int CoinForDeath = 15;

    #endregion
    
    public static Vector4 DamageNumberColor = Vector4.Red;
    public static Vector4 HealNumberColor = Vector4.Green;
    public static Vector4 CritNumberColor = new Vector4(1, 0.68f, 0, 1);
    public static Vector4 ShieldNumberColor = Vector4.LightBlue;
    public static Vector4 OutputDamageNumberColor = Vector4.White;
}