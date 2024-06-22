using AO;
namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityPsyThrow : FightAbility
{
    
}

public class AbilityPsyThrowLaunch : FightAbility
{
    
}



public class EffectPsyThrow : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override List<Type> AbilityWhitelist => new List<Type>() {typeof(AbilityPsyThrowLaunch)};
}

public class EffectPsyThrowLaunch : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => false;
}