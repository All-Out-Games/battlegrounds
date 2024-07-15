using AO;
using System;
public class ZoneTeleporter : Component
{

    [Serialized] public Entity SpawnPoint;
    [Serialized] public Zone TeleportZone;

    [Serialized] public string TeleportText;
    [Serialized] public string ChangeStatusTo;

    protected Interactable InteractableComp;

    public override void Start()
    {
        var interactable = Entity.GetComponent<Interactable>();
        interactable.OnInteract += OnInteract;
        interactable.CanUseCallback = (Player p) =>
        {
            return true;
        };
        InteractableComp = interactable;

        var spineAnimator = Entity.GetComponent<Spine_Animator>();
        if (spineAnimator.Alive())
        {
            spineAnimator.SpineInstance.SetAnimation("idle_loop", true);
        }
    }
    

    public void OnInteract(Player p)
    {
        var player = (FightPlayer) p;
        if (Network.IsServer) 
        {
            PlayerStatus newStatus;
            bool parsed =
                Enum.TryParse(ChangeStatusTo, out newStatus);
            if (parsed)
            {
                // player.Teleport(SpawnPoint.Position); // Moved to the player function (following)
                player.CallClient_SwitchStatus((int)newStatus);
            }
        }
    }
}