using AO;

namespace Assembly.scripts;

public interface IDamageable
{
    public abstract void TakeDamage(FightPlayer source, FightPlayer.DamageInfo info);
    public abstract bool Damageable();
}

public abstract class DamageableObject : Component, IDamageable
{
    public abstract void TakeDamage(FightPlayer source, FightPlayer.DamageInfo info);

    public abstract bool Damageable();
}