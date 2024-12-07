using AO;
using Assembly.scripts.Effects;
using Assembly.scripts.Effects.ActiveSkills;
namespace Assembly.scripts.SceneObjects.TriggersAndInteractions;

public partial class ChargingStation : OwnedTrigger
{
    [Serialized] protected bool Armed;
    [Serialized] protected float TrapArmTime;

    private int _shieldAmount = EffectConfig.ChargingStationConfig.BaseShieldAmt;
    
    protected override void OnOtherPlayerEnter(FightPlayer fp)
    {
        base.OnOtherPlayerEnter(fp);
        if (Network.IsServer)
        {
            if (fp.Alive() && fp.PlayerStatus == PlayerStatus.Combat)
            {
                // Each player can only be charged once. This is handled in parent class
                CallClient_ChargePlayer(fp);
            }
        }
    }

    [ClientRpc]
    public void ChargePlayer(FightPlayer fp)
    {
        if (fp.Alive())
        {
            fp.AddEffect<EffectOvershield>(Owner, 5f, overshield => overshield.AssignConfig(EffectConfig.ShieldConfig.GetOvershield(_shieldAmount, 5f)));
            SFX.Play(SFXKeys.BearTrapSnapAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity});
            Animator.SpineInstance.StateMachine.SetTrigger("activate");
        }
    }

    public override void Awake()
    {
        base.Awake();
        
        InitializeStateMachine();
    }

    public void InitializeStateMachine()
    {
        // Make a state machine.
        var stateMachine = StateMachine.Make();
        var mainLayer = stateMachine.CreateLayer("main");
        

        var appearSetUpState = mainLayer.CreateState("appear_set_up", 0, false);
        var idleState = mainLayer.CreateState("idle", 0, true);
        var disappearState = mainLayer.CreateState("dissappear", 0, false);
        var activateState = mainLayer.CreateState("activated", 0, false);
        var emptyState = mainLayer.CreateState("Null", 0, false);
        
        var disappearTrigger = stateMachine.CreateVariable("expire", StateMachineVariableKind.TRIGGER);
        var activateTrigger = stateMachine.CreateVariable("activate", StateMachineVariableKind.TRIGGER);

        mainLayer.SetInitialState(appearSetUpState);
        mainLayer.CreateTransition(appearSetUpState, idleState, true);
        mainLayer.CreateGlobalTransition(disappearState).CreateTriggerCondition(disappearTrigger);
        mainLayer.CreateTransition(idleState, activateState, false).CreateTriggerCondition(activateTrigger);
        mainLayer.CreateTransition(activateState, idleState, true);

        mainLayer.CreateTransition(disappearState, emptyState, true);
        
        Animator.SpineInstance.SetStateMachine(stateMachine, Entity);
        
        SFX.Play(SFXKeys.BearTrapSetAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity});
    }
    
    public override void Update()
    {
        base.Update();
        if (Util.OneTime(TimeElapsed > TrapArmTime, ref Armed))
        {
            Log.Warn($"Trap Armed after {TrapArmTime}s! Position = {Position.ToString()}");
        }
    }
    

    protected override void OnLifeTimeRunOut()
    {
        Animator.SpineInstance.StateMachine.SetTrigger("expire");
        Armed = false;
        // Charging Station despawn is handled by Effect
    }

    protected override void OnEntityEnter(Entity entity)
    {
        if (!Armed)
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
        TrapArmTime = EffectConfig.BearTrapConfig.TrapArmTime; // Same as bear trap (not likely to be changed)
        Armed = false;
    }
    
    [ClientRpc]
    public void SetShield(int shield)
    {
        _shieldAmount = shield;
    }
}