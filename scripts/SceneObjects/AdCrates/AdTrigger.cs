using AO;
using StreamReader = AO.StreamReader;
using StreamWriter = AO.StreamWriter;

namespace Assembly.scripts.SceneObjects.AdCrates;

public class AdTrigger : Component, INetworkedComponent
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
        if (Ads.IsRewardedAdLoaded())
        {
            Claimed = true;
            if (p.IsLocal)
            {
                Ads.PromptRewardedAd(RewardId, "Watch Ad to Claim", AdPromptText, Assets.GetAsset<Texture>(AdPromptTexturePath));
            }
        }
        else
        {
            if (p.IsLocal)
            {
                string msg = Game.IsMobile ? "Ad is temporarily unavailable. Try again later." : "Ad bonuses are only available on the iOS and Android app!";
                Notifications.Show(msg);
            }
        }
    }

    public void NetworkSerialize(StreamWriter writer)
    {
        writer.WriteString(RewardId);
        writer.WriteString(AdPromptText);
        writer.WriteString(AdPromptTexturePath);
    }

    public void NetworkDeserialize(StreamReader reader)
    {
        RewardId = reader.ReadString();
        AdPromptText = reader.ReadString();
        AdPromptTexturePath = reader.ReadString();
    }
}