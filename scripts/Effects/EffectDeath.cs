
using AO;
using Assembly.scripts.Effects;

public class EffectDeath : FightEffectWithImmunity
{
    protected override string InvincibilityReason => "Dead";

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
        FightPlayer.SwitchStatus((int)PlayerStatus.Safe); // teleport the player to central hub
        FightPlayer.CurrentHealth = FightPlayer.MaxHealth;
        
        FightPlayer.ClearAllEffects();
    }

    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => false;

    protected override bool PreventMovement => true;

    public override bool FreezePlayer => false; // Set to true could mess up with Teleport()
}