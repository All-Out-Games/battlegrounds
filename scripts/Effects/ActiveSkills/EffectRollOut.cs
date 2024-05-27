using AO;

public sealed partial class EffectRollOut : FightEffect
{
    private EffectConfig.RollOutConfig _config;

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
        SlotKey = slotKey;
    }
    
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        FightPlayer.AddSpeedModifier(_config.SpeedBuffMultiplier);
        FightPlayer.AddPlayerCollisionFunction(OnRolloutCollision);
        
        SkillSlot.SilentSlot(true); // This skill is not in cooldown until the effect ends, so we need to silent it to prevent casting again.
    }



    public override void OnEffectEnd(bool interrupt)
    {
        FightPlayer.RemoveSpeedModifier(_config.SpeedBuffMultiplier);
        FightPlayer.RemovePlayerCollisionFunction(OnRolloutCollision);
        SkillSlot.SilentSlot(false);
        SkillSlot.ApplyCooldown(_config.Cooldown);
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