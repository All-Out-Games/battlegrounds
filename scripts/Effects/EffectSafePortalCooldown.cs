namespace Assembly.scripts.Effects;

/// <summary>
/// Stop players from teleporting to safe zone. Prevent high level players cheesing the central hub heal.
/// </summary>
public class EffectSafePortalCooldown : FightEffect
{
    public override bool IsActiveEffect => false;
}