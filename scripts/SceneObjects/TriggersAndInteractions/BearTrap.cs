using AO;
using Assembly.scripts.Effects;
using Assembly.scripts.Effects.ActiveSkills;

namespace Assembly.scripts.SceneObjects.TriggersAndInteractions;

public class BearTrap : OwnedTrigger
{
    [Serialized] public bool Snapped;
    protected override void OnOtherPlayerEnter(FightPlayer fp)
    {
        if(Snapped) return;
        
        base.OnOtherPlayerEnter(fp);
        if (!Owner.IsLocal)
        {
            Animator.LocalEnabled = true;
        }
        Animator.SpineInstance.StateMachine.SetTrigger("snap");
        Snapped = true;

        fp.AddEffect<EffectBearTrapSnare>(Owner);
    }

    public override void Start()
    {
        base.Start();
        
        // Make a state machine.
        var stateMachine = StateMachine.Make();
        var mainLayer = stateMachine.CreateLayer("main");

        var appearSetUpState = mainLayer.CreateState("appear_setup", 0, false);
        var idleState = mainLayer.CreateState("idle", 0, true);
        var snapCloseState = mainLayer.CreateState("snap_close", 0, false);
        var disappearState = mainLayer.CreateState("disappear", 0, false);
        var disappearClosedState = mainLayer.CreateState("disappear_closed", 0, false);

        var snapTrigger = stateMachine.CreateVariable("snap", StateMachineVariableKind.TRIGGER);
        var disappearTrigger = stateMachine.CreateVariable("expire", StateMachineVariableKind.TRIGGER);

        mainLayer.CreateTransition(appearSetUpState, idleState, true);
        mainLayer.CreateTransition(idleState, snapCloseState, false).CreateTriggerCondition(snapTrigger);
        mainLayer.CreateGlobalTransition(disappearState).CreateTriggerCondition(disappearTrigger);
        mainLayer.CreateTransition(snapCloseState, disappearClosedState, true);
        
        Animator.SpineInstance.SetStateMachine(stateMachine, Entity);
        Animator.OnAnimationEnd += OnAnimationEnd;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Animator.OnAnimationEnd -= OnAnimationEnd;
    }

    public void OnAnimationEnd(string anim)
    {
        if (anim == "disappear" || anim == "disappear_closed")
        {
            Animator.LocalEnabled = true;
            Despawn();
        }

        if (anim == "appear_setup")
        {
            if (!Owner.IsLocal)
            {
                Animator.LocalEnabled = false; // Hide for non-local player
            }
        }
    }

    protected override void OnLifeTimeRunOut()
    {
        Animator.LocalEnabled = true;
        Animator.SpineInstance.StateMachine.SetTrigger("expire");
    }
}