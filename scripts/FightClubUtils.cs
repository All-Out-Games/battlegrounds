using AO;
namespace Assembly.scripts;

public static class FightClubUtils
{
    public static float AngleBetween(Vector2 a, Vector2 b)
    {
        float sin = a.X * b.Y - b.X * a.Y;  
        float cos = a.X * b.X + a.Y * b.Y;

        return AOMath.ToDegrees((float)Math.Atan2(sin, cos));
    }
}