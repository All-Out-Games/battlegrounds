using AO;

namespace Assembly.scripts.SceneObjects.AdCrates;

public class AdTrigger : Component
{
    [Serialized] public Interactable Trigger;
    
    protected SyncVar<bool> _claimed = new SyncVar<bool>();
    
    public string RewardId = "None"; // TODO: These need to be synced with SyncVars. They got lost when reconnect
    public string AdPromptText = "Reward Config Not Found.";
    public string AdPromptTexturePath = FightAbility.DefaultIconPath;

    public bool Claimed
    {
        get => _claimed.Value;
        set
        {
            if (Network.IsServer)
            {
                _claimed.Set(value);
            }
        }
    }

    public override void Awake()
    {
        base.Awake();
        Trigger ??= GetComponent<Interactable>();
        if (!Trigger.Alive())
        {
            Log.Error($"Ad Trigger Entity {Entity.Name} does not have a Interactable attached!");
            Entity.Destroy();
        }
        Trigger.CanUseCallback += IsInteractable;
        Trigger.OnInteract += OnInteract;
    }

    public virtual bool IsInteractable(Player p)
    {
        return !Claimed;
    }
    public virtual void OnInteract(Player p)
    {
        if (Ads.IsAdAvailable())
        {
            Claimed = true;
            Ads.PromptAd(RewardId, "Watch Ad to Claim", AdPromptText, Assets.GetAsset<Texture>(AdPromptTexturePath));
        }
        else
        {
            if (p.IsLocal)
            {
                Notifications.Show("Ads are unavailable at this moment. They are only available on mobile.");
            }
        }
    }
}