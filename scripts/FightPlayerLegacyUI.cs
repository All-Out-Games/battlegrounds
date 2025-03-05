using AO;
using Assembly.Koh;
using Assembly.scripts;
using Assembly.scripts.UI;

/// <summary>
/// This component manages world space player UI.
/// TODO: This component is to be retired after HealthBar becomes an Engine feature. [Previously Named FightPlayerUI]
/// </summary>
public class FightPlayerLegacyUI : FightPlayerComponent
{
    private static Texture BarBorder = Assets.KeepLoaded<Texture>("UI/Bars/border.png", synchronous: false);

    private static Texture LvPlate =
        Assets.KeepLoaded<Texture>("UI/LargeMenuButtons/Large Menu Buttons/large_button.png", synchronous: false);
    private List<string> _hideUIReasons = new List<string>();
    public string ClassDisplayName = "";

    public void AddPlayerUIInvisibleReason(string reason)
    {
        _hideUIReasons.Add(reason);
    }
    
    public void RemovePlayerUIInvisibleReason(string reason)
    {
        _hideUIReasons.Remove(reason);
    }

    public void DrawUI()
    {
        Rect healthRect;
        if (_player.CurrentHealth > 0 && _hideUIReasons.Count == 0)
        {
            healthRect = DrawHealthBar();
            DrawKoHScore();
        }

        if (_player.IsLocal && _player.PlayerStatus != PlayerStatus.Safe)
        {
            DrawDamageNumber();
        }
    }

    protected Rect DrawHealthBar()
    {
        using var _1 = UI.PUSH_CONTEXT(UI.Context.WORLD);
        using var _2 = IM.PUSH_Z(_player.GetZOffset() - 0.0001f); // minus an epsilon so the health bar draws over the player
        using var _3 = UI.PUSH_SCALE_FACTOR(5.0f / 540.0f);
        var healthRect = _player.FinalNameRect.BottomCenterRect().Offset(0, -20);
        var levelRect = healthRect.Grow(20, 20, 20, 20).Offset(-70, 0);
        healthRect = healthRect.Grow(13, 50, 0, 50).Offset(0, 0);
        var borderRect = healthRect.Grow(4, 3, 4, 3);
        UI.Image(borderRect, BarBorder, Vector4.White, new UI.NineSlice());
        UI.Image(healthRect, null, Vector4.Black, new UI.NineSlice());
        UI.Image(levelRect, LvPlate, Vector4.White, new UI.NineSlice());
        UI.Text(levelRect.Offset(0,5), $"{_player.Level + 1}", UI.TextSettings.Default with {Size = 28, HorizontalAlignment = UI.HorizontalAlignment.Center});

        var healthPercent = _player.CurrentHealth / (float)_player.MaxHealth;
        var healthPercentRect = healthRect.SubRect(0, 0, healthPercent, 1, 0, 0, 0, 0);
        UI.Image(healthPercentRect, null, Vector4.HSVLerp(Vector4.Red, Vector4.Green, healthPercent), new UI.NineSlice());
        if (_player.CurrentShield > 0)
        {
            DrawShieldBar(healthRect);
        }
        DrawKohInfo(healthRect);
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

    protected void DrawKohInfo(Rect healthRect)
    {
        var ts = new UI.TextSettings()
        {
            Font = GlobalData.AsapBold,
            Size = 20,
            VerticalAlignment = UI.VerticalAlignment.Top,
            HorizontalAlignment = UI.HorizontalAlignment.Center,
            Color = Vector4.White,
            Outline = true,
            OutlineColor = Vector4.Black,
            DoAutofit = false,
            Offset = new Vector2(0, 5)
        };
        
        UI.Text(healthRect, $"{_player.CurrentHealth}", ts);
        Rect clsRect = healthRect.Offset(0, -20);
        UI.Text(clsRect, ClassDisplayName, ts);
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
            ts.Color = Vector4.Lerp(new Vector4(0, 0, 0, 0),result.Color, color01);
            UI.Text(rect, result.Text, ts);
        }
    }

    protected void DrawKoHScore()
    {
        var ts = new UI.TextSettings()
        {
            Font = UI.Fonts.BarlowBold,
            Size = 32,
            VerticalAlignment = UI.VerticalAlignment.Top,
            HorizontalAlignment = UI.HorizontalAlignment.Center,
            Color = Vector4.White,
            Outline = true,
            OutlineColor = Vector4.Black,
            DoAutofit = false
        };
        
        using var _1 = UI.PUSH_CONTEXT(UI.Context.WORLD);
        using var _2 = IM.PUSH_Z(_player.GetZOffset() - 0.0001f); // minus an epsilon so the health bar draws over the player
        using var _3 = UI.PUSH_SCALE_FACTOR(5.0f / 540.0f);
        var scoreBarRect = _player.FinalNameRect.BottomCenterRect().GrowLeft(80).GrowRight(80).GrowBottom(35).Offset(0, -200);
        //UI.Image(scoreBarRect, null, Vector4.White);
        var kingIconRect = scoreBarRect.CutLeft(50).FitAspect(1).Offset(-10, 0);
        //UI.Image(kingIconRect, null, Vector4.LightBlue);
        var scoreIconRect = scoreBarRect.CutLeft(50).FitAspect(1).Offset(25, 0);
        //UI.Image(scoreIconRect, null, Vector4.Green);
        var kingScoreRect = kingIconRect.GrowRight(50).Offset(15,0);
        var roundScoreRect = scoreBarRect.GrowRight(50).Offset(-5, 0);
        UI.Image(kingIconRect, _player.HasEffect<EffectKing>() ? KohGlobalData.Crown : KohGlobalData.CrownGrey, Vector4.White);
        UI.Image(scoreIconRect, KohGlobalData.Clash, Vector4.White);
        UI.Text(kingScoreRect, $"{_player.KingScore}", ts);
        UI.Text(roundScoreRect, $"{_player.RoundScore}",ts);

        
    }
    
}