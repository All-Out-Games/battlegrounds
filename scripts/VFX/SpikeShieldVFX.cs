namespace Assembly.scripts.VFX;

using AO;

public class SpikeShieldVFX : AttachmentObject
{
    // Shield VFX: Attach to the player, begin despawn countdown after break animation played.
    [Serialized] public Spine_Animator Animator;

    public bool Broken;
    

    public void SetAnimTrigger(string trigger)
    {
        Animator.SpineInstance.StateMachine.SetTrigger(trigger);
    }

    public virtual void ConstructStateMachine()
    {
        var stateMachine = StateMachine.Make();
        var mainLayer = stateMachine.CreateLayer("main");
        
        var emptyState = mainLayer.CreateState("__CLEAR_TRACK__", 0, true);
        
        var appearState = mainLayer.CreateState("spike_shield/spike_shield_appear", 0, false);
        var idleState = mainLayer.CreateState("spike_shield/spike_shield_idle", 0, true);
        var hitState = mainLayer.CreateState("spike_shield/spike_shield_hit", 0, false);
        var disappearState = mainLayer.CreateState("spike_shield/spike_shield_disappear", 0, false);
        var breakState = mainLayer.CreateState("spike_shield/spike_shield_break", 0, false);

        var appearTrigger = stateMachine.CreateVariable("appear", StateMachineVariableKind.TRIGGER);
        var hitTrigger = stateMachine.CreateVariable("hit", StateMachineVariableKind.TRIGGER);
        var breakTrigger = stateMachine.CreateVariable("break", StateMachineVariableKind.TRIGGER);
        var disappearTrigger = stateMachine.CreateVariable("disappear", StateMachineVariableKind.TRIGGER);
        
        mainLayer.CreateGlobalTransition(appearState).CreateTriggerCondition(appearTrigger);
        mainLayer.CreateTransition(appearState, idleState, true);
        mainLayer.CreateGlobalTransition(breakState).CreateTriggerCondition(breakTrigger);
        mainLayer.CreateTransition(idleState, hitState, false).CreateTriggerCondition(hitTrigger);
        mainLayer.CreateTransition(hitState, idleState, true);
        mainLayer.CreateTransition(breakState, emptyState, true);
        mainLayer.CreateTransition(idleState, disappearState, false).CreateTriggerCondition(disappearTrigger);
        mainLayer.CreateTransition(disappearState, emptyState, true);
        
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
        if(!Broken) return;
        
        if (Util.OneTime(TimeElapsed > EntityLifeTime, ref LifeTimeEnded))
        {
            Despawn();
            //Log.Warn($"Entity {Entity.Name} Destroyed!");
        }

        TimeElapsed += Time.DeltaTime;
    }
}