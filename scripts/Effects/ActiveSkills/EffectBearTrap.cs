using AO;
namespace Assembly.scripts.Effects.ActiveSkills;

public class EffectBearTrap: FightEffect
{
    public override bool IsActiveEffect => false;


    public override void OnEffectStart()
    {
        // TODO
        base.OnEffectStart();
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
    }
}

public class EffectBearTrapSnare : FightEffect
{
    public override bool IsActiveEffect => false;

    public override void OnEffectStart()
    {
        base.OnEffectStart();
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
    }
}