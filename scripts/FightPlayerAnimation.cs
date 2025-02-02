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
        var resetTrigger = stateMachine.TryGetVariableByName("RESET");
        var resetALTrigger = stateMachine.CreateVariable("RESET_AL", StateMachineVariableKind.TRIGGER);
        // Main Layer
        var aoLayer = stateMachine.TryGetLayerByName("main");
        var aoIdleState = aoLayer.TryGetStateByName("Idle");
        var aoRunState = aoLayer.TryGetStateByName("Run_Fast");
        var aoMovingBool = stateMachine.TryGetVariableByName("moving");
        
        // KoH Win pose
        var superSchleem = aoLayer.CreateState("Emote/Super_Schleem", 0, false);
        var superSchleemBlue = aoLayer.CreateState("Emote/Super_Schleem_Blue", 0, false);
        aoLayer.CreateTransition(superSchleem, aoIdleState, true);
        aoLayer.CreateTransition(superSchleemBlue, aoIdleState, true);
        var winTrigger1 = stateMachine.CreateVariable("win1", StateMachineVariableKind.TRIGGER);
        var winTrigger2 = stateMachine.CreateVariable("win2", StateMachineVariableKind.TRIGGER);
        aoLayer.CreateGlobalTransition(superSchleem).CreateTriggerCondition(winTrigger1);
        aoLayer.CreateGlobalTransition(superSchleemBlue).CreateTriggerCondition(winTrigger2);
        
        // AL Layer
        var fightLayer = stateMachine.CreateLayer("fight_layer", 10);
        //var idleState = fightLayer.CreateState("BAT_003/Idle_short_AL", 0, false);
        //var emptyState = fightLayer.CreateState("__CLEAR_TRACK__", 0, true);
        var idleState = fightLayer.CreateState("__CLEAR_TRACK__", 0, false);

        fightLayer.InitialState = idleState;
        //fightLayer.CreateTransition(idleState, emptyState, true);
        fightLayer.CreateGlobalTransition(idleState).CreateTriggerCondition(resetALTrigger);

        #region Basic

        // Punches
        
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
        
        // Deaths
        var knockDeathTrigger = stateMachine.CreateVariable("death_knock", StateMachineVariableKind.TRIGGER);
        var knockDeathState = aoLayer.CreateState("BAT_003/death_knocked_down", 0, false);
        aoLayer.CreateGlobalTransition(knockDeathState).CreateTriggerCondition(knockDeathTrigger);
        
        var poofDeathTrigger = stateMachine.CreateVariable("death_poof", StateMachineVariableKind.TRIGGER);
        var poofDeathState = aoLayer.CreateState("BAT_003/death_poof", 0, false);
        aoLayer.CreateGlobalTransition(poofDeathState).CreateTriggerCondition(poofDeathTrigger);
        
        var swipedDeathTrigger = stateMachine.CreateVariable("death_swiped", StateMachineVariableKind.TRIGGER);
        var swipedDeathState = aoLayer.CreateState("BAT_003/death_swiped", 0, false);
        aoLayer.CreateGlobalTransition(swipedDeathState).CreateTriggerCondition(swipedDeathTrigger);
        
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

        var selfDestructState = fightLayer.CreateState("BAT_003/self_destruct_AL", 0, false);

        fightLayer.CreateGlobalTransition(selfDestructState).CreateTriggerCondition(selfDestructTrigger);
        fightLayer.CreateTransition(selfDestructState, idleState, true);

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
        fightLayer.CreateGlobalTransition(doublePunchState).CreateTriggerCondition(doublePunchTrigger);
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


        #region Stealth

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
        
        // BearTrap - Caster
        var placeTrapTrigger = stateMachine.CreateVariable("place_trap", StateMachineVariableKind.TRIGGER);
        var placeTrapState = fightLayer.CreateState("BAT_003/place_trap_AL", 0, false);
        fightLayer.CreateTransition(placeTrapState, idleState, true);
        fightLayer.CreateGlobalTransition(placeTrapState).CreateTriggerCondition(placeTrapTrigger);
        
        // BearTrap - Victim
        var bearTrapTrigger = stateMachine.CreateVariable("beartrapped", StateMachineVariableKind.TRIGGER);
        var bearTrapState = aoLayer.CreateState("BAT_003/bear_trap_full", 0, false);
        aoLayer.CreateGlobalTransition(bearTrapState).CreateTriggerCondition(bearTrapTrigger);
        aoLayer.CreateTransition(bearTrapState, aoIdleState, true);
        
        // Total Darkness
        var totalDarknessTrigger = stateMachine.CreateVariable("total_darkness", StateMachineVariableKind.TRIGGER);
        var totalDarknessALState = fightLayer.CreateState("BAT_003/total_darkness_AL", 0, false);
        fightLayer.CreateGlobalTransition(totalDarknessALState).CreateTriggerCondition(totalDarknessTrigger);
        fightLayer.CreateTransition(totalDarknessALState, idleState, true);

        #endregion


        #region Defensive

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
        aoLayer.CreateGlobalTransition(knockDownEndState).CreateTriggerCondition(knockDownRecoverTrigger);
        aoLayer.CreateTransition(knockDownEndState, aoIdleState, true);
        aoLayer.CreateGlobalTransition(knockDownStartState).CreateTriggerCondition(knockDownTrigger);
        
        // Parry states (start-loop-end / trigger)
        var parryStartTrigger = stateMachine.CreateVariable("parry_start", StateMachineVariableKind.TRIGGER);
        var parryEndTrigger = stateMachine.CreateVariable("parry_end", StateMachineVariableKind.TRIGGER);
        var parryAttackTrigger = stateMachine.CreateVariable("parry_attack", StateMachineVariableKind.TRIGGER);
        
        var parryStartState = aoLayer.CreateState("BAT_003/parry_start", 0, false);
        var parryLoopState = aoLayer.CreateState("BAT_003/parry_loop", 0, true);
        var parryEndState = aoLayer.CreateState("BAT_003/parry_end", 0, false);
        var parryAttackState = aoLayer.CreateState("BAT_003/parry_trigger", 0, false);
        
        aoLayer.CreateGlobalTransition(parryStartState).CreateTriggerCondition(parryStartTrigger);
        aoLayer.CreateGlobalTransition(parryAttackState).CreateTriggerCondition(parryAttackTrigger);
        aoLayer.CreateTransition(parryAttackState, aoIdleState, true);
        aoLayer.CreateTransition(parryStartState, parryLoopState, true);
        aoLayer.CreateTransition(parryLoopState, parryEndState, false).CreateTriggerCondition(parryEndTrigger);
        aoLayer.CreateTransition(parryEndState, aoIdleState, true);
        

        // Flash Of Steel
        var fosStartTrigger = stateMachine.CreateVariable("fos_start", StateMachineVariableKind.TRIGGER);
        var fosState = aoLayer.CreateState("Attack_Melee_2", 0, false);
        aoLayer.CreateGlobalTransition(fosState).CreateTriggerCondition(fosStartTrigger); // Need a RESET trigger to reset
        
        // FoS victim
        var fosVictimTrigger = stateMachine.CreateVariable("fos_victim", StateMachineVariableKind.TRIGGER);
        var fosVictimState = aoLayer.CreateState("Idle_Drowsy", 0, true);
        aoLayer.CreateGlobalTransition(fosVictimState).CreateTriggerCondition(fosVictimTrigger); // They'll get flinched and return idle at the end

        // Blade Frenzy
        var bladeFrenzyTrigger = stateMachine.CreateVariable("bf_start", StateMachineVariableKind.TRIGGER);
        var bladeFrenzyPullState = aoLayer.CreateState("BAT_003/parry_end", 0, false);
        aoLayer.CreateGlobalTransition(bladeFrenzyPullState).CreateTriggerCondition(bladeFrenzyTrigger);
        aoLayer.CreateTransition(bladeFrenzyPullState, aoIdleState, true);
        
        var katanaSlashTrigger = stateMachine.CreateVariable("bf_slash", StateMachineVariableKind.TRIGGER);
        var katanaState = fightLayer.CreateState("Attack_Melee_1_mIK_AL2", 0, false);
        fightLayer.CreateGlobalTransition(katanaState).CreateTriggerCondition(katanaSlashTrigger);
        fightLayer.CreateTransition(katanaState, idleState, true);

        var illusionSlashTrigger = stateMachine.CreateVariable("illusion_slash", StateMachineVariableKind.TRIGGER);
        var illusionSlashState = aoLayer.CreateState("Attack_Melee_3", 0, false);
        aoLayer.CreateGlobalTransition(illusionSlashState).CreateTriggerCondition(illusionSlashTrigger);
        aoLayer.CreateTransition(illusionSlashState, aoIdleState, true);
        
        // Blade Storm
        var bladeStormTrigger = stateMachine.CreateVariable("bladestorm_charge", StateMachineVariableKind.TRIGGER);
        var bladeStormSpinTrigger = stateMachine.CreateVariable("bladestorm_spin", StateMachineVariableKind.TRIGGER);
        var bladeStormChargeFailed = stateMachine.CreateVariable("bladestorm_fail", StateMachineVariableKind.BOOLEAN);
        var bladeStormSpinEnded = stateMachine.CreateVariable("bladestorm_spin_ended", StateMachineVariableKind.BOOLEAN);
        var bladeStormChargeState = aoLayer.CreateState("BAT_003/blade_storm_charge", 0, true);
        var bladeStormSpinState = aoLayer.CreateState("BAT_003/cleaver_spin", 0, true);
        aoLayer.CreateGlobalTransition(bladeStormChargeState).CreateTriggerCondition(bladeStormTrigger);
        aoLayer.CreateTransition(bladeStormChargeState, aoIdleState, false)
            .CreateBoolCondition(bladeStormChargeFailed, true);
        
        aoLayer.CreateTransition(bladeStormSpinState, aoIdleState, false)
            .CreateBoolCondition(bladeStormSpinEnded, true);

        aoLayer.CreateGlobalTransition(bladeStormSpinState).CreateTriggerCondition(bladeStormSpinTrigger);
            
        
        var bladeStormVictimTrigger =
            stateMachine.CreateVariable("bladestorm_victim", StateMachineVariableKind.TRIGGER);
        var bladeStormVictimState = aoLayer.CreateState("BAT_003/sent_flying_land", 0, false);
        aoLayer.CreateGlobalTransition(bladeStormVictimState).CreateTriggerCondition(bladeStormVictimTrigger);
        aoLayer.CreateTransition(bladeStormVictimState, aoIdleState, true);

        #endregion


        #region Elemental

        // Ice Fist
        var punchIceTrigger = stateMachine.CreateVariable("punch_ice", StateMachineVariableKind.TRIGGER);
        var punchIceState = fightLayer.CreateState("BAT_003/punch_ice_AL_mIK", 0, false);
        fightLayer.CreateGlobalTransition(punchIceState).CreateTriggerCondition(punchIceTrigger);
        fightLayer.CreateTransition(punchIceState, idleState, true);

        // Wind Punch
        var punchWindTrigger = stateMachine.CreateVariable("punch_wind", StateMachineVariableKind.TRIGGER);
        var punchWindState = fightLayer.CreateState("BAT_003/wind_punch_mIK_AL", 0, false);
        fightLayer.CreateGlobalTransition(punchWindState).CreateTriggerCondition(punchWindTrigger);
        fightLayer.CreateTransition(punchWindState, idleState, true);
        
        // Thunderbolt
        var summonThunderTrigger = stateMachine.CreateVariable("summon_thunder", StateMachineVariableKind.TRIGGER);
        var summonThunderState = aoLayer.CreateState("BAT_003/summon", 0, false);
        aoLayer.CreateGlobalTransition(summonThunderState).CreateTriggerCondition(summonThunderTrigger);
        aoLayer.CreateTransition(summonThunderState, aoIdleState, true);
        
        var shockedTrigger = stateMachine.CreateVariable("shocked_start", StateMachineVariableKind.TRIGGER);
        var shockedEndTrigger = stateMachine.CreateVariable("shocked_end", StateMachineVariableKind.TRIGGER);
        var shockedState = aoLayer.CreateState("BAT_003/snared_loop", 0, true);
        aoLayer.CreateGlobalTransition(shockedState).CreateTriggerCondition(shockedTrigger);
        aoLayer.CreateTransition(shockedState, aoIdleState, false).CreateTriggerCondition(shockedEndTrigger);

        // Ice storm
        var iceStormTrigger = stateMachine.CreateVariable("ice_storm_start", StateMachineVariableKind.TRIGGER);
        var iceStormEndTrigger = stateMachine.CreateVariable("ice_storm_end", StateMachineVariableKind.TRIGGER);
        
        var iceStormStartState = aoLayer.CreateState("BAT_003/ice_storm_start", 0, false);
        var iceStormLoopState = aoLayer.CreateState("BAT_003/ice_storm_loop", 0, true);
        var iceStormEndState = aoLayer.CreateState("BAT_003/ice_storm_end", 0, false);
        aoLayer.CreateGlobalTransition(iceStormStartState).CreateTriggerCondition(iceStormTrigger);
        aoLayer.CreateTransition(iceStormStartState, iceStormLoopState, true);
        aoLayer.CreateTransition(iceStormLoopState, iceStormEndState, false).CreateTriggerCondition(iceStormEndTrigger);
        aoLayer.CreateTransition(iceStormEndState, aoIdleState, true);
        
        // Meteor (TODO)
        var meteorLandTrigger = stateMachine.CreateVariable("meteor_land", StateMachineVariableKind.TRIGGER);
        var meteorLandState = aoLayer.CreateState("BAT_003/meteor_land", 0, false);
        aoLayer.CreateGlobalTransition(meteorLandState).CreateTriggerCondition(meteorLandTrigger);
        aoLayer.CreateTransition(meteorLandState, aoIdleState, true);


        #endregion
    }
    
    
    
    public void SetAnimTrigger(string variableName, bool resetAL = false)
    {
        if(resetAL) SpineAnimator.SpineInstance.StateMachine.SetTrigger("RESET_AL");
        SpineAnimator.SpineInstance.StateMachine.SetTrigger(variableName);
    }

    public void SetAnimBool(string variableName, bool val)
    {
        SpineAnimator.SpineInstance.StateMachine.SetBool(variableName, val);
    }

    public void SetAnimTriggerWithReset(string variableName, bool resetAL = false)
    {
        var machine = SpineAnimator.SpineInstance.StateMachine;
        machine.SetTrigger("RESET");
        if(resetAL) machine.SetTrigger("RESET_AL");
        machine.SetTrigger(variableName);
    }

    public void UnsetAnimTrigger(string variableName)
    {
        // This is now intrinsic behavior. To be removed
        // SpineAnimator.SpineInstance.StateMachine.UnsetTrigger(variableName);
    }

    public void SetBonePosition(string bone, Vector2 pos)
    {
        SpineAnimator.SpineInstance.SetBonePosition(bone, pos);
    }

    public void SetKatana(bool active)
    {
        if (active)
        {
            string katana = "weapons/katana/base";
            if (TotalEliminations > 10000)
            {
                katana = "weapons/katana/gold";
            }

            if (TotalEliminations > 30000)
            {
                katana = "weapons/katana/diamond";
            }
            SpineAnimator.SpineInstance.EnableSkin(katana);
        }
        else
        {
            SpineAnimator.SpineInstance.DisableSkin("weapons/katana/base");
            SpineAnimator.SpineInstance.DisableSkin("weapons/katana/gold");
            SpineAnimator.SpineInstance.DisableSkin("weapons/katana/diamond");
        }
        
        SpineAnimator.SpineInstance.RefreshSkins();
    }
}