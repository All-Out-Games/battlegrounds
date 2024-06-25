using AO;

/// <summary>
/// This component manages world space player UI.
/// 
/// </summary>
public class FightPlayerUI : FightPlayerComponent
{
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

    }

    protected Rect DrawHealthBar()
    {
        var healthRect = UI.GetPlayerRect(_player);
        healthRect = healthRect.Grow(13, 50, 0, 50).Offset(0, -85);

        UI.Image(healthRect, null, Vector4.Black, new UI.NineSlice());

        var healthPercent = _player.CurrentHealth / (float)_player.MaxHealth;
        var healthPercentRect = healthRect.SubRect(0, 0, healthPercent, 1, 0, 0, 0, 0);
        UI.Image(healthPercentRect, null, Vector4.HSVLerp(Vector4.Red, Vector4.Green, healthPercent), new UI.NineSlice());
        //UIManager.Instance.SetPopup($"Shield - {_player.CurrentShield} Max shield - {_player.MaxShield}", 0.5f, _player);
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

    #region Aura management

    /// <summary>
    /// Manage player aura here. Aura animations probably should go to the prefab not here.
    /// Note: Auras should be added/removed in Effect system, therefore no need to use RPCs.
    /// </summary>
    
    private Dictionary<string, Entity> _auraEntities = new Dictionary<string, Entity>();

    private Vector2 _auraOffset = new Vector2(0, -0.05f);
    
    public void AddAura(string prefabPath, float size)
    {
        if (_auraEntities.ContainsKey(prefabPath))
        {
            Log.Error("You cannot have two identical Auras!");
            return;
        }
        
        Entity aura = Assets.GetAsset<Prefab>(prefabPath).Instantiate();
        aura.SetParent(_player.Entity, false);
        aura.LocalPosition = _auraOffset;
        aura.LocalScale *= size;

        _auraEntities[prefabPath] = aura;
    }

    public void RemoveAura(string prefabPath)
    {
        if (!_auraEntities.ContainsKey(prefabPath))
        {
            Log.Error($"Aura to be removed {prefabPath} NOT FOUND");
            return;
        }

        _auraEntities.Remove(prefabPath, out Entity aura);
        aura.Destroy();
    }

    #endregion
    
}