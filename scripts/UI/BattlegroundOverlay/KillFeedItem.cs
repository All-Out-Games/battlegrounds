using AO;

namespace Assembly.scripts.UI.BattlegroundOverlay;

public class KillFeedItem : Component
{
    [Serialized] public UIText SourceName;
    [Serialized] public UIText VictimName;
    [Serialized] public UIImage AbilityIcon;

    private BattlegroundOverlayWindow _parentPage;
    private float _timeElapsed;

    public void SetKillFeed(string sourceId, string victimId, string skillKey, BattlegroundOverlayWindow parent)
    {
        _parentPage = parent;
        AbilityIcon.Sprite = Assets.GetAsset<Texture>(SkillConfig.GetAbilityIconPath(skillKey));
        SourceName.Text = sourceId;
        VictimName.Text = victimId;
        
        string localplayerName = Network.LocalPlayer.Name;
        if (sourceId == localplayerName)
        {
            SourceName.Settings = SourceName.Settings with { Color = GlobalData.SelfIdColor };
        }
        else
        {
            SourceName.Settings = SourceName.Settings with { Color = GlobalData.OtherIdColor };
        }
        
        if (victimId == localplayerName)
        {
            VictimName.Settings = VictimName.Settings with { Color = GlobalData.SelfIdColor };
        }
        else
        {
            VictimName.Settings = VictimName.Settings with { Color = GlobalData.OtherIdColor };
        }
    }

    public override void Update()
    {
        base.Update();
        _timeElapsed += Time.DeltaTime;
        if (_timeElapsed > GlobalData.KillFeedLifeTime)
        {
            _parentPage.RemoveKillFeed(this);
        }
    }
}