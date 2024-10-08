using AO;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.SceneObjects.TriggersAndInteractions;
using Assembly.scripts.VFX;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilitySpikeShield : FightAbility
{
    public override string SkillKey => "GravityCrush";

    public override Type Effect => typeof(EffectSpikeShield);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    
    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        float cd = EffectConfig.SpikeShieldConfig.Cooldown;
        int lv = fp.GetSkillTree().GetSkillLevel("GravityCrush");
        if (lv > 1)
        {
            cd -= 1;
        }

        return cd;
    }
}

public class EffectGravityCrush : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => false;
    public override bool IsValidTarget => true;

    private GravityFieldVFX _gravityFieldVfx;
    // NOTE: The class GravityField.cs has an OnCollision Function that handles the slowdown of projectiles
    private GravityField _gravityField;

    private EffectConfig.GravityCrushConfig _config;


    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        
        AssignConfig(EffectConfig.GravityCrushConfig.GetDefault(FightPlayer.GetSkillTree().GetSkillLevel("GravityCrush")));

        AddGravityFx();
        if (!isDropIn)
        {
            DurationRemaining = _config.Lifetime;
            SoundId = SFX.Play(SFXKeys.SpikeShieldAudio, DefaultSoundDesc); 
        }
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        
    }

    public void AssignConfig(EffectConfig.GravityCrushConfig cfg)
    {
        _config = cfg;
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        if (_gravityField.Alive())
        {
            _gravityField.LocalEnabled = false;
        }
    }
    

    protected virtual void AddGravityFx()
    {
        var attachment = VFXPrefabs.SpikeShieldFx.Instantiate();
        _gravityFieldVfx = attachment.GetComponent<GravityFieldVFX>();
        _gravityFieldVfx.Spawn(FightPlayer.Entity, new Vector2(0, 0.22f), false, DurationRemaining+1.5f);
        _gravityFieldVfx.SetAnimTrigger("appear");
        _gravityField = attachment.GetComponent<GravityField>();
    }
}