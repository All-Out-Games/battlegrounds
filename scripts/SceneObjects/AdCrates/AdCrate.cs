using AO;
using Assembly.scripts.VFX;
using StreamReader = AO.StreamReader;
using StreamWriter = AO.StreamWriter;

namespace Assembly.scripts.SceneObjects.AdCrates;

public partial class AdCrate : AdTrigger, INetworkedComponent
{
    [Serialized] public Spine_Animator Animator;

    [Serialized] public FadeAfterStart Fade;
    
    public static Prefab AdCratePrefab = Assets.KeepLoaded<Prefab>("AdCrate.prefab");

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
    }

    /// <summary>
    /// Function for crate init over the network. Roll and assign the item in it, also sets attributes.
    /// CAUTION: DO NOT CALL THIS EXCEPT IN THE SPAWN ROUTINE
    /// </summary>
    [ClientRpc]
    public void Initialization(Vector4 tint, string rewardId, string promptText,string texturePath, string interactableText)
    {
        Fade.SetPersistFadeTime(GlobalData.AdCrateLifeTime, GlobalData.AdCrateLifeTime+1);
        if (Network.IsClient)
        {
            SFX.Play(SFXKeys.CrateAppearAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity });
        }

        Animator.SpineInstance.ColorMultiplier = tint;
        RewardId = rewardId;
        AdPromptText = promptText;
        AdPromptTexturePath = texturePath;
        Trigger.Text = interactableText;
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

    public override void OnInteract(Player p)
    {
        base.OnInteract(p);
        if (Network.IsServer && Claimed)
        {
            CallClient_CrateBreak();
        }
    }
    
    [ClientRpc]
    public void CrateBreak(){
        Animator.SpineInstance.StateMachine.SetTrigger("break");
        Fade.FadeImmediately();
        if (Network.IsClient)
        {
            SFX.Play(SFXKeys.CrateBreakAudio, new SFX.PlaySoundDesc() { EntityToFollow = Entity });
        }
    }

    public new void NetworkDeserialize(StreamReader reader)
    {
        base.NetworkDeserialize(reader);
        Animator.SpineInstance.ColorMultiplier = reader.Read<Vector4>();
    }

    public new void NetworkSerialize(StreamWriter writer)
    {
        base.NetworkSerialize(writer);
        writer.Write<Vector4>(Animator.SpineInstance.ColorMultiplier);
    }
}