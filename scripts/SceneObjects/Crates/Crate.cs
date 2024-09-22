using System;
using AO;
using Assembly.scripts.VFX;

namespace Assembly.scripts.SceneObjects.Crates
{
    public partial class Crate : DamageableObject
    {
        public static Prefab CratePrefab = Assets.KeepLoaded<Prefab>("Crate.prefab");
        [Serialized] public Spine_Animator Animator;
        [Serialized] public int HitPoint = 1;

        [Serialized] public FadeAfterStart Fade;

        private bool _itemSpawned = false;

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
            Fade.SetPersistFadeTime(GlobalData.CrateLifeTime, GlobalData.CrateLifeTime+1);
            HitPoint = 2;
            CrateManager.Instance.Register(this);
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
            var c = CrateManager.Instance;
            c.Deregister(this);

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

        public override bool Damageable()
        {
            return HitPoint > 0 && !Fade.IsFading();
        }

        public override void TakeDamage(FightPlayer source, FightPlayer.DamageInfo info)
        {
            if (info.CrateImmediateDestroy)
            {
                HitPoint -= 114514;
            }
            else
            {
                HitPoint--;
            }
            
            // Hit animation
            Animator.SpineInstance.StateMachine.SetTrigger("hit");
            if (source.Position.X > Position.X)
            {
                Animator.SpineInstance.Scale = Animator.SpineInstance.Scale with { X = -1 }; // Flip the hit animation
            }
            else
            {
                Animator.SpineInstance.Scale = Animator.SpineInstance.Scale with { X = 1 };
            }
            
            if(HitPoint <= 0){
                if(Network.IsServer && !_itemSpawned) CallClient_CrateBreak();
                _itemSpawned = true;
            }
            else
            {
                Fade.ExtendLifetime(2f); // Extend time if a crate is damaged but not dead
            }
        }

        [ClientRpc]
        public void CrateBreak(){
            Animator.SpineInstance.StateMachine.SetTrigger("break");
            Fade.FadeImmediately();
            // TODO: Spawn Dropped Item
        }
    }
}