using AO;

public partial class EffectRollOut : FightEffect
{
    private EffectConfig.RollOutConfig _config;
    private string _skillSlotKey;
    private SkillSlot _skillSlot;

    public EffectRollOut()
    {
        IsActiveEffect = false;
        BlockAbilityActivation = false;
        IsValidTarget = true;
    }

    /// <summary>
    /// Call this function before adding the created Effect instance to the player!
    /// </summary>
    /// <param name="cfg"></param>
    public void AssignConfig(EffectConfig.RollOutConfig cfg, string slotKey)
    {
        _config = cfg;
        _skillSlotKey = slotKey;
    }
    
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        FightPlayer.AddSpeedModifier(_config.SpeedBuffMultiplier);
        FightPlayer.AddPlayerCollisionFunction(OnRolloutCollision);

        FightPlayerSkillSlotsManager slotsMgr = FightPlayer.GetSkillSlots();
        _skillSlot = slotsMgr.GetSkillSlots(_skillSlotKey);
        _skillSlot.SilentSlot(true);
    }



    public override void OnEffectEnd(bool interrupt)
    {
        FightPlayer.RemoveSpeedModifier(_config.SpeedBuffMultiplier);
        FightPlayer.RemovePlayerCollisionFunction(OnRolloutCollision);
        _skillSlot.SilentSlot(false);
        _skillSlot.ApplyCooldown(_config.Cooldown);
    }
    
    
    

    public override bool IsActiveEffect { get; }
    public override bool BlockAbilityActivation { get; }
    public override bool IsValidTarget { get; }

    protected void OnRolloutCollision(Entity other)
    {
        FightPlayer otherPlayer = other.GetComponent<FightPlayer>();
        if (otherPlayer != null)
        {
            if(otherPlayer.HasEffect<EffectNoMovement>())
            {
                return;
            }
            Vector2 bumpDir = other.Position - Entity.Position;
            var add = bumpDir * _config.BumpStrength;
            if (Network.IsServer)
            {
                otherPlayer.AddBumpFrom(FightPlayer, add, false);
                otherPlayer.TakeDamage(_config.ContactDamage);
            }

        }
    }
}