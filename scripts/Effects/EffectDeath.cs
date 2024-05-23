
using AO;

public class EffectDeath : FightEffect
{
    EffectDeath()
    {
        IsActiveEffect = true;
        IsValidTarget = false;
        BlockAbilityActivation = true;
        FreezePlayer = true;
        
    }
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        FightPlayer.SetAnimTrigger("death");
        FightPlayer.SetSkillBlockCast(true);
    }

    public override void OnEffectUpdate()
    {
        
    }

    public override void OnEffectEnd(bool interrupt)
    {
        FightPlayer.SetAnimTrigger("RESET");
        FightPlayer.SetSkillBlockCast(false);
    }

    public override bool IsActiveEffect { get; }
    public override bool BlockAbilityActivation { get; }
    public override bool IsValidTarget { get; }

    public override bool FreezePlayer { get;}
}