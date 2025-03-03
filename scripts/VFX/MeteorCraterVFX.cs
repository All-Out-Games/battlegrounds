using AO;

namespace Assembly.scripts.VFX;

public class MeteorCraterVFX : BaseVFX
{
    [Serialized] private Spine_Animator CraterVFX;
    [Serialized] private Spine_Animator ExplosionVFX;

    private bool _shouldDisappear;
    public override void Awake()
    {
        //base.Awake();
        // Need a small state machine for the crater
        if (CraterVFX == null || ExplosionVFX == null)
        {
            Log.Error($"{Entity.Name} is not correctly setup!");
            Despawn();
            return;
        }

        var stateMachine = StateMachine.Make();
        CraterVFX.SpineInstance.SetStateMachine(stateMachine, Entity);

        var pLayer = stateMachine.CreateLayer("crater", 2);
        var appearState = pLayer.CreateState("appear", 0, false);
        var idleState = pLayer.CreateState("idle", 0, true);
        var disappearState = pLayer.CreateState("disappear", 0, false);
        pLayer.InitialState = appearState;

        var shouldDisappear = stateMachine.CreateVariable("shouldDisappear", StateMachineVariableKind.BOOLEAN);
        pLayer.CreateTransition(appearState, idleState, true);
        pLayer.CreateTransition(idleState, disappearState, false).CreateBoolCondition(shouldDisappear, true);
        
        stateMachine.SetBool("shouldDisappear", false);
        ExplosionVFX.SpineInstance.SetAnimation("meteor_crash", false);
    }

    public override void Update()
    {
        base.Update();
        if (Util.OneTime(LifeTime > 2.5f, ref _shouldDisappear))
        {
            CraterVFX.SpineInstance.StateMachine.SetBool("shouldDisappear", true);
        }
    }
}