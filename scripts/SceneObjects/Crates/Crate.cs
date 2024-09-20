using System;
using AO;
using Assembly.scripts.VFX;

namespace Assembly.scripts.SceneObjects.Crates
{
    public partial class Crate : Component, IDamageable
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

        public override void Start()
        {
            base.Start();
            if (Network.IsServer)
            {
                CallClient_Initialization();
            }
            Animator.SpineInstance.StateMachine.SetTrigger("appear");

            Fade.OnFaded += () =>
            {
                Fade.OnFaded = null;
                if(Network.IsServer) CallClient_Despawn();
            };
        }

        /// <summary>
        /// Function for crate init over the network. Roll and assign the item in it, also sets attributes.
        /// CAUTION: DO NOT CALL THIS EXCEPT IN THE SPAWN ROUTINE
        /// </summary>
        [ClientRpc]
        public void Initialization()
        {
            CrateManager.Instance.AliveCrateCount++;
            Fade.SetPersistFadeTime(GlobalData.CrateLifeTime, GlobalData.CrateLifeTime+1);
        }

        [ClientRpc]
        public void Despawn()
        {
            Log.Debug($"Despawn called for {Entity.Name}");
            if (Network.IsServer)
            {
                Network.Despawn(Entity);
                Entity.Destroy();
            }
            CrateManager.Instance.AliveCrateCount--;
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