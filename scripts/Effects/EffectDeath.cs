
using AO;

public class EffectDeath : FightEffect
{
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        FightPlayer.SetAnimTrigger("death");
        FightPlayer.AddDash(Vector2.Zero, 0);
        FightPlayer.AddBump(Vector2.Zero, true);
    }
    

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.SetAnimTrigger("RESET");
        FightPlayer.SwitchStatus((int)PlayerStatus.Safe);
        FightPlayer.ClearAllEffects();
        FightPlayer.CurrentHealth = FightPlayer.MaxHealth;
    }

    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => false;

    public override bool FreezePlayer => false;
}