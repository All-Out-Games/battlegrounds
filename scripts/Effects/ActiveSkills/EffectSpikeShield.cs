using AO;
using Assembly.scripts.Effects;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.VFX;

public class AbilitySpikeShield : FightAbility
{
    public override string SkillKey => "SpikeShield";

    public override Type Effect => typeof(EffectSpikeShield);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    
    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        float cd = EffectConfig.SpikeShieldConfig.Cooldown;
        int lv = fp.GetSkillTree().GetSkillLevel("SpikeShield");
        if (lv > 1)
        {
            cd -= 1;
        }

        return cd;
    }
}
/// <summary>
/// Base class of a shield ability.
/// Shield abilities will overwrite each other. Only one of them may exist on a player.
/// </summary>
public class EffectSpikeShield : FightEffect
{
    protected EffectConfig.SpikeShieldConfig Config;
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => false;
    public override bool IsValidTarget => true;

    protected SpikeShieldVFX ShieldVfx;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        
        AssignConfig(EffectConfig.SpikeShieldConfig.GetDefault(FightPlayer.GetSkillTree().GetSkillLevel("SpikeShield")));
        FightPlayer.OnReceiveDamage += OnDamageEvent;

        AddShieldFx();
        if (!isDropIn)
        {
            DurationRemaining = Config.Lifetime;
            SoundId = SFX.Play(SFXKeys.SpikeShieldAudio, DefaultSoundDesc);
        }
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        ShieldVfx.Entity.LocalEnabled = FightPlayer.SpineAnimator.LocalEnabled;
    }

    public void AssignConfig(EffectConfig.SpikeShieldConfig cfg)
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
    
    
    protected override void OnDamageEvent(FightPlayer source, FightPlayer.DamageInfo info)
    {
        if (info.DmgType != DamageType.Heal)
        {
            ShieldVfx.SetAnimTrigger("hit");
        }
        if (info.DmgType == DamageType.Melee)
        {
            // deal damage to source
            FightPlayer.DamageInfo infoRef = info with { DamageNumberColor = GlobalData.DamageNumberColor, DmgType = DamageType.None, 
                AwardCoin = false, SkillKey = "SpikeShield"};
            infoRef.ReactionInfo.Amount = (int)float.Ceiling(info.ReactionInfo.Amount * Config.ReturnMultiplier);
            infoRef.ReactionInfo.Flinch = false;
            if (source != FightPlayer && source.Alive())
            {
                source.TakeDamage(FightPlayer, infoRef);
                EffectBleed.AddOrStackBleed(source, Player.Entity, EffectConfig.SpikeShieldConfig.BleedTime, 1);
                
            }
        }
    }

    protected virtual void AddShieldFx()
    {
        ShieldVfx = VFXPrefabs.SpikeShieldFx.Instantiate().GetComponent<SpikeShieldVFX>();
        ShieldVfx.Spawn(FightPlayer.Entity, new Vector2(0, 0.22f), false, DurationRemaining+1.5f);
        ShieldVfx.SetAnimTrigger("appear");
    }

}