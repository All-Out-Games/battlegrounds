using AO;
namespace Assembly.scripts.VFX;

public class StatAuraVFX : AttachmentObject
{
    [Serialized] public Spine_Animator Animator;
    //[Serialized] public Spine_Animator CloudAnimator;
    

    public void SetAnimTrigger(string trigger)
    {
        Animator.SpineInstance.StateMachine.SetTrigger(trigger);
    }

    public void SetSkin(string skin, Vector4 color)
    {
        var spine = Animator.SpineInstance;
        spine.DisableAllSkins();
        spine.EnableSkin(skin);
        spine.RefreshSkins();
        spine.ColorMultiplier = color;
    }

    public virtual void ConstructStateMachine()
    {
        var stateMachine = StateMachine.Make();
        var mainLayer = stateMachine.CreateLayer("main");
        
        var emptyState = mainLayer.CreateState("__CLEAR_TRACK__", 0, true);
        
        var appearState = mainLayer.CreateState("status_up_appear", 0, false);
        var idleState = mainLayer.CreateState("status_up_loop", 0, true);
        var disappearState = mainLayer.CreateState("status_up_disappear", 0, false);


        var appearTrigger = stateMachine.CreateVariable("appear", StateMachineVariableKind.TRIGGER);
        var disappearTrigger = stateMachine.CreateVariable("disappear", StateMachineVariableKind.TRIGGER);
        
        mainLayer.CreateGlobalTransition(appearState).CreateTriggerCondition(appearTrigger);
        mainLayer.CreateTransition(appearState, idleState, true);
        mainLayer.CreateTransition(idleState, disappearState, false).CreateTriggerCondition(disappearTrigger);
        mainLayer.CreateTransition(disappearState, emptyState, true);
        
        mainLayer.SetInitialState(emptyState);
        
        Animator.SpineInstance.SetStateMachine(stateMachine, Entity);
    }
    
    public override void Awake()
    {
        base.Awake();
        Animator ??= Entity.GetComponent<Spine_Animator>();
        if (Animator == null)
        {
            Log.Error($"No Animator found on {Entity.Name}!");
            Despawn();
        }
        else
        {
            ConstructStateMachine();
        }
    }
}