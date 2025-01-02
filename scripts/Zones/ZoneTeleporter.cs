using AO;
using System;
using Assembly.scripts.Effects;

public class ZoneTeleporter : Component
{
    [Serialized] public string ChangeStatusTo;

    protected Interactable InteractableComp;
    protected string defaulTxt;

    public override void Awake()
    {
        base.Awake();
        InteractableComp = Entity.GetComponent<Interactable>();
        InteractableComp.OnInteract += OnInteract;
        InteractableComp.CanUseCallback = (Player p) =>
        {
            return !p.HasEffect<EffectSpectralSpawn>();
        };
        defaulTxt = InteractableComp.Text;

        var spineAnimator = Entity.GetComponent<Spine_Animator>();
        if (spineAnimator.Alive())
        {
            if (ChangeStatusTo == "Safe")
            {
                spineAnimator.SpineInstance.SetAnimation("idle", true);
            }
            else
            {
                spineAnimator.SpineInstance.SetAnimation("idle_loop", true);
            }
            
        }
    }


    public override void Update()
    {
        base.Update();
        if (Network.LocalPlayer != null && ChangeStatusTo == "Combat") // Changed in KoH to function as a respawn timer
        {
            var p = Network.LocalPlayer;
            var cd = p.GetEffect(typeof(EffectSafePortalCooldown));
            if (cd != null)
            {
                InteractableComp.Text = $"Respawn: {float.Round(cd.DurationRemaining, 0)}s";
                InteractableComp.HoldText = $"Respawn: {float.Round(cd.DurationRemaining, 0)}s";
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
        PlayerStatus newStatus;
        bool parsed =
            Enum.TryParse(ChangeStatusTo, out newStatus);
        if (Network.IsServer) 
        {
            if (parsed)
            {
                if (newStatus == PlayerStatus.Safe && player.HasEffect<EffectSafePortalCooldown>())
                {
                    return;
                }
                if (newStatus == PlayerStatus.Safe && player.PlayerStatus == PlayerStatus.Combat)
                {
                    player.GetEffectMgr().AddSafePortalCooldown(player.Entity, 60);
                }
                player.SwitchStatus((int)newStatus);
            }
        }
    }
    
}