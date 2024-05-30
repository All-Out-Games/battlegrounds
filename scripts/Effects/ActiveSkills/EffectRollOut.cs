using AO;

public sealed partial class EffectRollOut : FightEffect
{
    private EffectConfig.RollOutConfig _config;
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => false;
    public override bool IsValidTarget => true;

    /// <summary>
    /// Call this function before adding the created Effect instance to the player!
    /// </summary>
    /// <param name="cfg"></param>
    public void AssignConfig(EffectConfig.RollOutConfig cfg)
    {
        _config = cfg;
    }
    
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        
        AssignConfig(EffectConfig.GetPlayerRollOutConfig(FightPlayer.CurrentAttack));
        
        DurationRemaining = _config.Duration;
        
        FightPlayer.AddSpeedModifier(_config.SpeedBuffMultiplier);
        FightPlayer.AddPlayerCollisionFunction(OnRolloutCollision);
    }



    public override void OnEffectEnd(bool interrupt)
    {
        FightPlayer.RemoveSpeedModifier(_config.SpeedBuffMultiplier);
        FightPlayer.RemovePlayerCollisionFunction(OnRolloutCollision);
    }
    
    
    

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
                FightPlayer.DamageReactionInfo info = new FightPlayer.DamageReactionInfo();
                otherPlayer.TakeDamage(_config.ContactDamage, FightPlayer, info);
            }

        }
    }
}