using AO;

/// <summary>
/// Controller of the arena player
/// </summary>
public partial class FightPlayer
{
    private void InitializeStateMachine()
    {
        var fightLayer = SpineAnimator.SpineInstance.StateMachine.CreateLayer("fight_layer", 10);
        var aoLayer = SpineAnimator.SpineInstance.StateMachine.TryGetLayerByName("main");
        var aoIdleState = aoLayer.TryGetStateByName("Idle");
        var aoRunState = aoLayer.TryGetStateByName("Run_Fast");
        var idleState = fightLayer.CreateState("__CLEAR_TRACK__", 0, true);
        fightLayer.SetInitialState(idleState);
        
        // ShoulderCrash
        var shoulderCrashTrigger = SpineAnimator.SpineInstance.StateMachine.CreateVariable("shoulder_crash", StateMachineVariableKind.TRIGGER);
        var shoulderCrashEndTrigger = SpineAnimator.SpineInstance.StateMachine.CreateVariable("shoulder_crash_end", StateMachineVariableKind.TRIGGER);
        
        var shoulderCrashState = aoLayer.CreateState("BAT_003/shoulder_crash_FX", 0, true);

        aoLayer.CreateGlobalTransition(shoulderCrashState).CreateTriggerCondition(shoulderCrashTrigger);
        aoLayer.CreateTransition(shoulderCrashState, aoIdleState, false).CreateTriggerCondition(shoulderCrashEndTrigger);
        
        // SelfDestruct
        var selfDestructTrigger = SpineAnimator.SpineInstance.StateMachine.CreateVariable("self_destruct", StateMachineVariableKind.TRIGGER);
        var selfDestructEndTrigger = SpineAnimator.SpineInstance.StateMachine.CreateVariable("self_destruct_end", StateMachineVariableKind.TRIGGER);

        var selfDestructState = aoLayer.CreateState("BAT_003/self_destruct", 0, false);

        aoLayer.CreateGlobalTransition(selfDestructState).CreateTriggerCondition(selfDestructTrigger);
        aoLayer.CreateTransition(selfDestructState, aoIdleState, true);

    }
}