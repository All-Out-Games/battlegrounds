using System;
using AO;
using Assembly.scripts.VFX;

namespace Assembly.scripts.SceneObjects.Crates
{
    public class Crate : Component, IDamageable
    {
        public static Prefab CratePrefab = Assets.KeepLoaded<Prefab>("Crate.prefab");
        [Serialized] public Spine_Animator Animator;
        [Serialized] public int HitPoint = 1;

        [Serialized] public FadeAfterStart Fade;

        public override void Awake()
        {
            base.Awake();
            if(!Fade.Alive()){
                Log.Warn("The Crate Component must come with a FadeAfterStart Component!");
                Entity.Destroy();
                return;
            }
            ConstructStateMachine();
        }

        public void ConstructStateMachine()
        {
            var stateMachine = StateMachine.Make();
            var mainLayer = stateMachine.CreateLayer("main");
            
            var emptyState = mainLayer.CreateState("__CLEAR_TRACK__", 0, true);
            
            var appearState = mainLayer.CreateState("appear", 0, false);
            var idleState = mainLayer.CreateState("idle_loop", 0, true);
            var hitState = mainLayer.CreateState("hit", 0, false);
            var breakState = mainLayer.CreateState("break", 0, false);

            var appearTrigger = stateMachine.CreateVariable("appear", StateMachineVariableKind.TRIGGER);
            var hitTrigger = stateMachine.CreateVariable("hit", StateMachineVariableKind.TRIGGER);
            var breakTrigger = stateMachine.CreateVariable("break", StateMachineVariableKind.TRIGGER);
            
            mainLayer.CreateGlobalTransition(appearState).CreateTriggerCondition(appearTrigger);
            mainLayer.CreateTransition(appearState, idleState, true);
            mainLayer.CreateGlobalTransition(breakState).CreateTriggerCondition(breakTrigger);
            mainLayer.CreateTransition(idleState, hitState, false).CreateTriggerCondition(hitTrigger);
            mainLayer.CreateTransition(hitState, idleState, true);
            
            mainLayer.SetInitialState(emptyState);
            Animator.SpineInstance.SetStateMachine(stateMachine, Entity);
        }

        public bool Damageable()
        {
            return HitPoint > 0 && !Fade.IsFading();
        }

        public void TakeDamage(FightPlayer source, FightPlayer.DamageInfo info)
        {
            HitPoint --;
            if(HitPoint <= 0){
                CrateBreak();
            }
        }

        protected void CrateBreak(){
            // TODO: Spawn Dropped Item
        }
    }
}