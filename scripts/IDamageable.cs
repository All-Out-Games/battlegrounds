using AO;

namespace Assembly.scripts;

public interface IDamageable
{
    
    
    // This is currently useless
    // But later we'll separate damage / effect delivery for a few abilities, so that they can damage both the player and other stuff
    // e.g. dummies & crates
    
    public abstract void TakeDamage(FightPlayer source, FightPlayer.DamageInfo info);
    public abstract bool Damageable();
}

public abstract class DamageableObject : Component, IDamageable
{
    public abstract void TakeDamage(FightPlayer source, FightPlayer.DamageInfo info);

    public abstract bool Damageable();
}