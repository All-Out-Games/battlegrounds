using AO;

namespace Assembly.scripts.Effects;

public class EffectBleed : FightEffect
{
    
    public override bool IsActiveEffect => false;

    public int PerSecondDmg = 0;
    protected float NextDmgTick = 1;
    protected bool Ticked = false;
    
    

    public override void OnEffectUpdate()
    {
        if (Util.OneTime(ElapsedTime > NextDmgTick, ref Ticked))
        {
            Bleed();
            NextDmgTick += 1;
            Ticked = false;
        }
    }

    protected void Bleed()
    {
        FightPlayer.DamageInfo selfDmgInfo = FightPlayer.DamageInfo.CreateSelfDamageInfo(PerSecondDmg);
        FightPlayer.TakeDamage(Caster as FightPlayer, selfDmgInfo); // You can create self damage that comes from other players.
    }
}