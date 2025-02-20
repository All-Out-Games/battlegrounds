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
        [Serialized] private CratesConfig _config;

        private bool _itemSpawned = false;
        private Vector2 _damageDir;
        

        public override void Awake()
        {
            base.Awake();
            if(!Fade.Alive()){
                Log.Warn("The Crate Component must come with a FadeAfterStart Component!");
                Entity.Destroy();
                return;
            }
            ConstructStateMachine();
            
            Animator.SpineInstance.StateMachine.SetTrigger("appear");
            Fade.OnFaded += () =>
            {
                Fade.OnFaded = null;
                if(Network.IsServer) CallClient_Despawn();
            };
            CrateManager.Instance?.Register(this);
        }

        /// <summary>
        /// Function for crate init over the network. Roll and assign the item in it, also sets attributes.
        /// CAUTION: DO NOT CALL THIS EXCEPT IN THE SPAWN ROUTINE
        /// </summary>
        [ClientRpc]
        public void Initialization(int cfgIndex)
        {
            Fade.SetPersistFadeTime(GlobalData.CrateLifeTime, GlobalData.CrateLifeTime+1);
            _config = CratesConfig.AllPossibleItems[cfgIndex];
            HitPoint = _config.HitPoint;
            if (Network.IsClient)
            {
                SFX.Play(SFXKeys.CrateAppearAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity });
            }

            Animator.SpineInstance.ColorMultiplier = _config.Tint;
        }

        [ClientRpc]
        public void Despawn()
        {
            Log.Debug($"Despawn called for {Entity.Name}");
            if (Network.IsServer)
            {
                var c = CrateManager.Instance;
                c?.Deregister(this);
                
                Network.Despawn(Entity);
                Entity.Destroy();
            }
            
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

            mainLayer.InitialState = emptyState;
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

            _damageDir = Position - source.Position;
            // Hit animation
            Animator.SpineInstance.StateMachine.SetTrigger("hit");
            if (_damageDir.X < 0)
            {
                Animator.SpineInstance.Scale = Animator.SpineInstance.Scale with { X = -1 }; // Flip the hit animation
            }
            else
            {
                Animator.SpineInstance.Scale = Animator.SpineInstance.Scale with { X = 1 };
            }
            
            if(HitPoint <= 0){
                if(Network.IsServer && !_itemSpawned) CallClient_CrateBreak(_damageDir);
                _itemSpawned = true;
            }
            else
            {
                Fade.ExtendLifetime(2f); // Extend time if a crate is damaged but not dead
                if (Network.IsClient)
                {
                    SFX.Play(SFXKeys.CrateHitAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity });
                }
            }
        }

        [ClientRpc]
        public void CrateBreak(Vector2 damageDir){
            Animator.SpineInstance.StateMachine.SetTrigger("break");
            Fade.FadeImmediately();
            // Server authoritatively spawn the item dropped.
            // Pass the last damage direction to push against the direction of the player who broke them 
            if (_config.Special)
            {
                // Put special itemName cases here
            }
            else
            {
                FightClubGameManager.Instance.ServerSpawn(CrateItemDrop.DropPrefab, Position, entity =>
                {
                    CrateItemDrop drop = entity.GetComponent<CrateItemDrop>();
                    drop.CallClient_Initialization(_config.ItemName, damageDir.Normalized);
                });
            }

            if (Network.IsClient)
            {
                SFX.Play(SFXKeys.CrateBreakAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity });
            }
        }
    }
}