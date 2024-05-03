using AO;

public class FightPlayerUI : FightPlayerComponent
{
    public override void Update()
    {
        if (_player.CurrentHealth > 0)
        {
            DrawHealthBar();
            
        }
    }

    protected void DrawHealthBar()
    {
        var healthRect = UI.GetPlayerRect(_player);
        healthRect = healthRect.Grow(13, 50, 0, 50).Offset(0, -85);

        UI.Image(healthRect, null, Vector4.Black, new UI.NineSlice());

        var healthPercent = _player.CurrentHealth / (float)_player.MaxHealth;
        var healthPercentRect = healthRect.SubRect(0, 0, healthPercent, 1, 0, 0, 0, 0);
        UI.Image(healthPercentRect, null, Vector4.HSVLerp(Vector4.Red, Vector4.Green, healthPercent), new UI.NineSlice());

    }
}