using AO;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.VFX;
using StreamReader = AO.StreamReader;

public class AbilityShield : FightAbility
{
    public override string SkillKey => "Shield";

    public override Type Effect => typeof(EffectShield);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    
    public override float Cooldown => EffectConfig.ShieldConfig.Cooldown;
}
/// <summary>
/// Base class of a shield ability.
/// Shield abilities will overwrite each other. Only one of them may exist on a player.
/// </summary>
public class EffectShield : FightEffect
{
    protected EffectConfig.ShieldConfig Config;
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => false;
    public override bool IsValidTarget => true;

    protected ShieldVFX ShieldVfx;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        
        AssignConfig(EffectConfig.ShieldConfig.GetDefault());
        DurationRemaining = Config.Duration;
        
        FightPlayer.MaxShield = Config.ShieldAmt;
        FightPlayer.CurrentShield = Config.ShieldAmt;
        FightPlayer.OnReceiveDamage += OnDamageEvent;

        AddShieldFx();
        if (!isDropIn)
        {
            SoundId = SFX.Play(SFXKeys.WoodShieldAudio, DefaultSoundDesc);
        }
    }

    public void AssignConfig(EffectConfig.ShieldConfig cfg)
    {
        Config = cfg;
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        ShieldVfx.Broken = true;
        if (interrupt)
        {
            Log.Debug("Shield Premature Removal!");
            ShieldVfx.SetAnimTrigger("break");
        }
        else
        {
            ShieldVfx.SetAnimTrigger("disappear");
        }
        FightPlayer.CurrentShield = 0;
        FightPlayer.MaxShield = 0;
        FightPlayer.OnReceiveDamage -= OnDamageEvent;
    }

    public override void NetworkDeserialize(StreamReader reader)
    {
        base.NetworkDeserialize(reader);
        AddShieldFx();
        FightPlayer.OnReceiveDamage += OnDamageEvent;
    }
    
    protected override void OnDamageEvent(FightPlayer source, FightPlayer.DamageInfo info)
    {
        if (info.ReactionInfo.ShieldBroken)
        {
            FightPlayer.RemoveEffect<EffectShield>(true);
        }
        else
        {
            ShieldVfx.SetAnimTrigger("hit");
        }
    }

    protected virtual void AddShieldFx()
    {
        ShieldVfx = VFXPrefabs.ShieldFx.Instantiate().GetComponent<ShieldVFX>();
        ShieldVfx.Spawn(FightPlayer.Entity, new Vector2(0, 0.22f), false, 1);
        ShieldVfx.SetAnimTrigger("appear");
    }

}