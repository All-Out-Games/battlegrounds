
using AO;
using Assembly.scripts.Effects;

public class EffectDeath : FightEffectWithImmunity
{
    protected override string InvincibilityReason => "Dead";

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.SetAnimTrigger("death");
        FightPlayer.AddDash(Vector2.Zero, 0);
        FightPlayer.AddBump(Vector2.Zero, true);
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if (MainLayer.CurrentState.Name == "Idle")
        {
            FightPlayer.SetAnimTrigger("death");
        }
        else
        {
            FightPlayer.UnsetAnimTrigger("death");
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.SetAnimTrigger("RESET");
        FightPlayer.SetAnimTrigger("RESET_AL");
        FightPlayer.SwitchStatus((int)PlayerStatus.Safe); // teleport the player to central hub
        FightPlayer.CurrentHealth = FightPlayer.MaxHealth;

        var slots = FightPlayer.GetSkillSlots().GetCurrentAbilities();
        foreach (var slot in slots)
        {
            slot.CooldownRemaining = 1;
        }
        
        FightPlayer.ClearAllEffects();
        FightPlayer.ClearSpeedModifier();
    }

    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => false;

    protected override bool PreventMovement => true;

    public override bool FreezePlayer => false; // Set to true could mess up with Teleport()
}