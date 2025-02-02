using AO;

namespace Assembly.scripts.UI.BattlegroundOverlay;

public class BattlegroundOverlayWindow : BaseUIWindow
{
    [Serialized] private Entity _killFeedScroll;

    private Prefab _killFeedElementPrefab = Assets.KeepLoaded<Prefab>("KillFeedElement.prefab");
    private static readonly int MaxKillFeed = 4;
    /// <summary>
    /// Hold 4 kill feed items. Pop the first one if more come in.
    /// </summary>
    public List<KillFeedItem> KillFeedList = new List<KillFeedItem>();

    public void RemoveKillFeed(KillFeedItem item)
    {
        KillFeedList.Remove(item);
        item.Entity.Destroy();
    }

    public void AddKillFeed(string sourceId, string victimId, string skillKey, int skillLv)
    {
        var kf = _killFeedElementPrefab.Instantiate();
        var kfComp =  kf.GetComponent<KillFeedItem>();    
        kfComp.SetKillFeed(sourceId, victimId, skillKey, this, skillLv);
        kf.SetParent(_killFeedScroll, false);
        if (KillFeedList.Count >= MaxKillFeed)
        {
            RemoveKillFeed(KillFeedList[0]);
        }
        KillFeedList.Add(kfComp);
    }
}