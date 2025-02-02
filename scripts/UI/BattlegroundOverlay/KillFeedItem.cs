using AO;

namespace Assembly.scripts.UI.BattlegroundOverlay;

public class KillFeedItem : Component
{
    [Serialized] public UIText SourceName;
    [Serialized] public UIText VictimName;
    [Serialized] public UIImage AbilityIcon;
    [Serialized] public UIText LevelText;

    private BattlegroundOverlayWindow _parentPage;
    private float _timeElapsed;

    public void SetKillFeed(string sourceId, string victimId, string skillKey, BattlegroundOverlayWindow parent, int skillLv)
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

        LevelText.Text = $"Lv. {skillLv}";
        if (skillLv == 5)
        {
            LevelText.Settings = LevelText.Settings with {Color = GlobalData.CritNumberColor};
        }

        char last = skillKey.Last();
        if (skillKey[0] == 'P' && last == '3' | last == '2' | last == 'h') // Why not just Contain("punch")
        {
            LevelText.Entity.LocalEnabled = false;
        }
        else
        {
            LevelText.Entity.LocalEnabled = true;
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