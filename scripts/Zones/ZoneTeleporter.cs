using AO;
using System;
using Assembly.scripts.Effects;

public class ZoneTeleporter : Component
{
    [Serialized] public string ChangeStatusTo;

    protected Interactable InteractableComp;
    protected string defaulTxt;

    public override void Start()
    {
        
        InteractableComp = Entity.GetComponent<Interactable>();
        InteractableComp.OnInteract += OnInteract;
        InteractableComp.CanUseCallback = (Player p) =>
        {
            return true;
        };
        defaulTxt = InteractableComp.Text;

        var spineAnimator = Entity.GetComponent<Spine_Animator>();
        if (spineAnimator.Alive())
        {
            spineAnimator.SpineInstance.SetAnimation("idle_loop", true);
        }
    }


    public override void Update()
    {
        base.Update();
        if (Network.LocalPlayer != null && ChangeStatusTo == "Safe")
        {
            var p = Network.LocalPlayer;
            var cd = p.GetEffect(typeof(EffectSafePortalCooldown));
            if (cd != null)
            {
                InteractableComp.Text = $"Cooldown: {float.Round(cd.DurationRemaining, 0)}s";
                InteractableComp.HoldText = $"Cooldown: {float.Round(cd.DurationRemaining, 0)}s";
            }
            else
            {
                InteractableComp.Text = defaulTxt;
                InteractableComp.HoldText = defaulTxt;
            }
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
                if (newStatus == PlayerStatus.Safe && player.HasEffect<EffectSafePortalCooldown>())
                {
                    return;
                }
                if (newStatus == PlayerStatus.Safe)
                {
                    player.GetEffectMgr().AddSafePortalCooldown(player.Entity, 60);
                }
                player.CallClient_SwitchStatus((int)newStatus);
            }
        }
    }
    
}