using AO;
using Assembly.scripts;
using StreamReader = AO.StreamReader;


public class AbilityRollOut : FightAbility
{
    public override string SkillKey => "RollOut";
    
    public override Type Effect => typeof(EffectRollOut);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => EffectConfig.RollOutConfig.Cooldown;
}

public class EffectRollOut : FightEffect
{
    private EffectConfig.RollOutConfig _config;
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    protected override int InterruptLevel => FightPlayer.DamageInfo.StunInterruptLevel;

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
        
        RollOutStart();
        FightPlayer.SetAnimTrigger("rollout_start");
        
        DurationRemaining = _config.Duration;
        if (FightPlayer.IsLocal)
        {
            UIManager.Instance.SetPopup("You are Rollin! Bump other players with extra speed!", 3f, FightPlayer);
        }

        
    }

    public override void NetworkDeserialize(StreamReader reader)
    {
        base.NetworkDeserialize(reader);
        //AssignConfig(EffectConfig.GetPlayerRollOutConfig(FightPlayer.CurrentAttack));
        RollOutStart();
    }

    private void RollOutStart()
    {
        FightPlayer.UnsetAnimTrigger("rollout_end");
        AssignConfig(EffectConfig.RollOutConfig.GetDefault(FightPlayer.CurrentAttack));
        FightPlayer.AddSpeedModifier(_config.SpeedBuffMultiplier);
        FightPlayer.AddPlayerCollisionFunction(OnRolloutCollision);
        
        FightPlayer.OnReceiveDamage += OnDamageEvent;
        FightPlayer.RegisterPreDamageEvent(this);
    }


    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        
        FightPlayer.RemoveSpeedModifier(_config.SpeedBuffMultiplier);
        FightPlayer.RemovePlayerCollisionFunction(OnRolloutCollision);
        
        FightPlayer.OnReceiveDamage -= OnDamageEvent;
        FightPlayer.RemovePreDamageEvent(this);
        if (interrupt)
        {
            FightPlayer.UnsetAnimTrigger("rollout_end");
        }
        else
        {
            FightPlayer.SetAnimTrigger("rollout_end");
        }
        
    }
    
    
    

    protected void OnRolloutCollision(Entity other)
    {
        Log.Debug($"Collide With {other.Name}");
        PlayerCollisionChild pcc = other.GetComponent<PlayerCollisionChild>();
        FightPlayer otherPlayer = pcc?.Player;
        
        if (otherPlayer != null && otherPlayer.Damageable())
        {
            if(otherPlayer == FightPlayer || otherPlayer.HasEffect<EffectNoMovement>())
            {
                return;
            }
            Vector2 bumpDir = other.Position - Entity.Position;
            var add = bumpDir * _config.BumpStrength;
            otherPlayer.AddBumpFrom(FightPlayer, add, false);
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.ContactDamage) with {InterruptLevel = FightPlayer.DamageInfo.KnockBackInterruptLevel};
            otherPlayer.TakeDamage( FightPlayer, info);

        }
    }

    public override void PreDamageMod(ref FightPlayer.DamageInfo info)
    {
        base.PreDamageMod(ref info);
        info.ReactionInfo.Flinch = false;
    }
    
}