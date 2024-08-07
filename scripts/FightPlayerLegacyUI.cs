using AO;
using Assembly.scripts;
using Assembly.scripts.UI;

/// <summary>
/// This component manages world space player UI.
/// TODO: This component is to be retired after HealthBar becomes an Engine feature. [Previously Named FightPlayerUI]
/// </summary>
public class FightPlayerLegacyUI : FightPlayerComponent
{
    private static Texture BarBorder = Assets.KeepLoaded<Texture>("UI/Bars/border.png");
    private List<string> _hideUIReasons = new List<string>();

    public void AddPlayerUIInvisibleReason(string reason)
    {
        _hideUIReasons.Add(reason);
    }
    
    public void RemovePlayerUIInvisibleReason(string reason)
    {
        _hideUIReasons.Remove(reason);
    }
    public override void Update()
    {
        Rect healthRect;
        if (_player.CurrentHealth > 0 && _hideUIReasons.Count == 0)
        {
            healthRect = DrawHealthBar();
            if (_player.CurrentShield > 0)
            {
                DrawShieldBar(healthRect.Copy());
            }
        }

        if (_player.IsLocal && _player.PlayerStatus == PlayerStatus.Combat)
        {
            DrawDamageNumber();
        }

    }

    protected Rect DrawHealthBar()
    {
        var healthRect = UI.GetPlayerRect(_player);
        healthRect = healthRect.Grow(13, 50, 0, 50).Offset(0, 160);
        var borderRect = healthRect.Grow(4, 3, 4, 3);
        UI.PushLayer(-2);
        UI.Image(borderRect, BarBorder, Vector4.White, new UI.NineSlice());
        UI.Image(healthRect, null, Vector4.Black, new UI.NineSlice());

        var healthPercent = _player.CurrentHealth / (float)_player.MaxHealth;
        var healthPercentRect = healthRect.SubRect(0, 0, healthPercent, 1, 0, 0, 0, 0);
        UI.Image(healthPercentRect, null, Vector4.HSVLerp(Vector4.Red, Vector4.Green, healthPercent), new UI.NineSlice());
        //UIManager.Instance.SetPopup($"Shield - {_player.CurrentShield} Max shield - {_player.MaxShield}", 0.5f, _player);
        UI.PopLayer();
        return healthRect;
        
    }

    protected void DrawShieldBar(Rect healthRect)
    {
        
        Rect shieldRect = healthRect.Grow(-7, -5, 0, -5).Offset(0, 13);
        UI.Image(shieldRect, null, Vector4.Black, new UI.NineSlice());

        float shieldPercent = _player.CurrentShield / (float)_player.MaxShield;
        var shieldPercentRect = shieldRect.SubRect(0, 0, shieldPercent, 1, 0, 0, 0, 0);
        UI.Image(shieldPercentRect, null, Vector4.LightBlue);
    }

    protected void DrawDamageNumber()
    {
        using var _1 = UI.PUSH_CONTEXT(UI.Context.WORLD);
        using var _2 = UI.PUSH_LAYER(FightClubGameManager.DamageNumberLayer);

        var ts = new UI.TextSettings()
        {
            Font = UI.Fonts.BarlowBold,
            Size = 0.7f,
            Color = Vector4.White,
            DropShadowColor = new Vector4(0f, 0f, 0f, 1f),
            DropShadowOffset = new Vector2(0f, -3f),
            HorizontalAlignment = UI.HorizontalAlignment.Center,
            VerticalAlignment = UI.VerticalAlignment.Center,
            WordWrap = false,
            WordWrapOffset = 0,
            Outline = true,
            OutlineThickness = 3,
        };

        List<DamageNumbers> numbers = FightClubGameManager.Instance.ActiveDamageNumbers;
        for (int i = numbers.Count-1; i >= 0; i -= 1)
        {
            var result = numbers[i];
            result.T += Time.DeltaTime * 0.5f;
            if (result.T >= 1)
            {
                numbers.UnorderedRemoveAt(i);
                continue;
            }
            var pos = result.Position;
            pos.Y += AOMath.Lerp(0, 0.5f, Ease.OutQuart(result.T));
            var rect = new Rect(pos, pos);
            var color01 = Ease.FadeInAndOut(0.1f, 1, result.T);
            ts.Color = new Vector4(0, 0, 0, 0).LerpTo(result.Color, color01);
            UI.Text(rect, result.Text, ts);
        }
    }
    
}