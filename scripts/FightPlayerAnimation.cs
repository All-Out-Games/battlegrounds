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
        var aoMovingBool = SpineAnimator.SpineInstance.StateMachine.TryGetVariableByName("moving");
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

        var selfDestructState = aoLayer.CreateState("BAT_003/self_destruct", 0, false);

        aoLayer.CreateGlobalTransition(selfDestructState).CreateTriggerCondition(selfDestructTrigger);
        aoLayer.CreateTransition(selfDestructState, aoIdleState, true);
        
        // Confusion
        var confusionBool = SpineAnimator.SpineInstance.StateMachine.CreateVariable("confusion", StateMachineVariableKind.BOOLEAN);
        var confusionRunState = aoLayer.CreateState("BAT_003/Run_confused", 0, true);
        var confusionIdleState = aoLayer.CreateState("BAT_003/idle_confused", 0, true);
        
        // Backstab - Caster
        var backstabTrigger = SpineAnimator.SpineInstance.StateMachine.CreateVariable("backstab", StateMachineVariableKind.TRIGGER);
        var backstabState = aoLayer.CreateState("BAT_003/backstab_attack", 0, false);
        aoLayer.CreateGlobalTransition(backstabState).CreateTriggerCondition(backstabTrigger);
        aoLayer.CreateTransition(backstabState, aoIdleState, true);
        // Backstab - Victim
        var backstabbedTrigger = SpineAnimator.SpineInstance.StateMachine.CreateVariable("backstabbed", StateMachineVariableKind.TRIGGER);
        var backstabbedState = aoLayer.CreateState("BAT_003/backstab_victim", 0, false);
        
        // BearTrap - Victim
        var bearTrapTrigger = SpineAnimator.SpineInstance.StateMachine.CreateVariable("beartrapped", StateMachineVariableKind.TRIGGER);
        var bearTrapState = aoLayer.CreateState("BAT_003/bear_trap_full", 0, false);
        aoLayer.CreateGlobalTransition(bearTrapState).CreateTriggerCondition(bearTrapTrigger);
        aoLayer.CreateTransition(bearTrapState, aoIdleState, true);
        
        aoLayer.CreateGlobalTransition(backstabbedState).CreateTriggerCondition(backstabbedTrigger);
        aoLayer.CreateTransition(backstabbedState, aoIdleState, true);

        aoLayer.CreateTransition(aoRunState, confusionRunState, false).CreateBoolCondition(confusionBool, true);
        aoLayer.CreateTransition(confusionRunState, aoRunState, false).CreateBoolCondition(confusionBool, false);
        
        aoLayer.CreateTransition(confusionRunState, confusionIdleState, false).CreateBoolCondition(aoMovingBool, false);
        aoLayer.CreateTransition(confusionIdleState, confusionRunState, false).CreateBoolCondition(aoMovingBool, true);
        
        aoLayer.CreateTransition(aoIdleState, confusionIdleState, false).CreateBoolCondition(confusionBool, true);
        aoLayer.CreateTransition(confusionIdleState, aoIdleState, false).CreateBoolCondition(confusionBool, false);

    }
}