using AO;
using Assembly.scripts;

public class PlayerCollisionChild : DamageableObject
{
    public FightPlayer Player;


    public override void TakeDamage(FightPlayer source, FightPlayer.DamageInfo info)
    {
        Player.TakeDamage(source, info);
    }

    public override bool Damageable()
    {
        return Player.Damageable();
    }
}