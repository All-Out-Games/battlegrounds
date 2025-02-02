using Assembly.scripts.VFX;

namespace Assembly.scripts.SceneObjects.AdCrates;
using AO;

public partial class AdCrab : AdTrigger
{
    public static Prefab AdCrabPrefab = Assets.KeepLoaded<Prefab>("AdCrab.prefab");
    [Serialized] public Spine_Animator Animator;
    [Serialized] public FadeAfterStart Fade;
    [Serialized] public Vector2 Speed;
    
    public void ConstructStateMachine()
    {
        var stateMachine = StateMachine.Make();
        var mainLayer = stateMachine.CreateLayer("main");
        
        
        var moveState = mainLayer.CreateState("move", 0, true);
        var idleState = mainLayer.CreateState("idle", 0, true);
        var deadState = mainLayer.CreateState("death", 0, false);

        var deadTrigger = stateMachine.CreateVariable("death", StateMachineVariableKind.TRIGGER);
        
        mainLayer.CreateGlobalTransition(deadState).CreateTriggerCondition(deadTrigger);
        
        
        mainLayer.InitialState = moveState;
        Animator.SpineInstance.SetStateMachine(stateMachine, Entity);
        Animator.SpineInstance.SetSkin("normal");
        Animator.SpineInstance.EnableSkin("normal");
        Animator.SpineInstance.RefreshSkins();
    }
    
    public override void Awake()
    {
        base.Awake();
        if(!Fade.Alive()){
            Log.Warn("The Crate Component must come with a FadeAfterStart Component!");
            Entity.Destroy();
            return;
        }
        ConstructStateMachine();
        
        //Animator.SpineInstance.StateMachine.SetTrigger("appear");
        Fade.OnFaded += () =>
        {
            Fade.OnFaded = null;
            if(Network.IsServer) CallClient_Despawn();
        };

        if (Network.IsServer)
        {
            // Gives a random speed on server. Note that we checked network_position property to sync it to the clients.
            Speed = new Vector2(Random.Shared.NextFloat()-0.5f, Random.Shared.NextFloat()-0.5f).Normalized;
        }
    }

    /// <summary>
    /// Function for crate init over the network. Roll and assign the item in it, also sets attributes.
    /// CAUTION: DO NOT CALL THIS EXCEPT IN THE SPAWN ROUTINE
    /// </summary>
    [ClientRpc]
    public void Initialization(Vector4 tint, string rewardId, string promptText,string texturePath, string interactableText)
    {
        Fade.SetPersistFadeTime(GlobalData.AdCrabLifeTime, GlobalData.AdCrabLifeTime+1);
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

    public override void Update()
    {
        base.Update();
        if(Network.IsServer) Entity.Position += Speed * Time.DeltaTime;
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
        Animator.SpineInstance.StateMachine.SetTrigger("death");
        Fade.FadeImmediately();
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