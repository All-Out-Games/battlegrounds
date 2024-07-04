using AO;

namespace Assembly.scripts.Effects;

public class EffectStun : EffectNoMovement
{
    public override bool BlockAbilityActivation => true;
}

public class EffectKnockDown : FightEffectWithNoFlinch
{
    
    protected override bool PreventMovement => true;
    public override bool BlockAbilityActivation => true;

    protected override int InterruptLevel => FightPlayer.DamageInfo.KnockBackInterruptLevel;

    private bool _interruptable = false;

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        FightPlayer.SetAnimTrigger("knockdown");
        FightPlayer.SpineAnimator.OnAnimationEnd += OnAnimationEnd;
    }

    public override bool Interruptable(int incoming)
    {
        return base.Interruptable(incoming) && _interruptable;
    }

    public void OnAnimationEnd(string ani)
    {
        if (ani == "BAT_003/knocked_down")
        {
            _interruptable = true; // Interruptable after fall down animation
            FightPlayer.OnReceiveDamage += OnDamageEvent;
        }

    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        if (interrupt)
        {
            FightPlayer.UnsetAnimTrigger("knockdown_end");
            FightPlayer.SetAnimTrigger("flinch");
        }
        else
        {
            FightPlayer.SetAnimTrigger("knockdown_end");
        }
        FightPlayer.OnReceiveDamage -= OnDamageEvent;
        FightPlayer.SpineAnimator.OnAnimationEnd -= OnAnimationEnd;
        //FightPlayer.SetAnimTrigger("knockdown_end");
    }
}