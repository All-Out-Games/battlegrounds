using AO;

/// <summary>
/// Controller of the arena player
/// </summary>
public partial class FightPlayer
{
    // Create your state machine here. We are going to have a sh*t ton of animations so keep this section well documented.
    private void InitializeStateMachine()
    {
        var stateMachine = SpineAnimator.SpineInstance.StateMachine;
        // Main Layer
        var aoLayer = stateMachine.TryGetLayerByName("main");
        var aoIdleState = aoLayer.TryGetStateByName("Idle");
        var aoRunState = aoLayer.TryGetStateByName("Run_Fast");
        var aoMovingBool = stateMachine.TryGetVariableByName("moving");
        
        // AL Layer
        var fightLayer = stateMachine.CreateLayer("fight_layer", 10);
        var idleState = fightLayer.CreateState("BAT_003/neutralize_AL", 0, false);
        var emptyState = fightLayer.CreateState("__CLEAR_TRACK__", 0, true);
        fightLayer.SetInitialState(idleState);
        fightLayer.CreateTransition(idleState, emptyState, true);

        #region Basic Punch

        
        var punch1Trigger = stateMachine.CreateVariable("punch1", StateMachineVariableKind.TRIGGER);
        var punch1State = fightLayer.CreateState("BAT_003/punch_small_AL_mIK", 0, false);
        fightLayer.CreateGlobalTransition(punch1State).CreateTriggerCondition(punch1Trigger);
        fightLayer.CreateTransition(punch1State, idleState, true);
        
        var punch2Trigger = stateMachine.CreateVariable("punch2", StateMachineVariableKind.TRIGGER);
        var punch2State = fightLayer.CreateState("BAT_003/punch_strong_AL_mIK", 0, false);
        fightLayer.CreateGlobalTransition(punch2State).CreateTriggerCondition(punch2Trigger);
        fightLayer.CreateTransition(punch2State, idleState, true);
        
        var punch3Trigger = stateMachine.CreateVariable("punch3", StateMachineVariableKind.TRIGGER);
        var punch3State = fightLayer.CreateState("BAT_003/punch_strongest_AL_mIK", 0, false);
        fightLayer.CreateGlobalTransition(punch3State).CreateTriggerCondition(punch3Trigger);
        fightLayer.CreateTransition(punch3State, idleState, true);

        #endregion

        #region Brawler

        // ShoulderCrash
        var shoulderCrashTrigger = stateMachine.CreateVariable("shoulder_crash", StateMachineVariableKind.TRIGGER);
        var shoulderCrashEndTrigger = stateMachine.CreateVariable("shoulder_crash_end", StateMachineVariableKind.TRIGGER);
        
        var shoulderCrashState = aoLayer.CreateState("BAT_003/shoulder_crash_FX", 0, true);

        aoLayer.CreateGlobalTransition(shoulderCrashState).CreateTriggerCondition(shoulderCrashTrigger);
        aoLayer.CreateTransition(shoulderCrashState, aoIdleState, false).CreateTriggerCondition(shoulderCrashEndTrigger);
        
        // SelfDestruct
        var selfDestructTrigger = stateMachine.CreateVariable("self_destruct", StateMachineVariableKind.TRIGGER);

        var selfDestructState = aoLayer.CreateState("BAT_003/self_destruct", 0, false);

        aoLayer.CreateGlobalTransition(selfDestructState).CreateTriggerCondition(selfDestructTrigger);
        aoLayer.CreateTransition(selfDestructState, aoIdleState, true);

        // LeapSlam
        var leapSlamTrigger = stateMachine.CreateVariable("leapslam", StateMachineVariableKind.TRIGGER);
        var leapSlamState = aoLayer.CreateState("BAT_003/leaping_slam", 0, false);
        aoLayer.CreateTransition(leapSlamState, aoIdleState, true);
        aoLayer.CreateGlobalTransition(leapSlamState).CreateTriggerCondition(leapSlamTrigger);
        
        // LeapSlam - Victim (sent flying) [Also used for PsyThrow Victim]
        var sentFlyTrigger = stateMachine.CreateVariable("sentfly", StateMachineVariableKind.TRIGGER);
        var sentFlyRecoverTrigger = stateMachine.CreateVariable("sentfly_end", StateMachineVariableKind.TRIGGER);
        
        var sentFlyLoopState = aoLayer.CreateState("BAT_003/sent_flying_loop", 0, true);
        var sentFlyEndState = aoLayer.CreateState("BAT_003/sent_flying_land", 0, false);

        aoLayer.CreateGlobalTransition(sentFlyLoopState).CreateTriggerCondition(sentFlyTrigger);
        aoLayer.CreateTransition(sentFlyLoopState, sentFlyEndState, false).CreateTriggerCondition(sentFlyRecoverTrigger);
        aoLayer.CreateTransition(sentFlyEndState, aoIdleState, true);
        
        // Ground Stomp
        var groundStompTrigger = stateMachine.CreateVariable("groundstomp", StateMachineVariableKind.TRIGGER);
        var groundStompState = aoLayer.CreateState("BAT_003/ground_stomp", 0, false);
        aoLayer.CreateTransition(groundStompState, aoIdleState, true);
        aoLayer.CreateGlobalTransition(groundStompState).CreateTriggerCondition(groundStompTrigger);
        
        // BattleCry
        var battleCryTrigger = stateMachine.CreateVariable("battlecry", StateMachineVariableKind.TRIGGER);
        var battleCryState = aoLayer.CreateState("BAT_003/battle_cry", 0, false);
        aoLayer.CreateTransition(battleCryState, aoIdleState, true);
        aoLayer.CreateGlobalTransition(battleCryState).CreateTriggerCondition(battleCryTrigger);
        // Battle Cry victim
        var battleCryStunTrigger = stateMachine.CreateVariable("battlecry_stun", StateMachineVariableKind.TRIGGER);
        var battleCryStunState = aoLayer.CreateState("BAT_003/battle_cry_stunned", 0, true);
        aoLayer.CreateGlobalTransition(battleCryStunState).CreateTriggerCondition(battleCryStunTrigger);

        // ClawSlash
        var clawSlashTrigger = stateMachine.CreateVariable("clawslash", StateMachineVariableKind.TRIGGER);
        var clawSlashState = aoLayer.CreateState("BAT_003/claw_swipe_mIK", 0, false);
        aoLayer.CreateGlobalTransition(clawSlashState).CreateTriggerCondition(clawSlashTrigger);
        aoLayer.CreateTransition(clawSlashState, aoIdleState, true);
        
        // double punch
        var doublePunchTrigger = stateMachine.CreateVariable("doublepunch", StateMachineVariableKind.TRIGGER);
        var doublePunchState = fightLayer.CreateState("BAT_003/punch_double_AL_mIK", 0, false);
        fightLayer.CreateTransition(idleState, doublePunchState, false).CreateTriggerCondition(doublePunchTrigger);
        fightLayer.CreateTransition(doublePunchState, idleState, true);
        
        // Rage
        var rageTrigger = stateMachine.CreateVariable("rage_stomp", StateMachineVariableKind.TRIGGER);
        var rageState = aoLayer.CreateState("BAT_003/rage_stomp", 0, false);
        aoLayer.CreateGlobalTransition(rageState).CreateTriggerCondition(rageTrigger);
        aoLayer.CreateTransition(rageState, aoIdleState, true);
        #endregion

        #region Psionic

        // Confusion
        var confusionBool = stateMachine.CreateVariable("confusion", StateMachineVariableKind.BOOLEAN);
        var confusionRunState = aoLayer.CreateState("BAT_003/Run_confused", 0, true);
        var confusionIdleState = aoLayer.CreateState("BAT_003/idle_confused", 0, true);

        aoLayer.CreateTransition(aoRunState, confusionRunState, false).CreateBoolCondition(confusionBool, true);
        aoLayer.CreateTransition(confusionRunState, aoRunState, false).CreateBoolCondition(confusionBool, false);
        
        aoLayer.CreateTransition(confusionRunState, confusionIdleState, false).CreateBoolCondition(aoMovingBool, false);
        aoLayer.CreateTransition(confusionIdleState, confusionRunState, false).CreateBoolCondition(aoMovingBool, true);
        
        aoLayer.CreateTransition(aoIdleState, confusionIdleState, false).CreateBoolCondition(confusionBool, true);
        aoLayer.CreateTransition(confusionIdleState, aoIdleState, false).CreateBoolCondition(confusionBool, false);
        
        // Projectile Throw
        var throwTrigger = stateMachine.CreateVariable("throw", StateMachineVariableKind.TRIGGER);
        var throwState = fightLayer.CreateState("BAT_003/throw_weapon_AL_mIK", 0, false);
        fightLayer.CreateGlobalTransition(throwState).CreateTriggerCondition(throwTrigger);
        fightLayer.CreateTransition(throwState, idleState, true);
        
        // Confusion Ball Throw
        var befuddleTrigger = stateMachine.CreateVariable("befuddle_throw", StateMachineVariableKind.TRIGGER);
        var befuddleState = fightLayer.CreateState("BAT_003/confusion_ball_AL_mIK", 0, false);
        fightLayer.CreateGlobalTransition(befuddleState).CreateTriggerCondition(befuddleTrigger);
        fightLayer.CreateTransition(befuddleState, idleState, true);
        
        // Psybolt
        var psyboltTrigger = stateMachine.CreateVariable("psybolt", StateMachineVariableKind.TRIGGER);
        var psyboltState = fightLayer.CreateState("BAT_003/psybolt_AL", 0, false);
        fightLayer.CreateGlobalTransition(psyboltState).CreateTriggerCondition(psyboltTrigger);
        fightLayer.CreateTransition(psyboltState, idleState, true);
        
        // Hypnotize
        var hypnotizeTrigger = stateMachine.CreateVariable("hypnotize", StateMachineVariableKind.TRIGGER);
        var hypnotizeState = aoLayer.CreateState("BAT_003/hypnotize", 0, false);
        aoLayer.CreateGlobalTransition(hypnotizeState).CreateTriggerCondition(hypnotizeTrigger);
        aoLayer.CreateTransition(hypnotizeState, aoIdleState, true);
        
        // Psionic beam
        var psiBeamTrigger = stateMachine.CreateVariable("psibeam", StateMachineVariableKind.TRIGGER);
        var psiBeamEndTrigger = stateMachine.CreateVariable("psibeam_end", StateMachineVariableKind.TRIGGER);
        var psiBeamState = aoLayer.CreateState("BAT_003/psionic_beam_loop", 0, true);
        var psiBeamEndState = aoLayer.CreateState("BAT_003/psionic_beam_end", 0, false);

        aoLayer.CreateGlobalTransition(psiBeamState).CreateTriggerCondition(psiBeamTrigger);
        aoLayer.CreateTransition(psiBeamState, psiBeamEndState, false).CreateTriggerCondition(psiBeamEndTrigger);
        aoLayer.CreateTransition(psiBeamEndState, aoIdleState, true);
        
        // Self heal
        var selfHealTrigger = stateMachine.CreateVariable("selfheal", StateMachineVariableKind.TRIGGER);
        var selfHealEndTrigger = stateMachine.CreateVariable("selfheal_end", StateMachineVariableKind.TRIGGER);
        var selfHealStartState = aoLayer.CreateState("BAT_003/self_heal_start", 0, false);
        var selfHealLoopState = aoLayer.CreateState("BAT_003/self_heal_loop", 0, false);
        var selfHealEndState = aoLayer.CreateState("BAT_003/self_heal_end", 0, false);

        aoLayer.CreateGlobalTransition(selfHealStartState).CreateTriggerCondition(selfHealTrigger);
        aoLayer.CreateTransition(selfHealStartState, selfHealLoopState, true);
        aoLayer.CreateTransition(selfHealLoopState, selfHealEndState, false).CreateTriggerCondition(selfHealEndTrigger);
        aoLayer.CreateTransition(selfHealEndState, aoIdleState, true);
        
        // PsyThrow - Caster
        var psyThrowTrigger = stateMachine.CreateVariable("psythrow_attack", StateMachineVariableKind.TRIGGER);
        var psyThrowAttackTrigger =
            stateMachine.CreateVariable("psythrow_attack_throw", StateMachineVariableKind.TRIGGER);
        var psyThrowStartState = fightLayer.CreateState("BAT_003/psythrow_attack_start_AL", 0, false);
        var psyThrowHeldState = fightLayer.CreateState("BAT_003/psythrow_attack_loop_AL", 0, true);
        var psyThrowEndState = fightLayer.CreateState("BAT_003/psythrow_attack_throw_AL", 0, false);

        fightLayer.CreateTransition(psyThrowStartState, psyThrowEndState, false)
            .CreateTriggerCondition(psyThrowAttackTrigger);
        fightLayer.CreateTransition(psyThrowHeldState, psyThrowEndState, false)
            .CreateTriggerCondition(psyThrowAttackTrigger);
        fightLayer.CreateGlobalTransition(psyThrowStartState).CreateTriggerCondition(psyThrowTrigger);
        fightLayer.CreateTransition(psyThrowEndState, idleState, true);
        
        // PsyThrow - Victim
        var psyThrowGrabbedTrigger = stateMachine.CreateVariable("psythrow_grabbed", StateMachineVariableKind.TRIGGER);
        var psyThrowGrabbedState = aoLayer.CreateState("BAT_003/psythrow_victim_start", 0, false);
        var psyThrowGrabbedLoopState = aoLayer.CreateState("BAT_003/psythrow_victim_loop", 0, true);

        aoLayer.CreateGlobalTransition(psyThrowGrabbedState).CreateTriggerCondition(psyThrowGrabbedTrigger);
        aoLayer.CreateTransition(psyThrowGrabbedState, psyThrowGrabbedLoopState, true);

        aoLayer.CreateTransition(psyThrowGrabbedLoopState, aoIdleState, false).CreateTriggerCondition(sentFlyRecoverTrigger);

        #endregion


        // Backstab - Caster
        var backstabTrigger = stateMachine.CreateVariable("backstab", StateMachineVariableKind.TRIGGER);
        var backstabState = aoLayer.CreateState("BAT_003/backstab_attack", 0, false);
        aoLayer.CreateGlobalTransition(backstabState).CreateTriggerCondition(backstabTrigger);
        aoLayer.CreateTransition(backstabState, aoIdleState, true);
        
        // Backstab - Victim
        var backstabbedTrigger = stateMachine.CreateVariable("backstabbed", StateMachineVariableKind.TRIGGER);
        var backstabbedState = aoLayer.CreateState("BAT_003/backstab_victim", 0, false);
        aoLayer.CreateGlobalTransition(backstabbedState).CreateTriggerCondition(backstabbedTrigger);
        aoLayer.CreateTransition(backstabbedState, aoIdleState, true);
        
        // BearTrap - Victim
        var bearTrapTrigger = stateMachine.CreateVariable("beartrapped", StateMachineVariableKind.TRIGGER);
        var bearTrapState = aoLayer.CreateState("BAT_003/bear_trap_full", 0, false);
        aoLayer.CreateGlobalTransition(bearTrapState).CreateTriggerCondition(bearTrapTrigger);
        aoLayer.CreateTransition(bearTrapState, aoIdleState, true);
        
        
        // Rollout
        var rolloutStartTrigger = stateMachine.CreateVariable("rollout_start", StateMachineVariableKind.TRIGGER);
        var rolloutEndTrigger = stateMachine.CreateVariable("rollout_end", StateMachineVariableKind.TRIGGER);

        var rolloutStartState = aoLayer.CreateState("BAT_003/rollout_start", 0, false);
        var rolloutLoopState = aoLayer.CreateState("BAT_003/rollout_loop", 0, true);
        var rolloutEndState = aoLayer.CreateState("BAT_003/rollout_end", 0, false);

        aoLayer.CreateGlobalTransition(rolloutStartState).CreateTriggerCondition(rolloutStartTrigger);
        aoLayer.CreateTransition(rolloutStartState, rolloutLoopState, true);
        aoLayer.CreateTransition(rolloutLoopState, rolloutEndState, false).CreateTriggerCondition(rolloutEndTrigger);
        aoLayer.CreateTransition(rolloutEndState, aoIdleState, true);
        
        
        // Knocked Down (Used as placeholder for hypnotize)
        var knockDownTrigger = stateMachine.CreateVariable("knockdown", StateMachineVariableKind.TRIGGER);
        var knockDownRecoverTrigger = stateMachine.CreateVariable("knockdown_end", StateMachineVariableKind.TRIGGER);

        var knockDownStartState = aoLayer.CreateState("BAT_003/knocked_down", 0, false);
        var knockDownLoopState = aoLayer.CreateState("BAT_003/knocked_down_loop", 0, true);
        var knockDownEndState = aoLayer.CreateState("BAT_003/knocked_down_get_up", 0, false);
        aoLayer.CreateTransition(knockDownStartState, knockDownLoopState, true);
        aoLayer.CreateTransition(knockDownLoopState, knockDownEndState, false)
            .CreateTriggerCondition(knockDownRecoverTrigger);
        aoLayer.CreateTransition(knockDownEndState, aoIdleState, true);
        aoLayer.CreateGlobalTransition(knockDownStartState).CreateTriggerCondition(knockDownTrigger);
        

    }
    
    
    
    public void SetAnimTrigger(string variableName)
    {
        SpineAnimator.SpineInstance.StateMachine.SetTrigger(variableName);
    }

    public void UnsetAnimTrigger(string variableName)
    {
        SpineAnimator.SpineInstance.StateMachine.UnsetTrigger(variableName);
    }

    public void SetBonePosition(string bone, Vector2 pos)
    {
        SpineAnimator.SpineInstance.SetBonePosition(bone, pos);
    }
}