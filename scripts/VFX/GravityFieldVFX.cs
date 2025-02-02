using AO;
namespace Assembly.scripts.VFX;


public class GravityFieldVFX: AttachmentObject
{
    // Gravity Field VFX. Used for Gravity Crush skill.
    [Serialized] public Spine_Animator Animator;
    
    public void SetAnimTrigger(string trigger)
    {
        Animator.SpineInstance.StateMachine.SetTrigger(trigger);
    }

    public virtual void ConstructStateMachine()
    {
        var stateMachine = StateMachine.Make();
        var mainLayer = stateMachine.CreateLayer("main");
        
        var emptyState = mainLayer.CreateState("__CLEAR_TRACK__", 0, true);
        
        var appearState = mainLayer.CreateState("appear", 0, false);
        var idleState = mainLayer.CreateState("loop", 0, true);
        var disappearState = mainLayer.CreateState("disappear", 0, false);

        var appearTrigger = stateMachine.CreateVariable("appear", StateMachineVariableKind.TRIGGER);
        var disappearTrigger = stateMachine.CreateVariable("disappear", StateMachineVariableKind.TRIGGER);
        
        mainLayer.CreateGlobalTransition(appearState).CreateTriggerCondition(appearTrigger);
        mainLayer.CreateTransition(appearState, idleState, true);
        mainLayer.CreateTransition(idleState, disappearState, false).CreateTriggerCondition(disappearTrigger);
        //mainLayer.CreateTransition(disappearState, emptyState, true);

        mainLayer.InitialState = emptyState;
        
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

    public override void Update()
    {

        if (Util.OneTime(TimeElapsed > EntityLifeTime, ref LifeTimeEnded))
        {
            Despawn();
            //Log.Warn($"Entity {Entity.Name} Destroyed!");
        }

        TimeElapsed += Time.DeltaTime;
    }
}