using AO;
using Assembly.scripts.Effects;
using Assembly.scripts.Effects.ActiveSkills;

namespace Assembly.scripts.SceneObjects.TriggersAndInteractions;

public partial class BearTrap : OwnedTrigger
{
    [Serialized] public bool Snapped;
    [Serialized] protected bool Armed;
    [Serialized] protected float TrapArmTime;
    
    protected override void OnOtherPlayerEnter(FightPlayer fp)
    {
        base.OnOtherPlayerEnter(fp);
        if (Network.IsServer)
        {
            if (fp.Alive() && fp.PlayerStatus == PlayerStatus.Combat)
            {
                CallClient_BearTrapSnap(fp);
            }
        }
    }

    [ClientRpc]
    public void BearTrapSnap(FightPlayer fp)
    {
        if (fp != Owner && fp.Alive())
        {
            if (fp.Damageable())
            {
                fp.AddEffect<EffectBearTrapSnare>(Owner);
            }
            Animator.SpineInstance.ColorMultiplier = new Vector4(1,1,1, 1);
            Animator.SpineInstance?.StateMachine?.SetTrigger("snap");
            Snapped = true;
            SFX.Play(SFXKeys.BearTrapSnapAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity});
        }
    }

    public override void Awake()
    {
        base.Awake();
        // Transparent when deployed. Display only for owner
        Vector4 curColor = Animator.SpineInstance.ColorMultiplier;
        curColor.W = 0;
        Animator.SpineInstance.ColorMultiplier = curColor;
        
        InitializeStateMachine();
    }

    public void InitializeStateMachine()
    {
        // Make a state machine.
        var stateMachine = StateMachine.Make();
        var mainLayer = stateMachine.CreateLayer("main");
        

        var appearSetUpState = mainLayer.CreateState("appear_set_up", 0, false);
        var idleState = mainLayer.CreateState("idle", 0, true);
        var snapCloseState = mainLayer.CreateState("snap_close", 0, false);
        var disappearState = mainLayer.CreateState("dissappear", 0, false);
        var disappearClosedState = mainLayer.CreateState("dissappear_closed", 0, false);
        var emptyState = mainLayer.CreateState("Null", 0, false);

        var snapTrigger = stateMachine.CreateVariable("snap", StateMachineVariableKind.TRIGGER);
        var disappearTrigger = stateMachine.CreateVariable("expire", StateMachineVariableKind.TRIGGER);

        mainLayer.SetInitialState(appearSetUpState);
        mainLayer.CreateTransition(appearSetUpState, idleState, true);
        mainLayer.CreateTransition(idleState, snapCloseState, false).CreateTriggerCondition(snapTrigger);
        mainLayer.CreateGlobalTransition(disappearState).CreateTriggerCondition(disappearTrigger);
        mainLayer.CreateTransition(snapCloseState, disappearClosedState, true);

        mainLayer.CreateTransition(disappearState, emptyState, true);
        mainLayer.CreateTransition(disappearClosedState, emptyState, true);
        
        Animator.SpineInstance.SetStateMachine(stateMachine, Entity);
        Animator.OnAnimationEnd += OnAnimationEnd;
        
        SFX.Play(SFXKeys.BearTrapSetAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity});
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Animator.OnAnimationEnd -= OnAnimationEnd;
    }

    public override void Update()
    {
        base.Update();
        if (Util.OneTime(TimeElapsed > TrapArmTime, ref Armed))
        {
            Log.Warn($"Trap Armed after {TrapArmTime}s! Position = {Position.ToString()}");
        }

        
    }

    public void OnAnimationEnd(string anim)
    {
        Log.Debug($"Animation End {anim}");
        if (anim == "dissappear" || anim == "dissappear_closed")
        {
            
            Despawn();
        }
        
    }

    protected override void OnLifeTimeRunOut()
    {
        Animator.SpineInstance.StateMachine.SetTrigger("expire");
        Snapped = true;
        Animator.SpineInstance.ColorMultiplier = new Vector4(1, 1, 1, 1);
    }

    protected override void OnEntityEnter(Entity entity)
    {
        if (!Armed || Snapped)
        {
            // When setup animation is not completed. Trap will not trigger.
            return;
        }
        base.OnEntityEnter(entity);
    }

    [ClientRpc]
    public override void Initialization(Entity owner, float lifeTime)
    {
        Awaken();
        base.Initialization(owner, lifeTime);
        TrapArmTime = EffectConfig.BearTrapConfig.TrapArmTime;
        Armed = false;
        Snapped = false;

        // Stealth for non local player
        if (Owner.IsLocal)
        {
            Vector4 curColor = Animator.SpineInstance.ColorMultiplier;
            curColor.W = 1;
            Animator.SpineInstance.ColorMultiplier = curColor;
        }
    }
    
    [ClientRpc]
    public void SetSize(float sizeMultiplier)
    {
        Entity.LocalScale = new Vector2(sizeMultiplier, sizeMultiplier);
    }
}