using AO;
using Assembly.scripts;
using Assembly.scripts.Effects;
using Assembly.scripts.SceneObjects;
using StreamReader = AO.StreamReader;
using StreamWriter = AO.StreamWriter;


public class AbilityRollOut : FightAbility
{
    public override string SkillKey => "RollOut";
    
    public override Type Effect => typeof(EffectRollOut);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => EffectConfig.RollOutConfig.Cooldown;
}

public class AbilityRollOutCancel : FightAbility
{
    public override string SkillKey => "RollOutCancel";
    public override bool CanUse() => Player.HasEffect<EffectRollOut>();

    public override Type Effect => typeof(EffectRollOutCancel);

    public override TargettingMode TargettingMode => TargettingMode.Self;
    
    public override string SkillIconPath => "AbilityIcon_Merged/defense/rollout_cancel.png";
}

public class EffectRollOut : FightEffect
{
    private EffectConfig.RollOutConfig _config;
    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;
    protected override int InterruptLevel => FightPlayer.DamageInfo.StunInterruptLevel;

    private static readonly List<Type> Wl = new List<Type>() { typeof(AbilityRollOutCancel) };
    public override List<Type> AbilityWhitelist => Wl;
    private int _originalIndex = -1;
    
    
    protected float NextDmgTick = 1;
    protected bool Ticked = false;

    /// <summary>
    /// Call this function before adding the created Effect instance to the player!
    /// </summary>
    /// <param name="cfg"></param>
    public void AssignConfig(EffectConfig.RollOutConfig cfg)
    {
        _config = cfg;
    }
    
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);

        RollOutStart();
        FightPlayer.SetAnimTrigger("rollout_start");
        
        
        if (!isDropIn)
        {
            DurationRemaining = _config.Duration;
            SFX.Play(SFXKeys.RolloutStartAudio, DefaultSoundDesc);
            if (_config.ProvideShield)
            {
                FightPlayer.AddEffect<EffectOvershield>(FightPlayer, DurationRemaining, overshield => overshield.AssignConfig(EffectConfig.ShieldConfig.GetOvershield(10, DurationRemaining)));
            }
        }

        if (FightPlayer.IsLocal)
        {
            var slotsMgr = FightPlayer.GetSkillSlots();
            var f = typeof(AbilityRollOut);
            _originalIndex = slotsMgr.GetAbilityIndex(f);
            if (_originalIndex > 0)
            {
                slotsMgr.ReplaceSlot(_originalIndex, slotsMgr.GetAbilityInstance(typeof(AbilityRollOutCancel)));
            }
            else
            {
                Log.Error("Rollout: Skill Replacement Error! The player does not have the primary skill equipped.");
            }
        }
        
    }
    
    

    public override void NetworkSerialize(StreamWriter writer)
    {
        base.NetworkSerialize(writer);
        writer.Write(NextDmgTick);
    }

    public override void NetworkDeserialize(StreamReader reader)
    {
        base.NetworkDeserialize(reader);
        NextDmgTick = reader.Read<float>();
        //AssignConfig(EffectConfig.GetPlayerRollOutConfig(FightPlayer.CurrentAttack));
    }

    private void RollOutStart()
    {
        FightPlayer.UnsetAnimTrigger("rollout_end");
        AssignConfig(EffectConfig.RollOutConfig.GetDefault(FightPlayer.CurrentAttack, FightPlayer.GetSkillTree().GetSkillLevel("RollOut")));
        FightPlayer.AddSpeedModifier(_config.SpeedBuffMultiplier);
        
        // TODO: This collision is currently broken because the player collision entity will continuously trigger with the player itself and ignore others
        // It should be fixed when we add collision layers
        // FightPlayer.AddPlayerCollisionFunction(OnRolloutCollision);
        
        FightPlayer.OnReceiveDamage += OnDamageEvent;
        FightPlayer.RegisterPreDamageEvent(this);
        
        SoundId = SFX.Play(SFXKeys.RolloutLoopAudio, new SFX.PlaySoundDesc() { EntityToFollow = FightPlayer.Entity, Loop = true, LoopTimeout = 1+_config.Duration});
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if (Util.OneTime(ElapsedTime > NextDmgTick, ref Ticked))
        {
            foreach (var dmg in FightClubGameManager.Instance.OverlapCircleForDamageables(Entity.Position, 2, Player))
            {
                RolloutDamage(dmg);
            }
            NextDmgTick += 1;
            Ticked = false;
        }
    }


    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        
        FightPlayer.RemoveSpeedModifier(_config.SpeedBuffMultiplier);
        // FightPlayer.RemovePlayerCollisionFunction(OnRolloutCollision);
        
        FightPlayer.OnReceiveDamage -= OnDamageEvent;
        FightPlayer.RemovePreDamageEvent(this);
        if (interrupt)
        {
            FightPlayer.UnsetAnimTrigger("rollout_end");
        }
        else
        {
            FightPlayer.SetAnimTrigger("rollout_end");
            SFX.Play(SFXKeys.RolloutEndAudio, DefaultSoundDesc);
        }
        
        if (_originalIndex > 0 && FightPlayer.IsLocal)
        {
            var slotsMgr = FightPlayer.GetSkillSlots();
            slotsMgr.ReplaceSlot(_originalIndex, slotsMgr.GetAbilityInstance(typeof(AbilityRollOut)));
        }

        SFX.Stop(SoundId);
        
    }
    
    
    

    protected void OnRolloutCollision(Entity other)
    {
        Log.Debug($"Collide With {other.Name}");
        DamageableObject pcc = other.GetComponent<DamageableObject>();
        RolloutDamage(pcc);
    }

    protected void RolloutDamage(DamageableObject other)
    {
        if(!other.Damageable() || other.Entity == FightPlayer.Entity)
        {
            return;
        }
        FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.ContactDamage) with {InterruptLevel = FightPlayer.DamageInfo.KnockBackInterruptLevel};
        info.SkillKey = SkillConfig.RollOutNodeConfig.SkillKey;
        other.TakeDamage( FightPlayer, info);
        if (other is PlayerCollisionChild)
        {
            var fp = (PlayerCollisionChild)other;
            
            Vector2 bumpDir = other.Entity.Position - Entity.Position;
            var add = bumpDir.Normalized * _config.BumpStrength;
            fp.Player.AddBumpFrom(FightPlayer, add, false);
        }
    }

    public override void PreDamageMod(ref FightPlayer.DamageInfo info)
    {
        base.PreDamageMod(ref info);
        info.ReactionInfo.Flinch = false;
    }
    
}

public class EffectRollOutCancel : FightEffect
{
    public override bool IsActiveEffect => false;

    public override bool BlockAbilityActivation => true;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.RemoveEffect<EffectRollOut>(false);
        FightPlayer.RemoveEffect<EffectOvershield>(true);
        DurationRemaining = 0.2f;
    }
    
}