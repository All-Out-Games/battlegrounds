using AO;
using Assembly.scripts.Effects;
using Assembly.scripts.Effects.ActiveSkills;

namespace Assembly.scripts.SceneObjects.TriggersAndInteractions;

public class BearTrap : OwnedTrigger
{
    [Serialized] public bool Snapped;
    protected override void OnOtherPlayerEnter(FightPlayer fp)
    {
        if (Snapped)
        {
            Log.Error("This trap already snapped!");
            return;
        }
        Log.Error("SNAP!!");
        base.OnOtherPlayerEnter(fp);
        Animator.SpineInstance.ColorMultiplier = new Vector4(1,1,1, 1);
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
        

        var appearSetUpState = mainLayer.CreateState("appear_set_up", 0, false);
        var idleState = mainLayer.CreateState("idle", 0, true);
        var snapCloseState = mainLayer.CreateState("snap_close", 0, false);
        var disappearState = mainLayer.CreateState("dissappear", 0, false);
        var disappearClosedState = mainLayer.CreateState("dissappear_closed", 0, false);
        var emptyState = mainLayer.CreateState("__CLEAR_TRACK__", 0, false);

        var snapTrigger = stateMachine.CreateVariable("snap", StateMachineVariableKind.TRIGGER);
        var disappearTrigger = stateMachine.CreateVariable("expire", StateMachineVariableKind.TRIGGER);

        mainLayer.SetInitialState(appearSetUpState);
        mainLayer.CreateTransition(appearSetUpState, idleState, true);
        mainLayer.CreateTransition(idleState, snapCloseState, false).CreateTriggerCondition(snapTrigger);
        mainLayer.CreateGlobalTransition(disappearState).CreateTriggerCondition(disappearTrigger);
        mainLayer.CreateTransition(snapCloseState, disappearClosedState, true);

        mainLayer.CreateTransition(disappearState, emptyState, true);
        mainLayer.CreateTransition(disappearClosedState, emptyState, true);
        
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
        Log.Debug($"Animation End {anim}");
        if (anim == "dissappear" || anim == "dissappear_closed")
        {
            Animator.LocalEnabled = true;
            Despawn();
        }

        if (anim == "appear_set_up")
        {
            //var spr = Entity.GetComponent<Sprite_Renderer>();
            //Log.Warn($" Is there a renderer? {spr != null}"); // No
            /*if (!Owner.IsLocal)
            {
                Animator.SpineInstance.ColorMultiplier = new Vector4(1,1,1, 0); // Hide for non-local player
            }*/
        }
    }

    protected override void OnLifeTimeRunOut()
    {
        Animator.SpineInstance.StateMachine.SetTrigger("expire");
    }
}