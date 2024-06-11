namespace Assembly.scripts.Effects;

public class EffectNoMovementWithInvincibility : EffectNoMovement
{
    public override bool IsValidTarget => false;
}