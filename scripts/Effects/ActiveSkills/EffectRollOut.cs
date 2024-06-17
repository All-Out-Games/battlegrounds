using AO;
using StreamReader = AO.StreamReader;


public class AbilityRollOut : FightAbility
{
    public override string SkillKey => "RollOut";
    
    public override Type Effect => typeof(EffectRollOut);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => EffectConfig.RollOutConfig.Cooldown;
}

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
        
        AssignConfig(EffectConfig.RollOutConfig.GetDefault(FightPlayer.CurrentAttack));
        
        DurationRemaining = _config.Duration;
        
        FightPlayer.AddSpeedModifier(_config.SpeedBuffMultiplier);
        FightPlayer.AddPlayerCollisionFunction(OnRolloutCollision);

        if (FightPlayer.IsLocal)
        {
            UIManager.Instance.SetPopup("You are Rollin! Bump other players with extra speed!", 3f, FightPlayer);
        }
    }

    public override void NetworkDeserialize(StreamReader reader)
    {
        base.NetworkDeserialize(reader);
        //AssignConfig(EffectConfig.GetPlayerRollOutConfig(FightPlayer.CurrentAttack));
        AssignConfig(EffectConfig.RollOutConfig.GetDefault(FightPlayer.CurrentAttack));
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
            otherPlayer.AddBumpFrom(FightPlayer, add, false);
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.ContactDamage);
            otherPlayer.TakeDamage( FightPlayer, info);

        }
    }
}