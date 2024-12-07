using Assembly.scripts.SceneObjects;
using Assembly.scripts.VFX;

namespace Assembly.scripts.Effects.ActiveSkills;
using AO;

public class AbilityRage : FightAbility
{
    public override string SkillKey => "Rage";
    
    public override Type Effect => typeof(EffectRageCast);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    // Reduced 1s cooldown for each level
    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        int lv = int.Min(fp.GetSkillTree().GetSkillLevel("Rage"), 4);
        return EffectConfig.RageConfig.Cooldown - lv;
    }
}

public class EffectRageCast : FightEffectWithNoFlinch
{
    public override bool BlockAbilityActivation => !_casted; // Block during cast

    private float _animDuration = 1f;
    private float _rageDuration = 8f;
    private bool _casted;

    public override bool IsActiveEffect => true;

    protected override bool PreventMovement => false;

    public override float SpeedModifier => _casted ? 1 : 0;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.SetAnimTrigger("rage_stomp");
        FightPlayer.RegisterSpeedModify(this);
        
        _animDuration = MainLayer.GetCurrentStateLength();
        if (!isDropIn)
        {
            _rageDuration = EffectConfig.RageConfig.Duration;
            if (FightPlayer.GetSkillTree().GetSkillLevel("Rage") > 4)
            {
                _rageDuration += 2;
            }
            SoundId = SFX.Play(SFXKeys.RageAudio, DefaultSoundDesc);
            DurationRemaining = _animDuration + _rageDuration;
        }
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        
        if (Util.OneTime(ElapsedTime > _animDuration, ref _casted))
        {
            FightPlayer.GetAbility<AbilityRage>().AppliedEffect = EffectRage.CastOrExtendRage(FightPlayer, _rageDuration);
            
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.RemoveSpeedModify(this);
    }
}

public class EffectRage : FightEffect
{

    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => false;

    private AttachmentObject _aura;
    private Spine_Animator _auraAnimator;

    // General Atk boost buff
    protected EffectConfig.RageConfig Config;
    
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        AssignConfig(EffectConfig.RageConfig.GetDefault());
        
        FightPlayer.CurrentAttack += Config.AtkBoost;

        AddAura();
        
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.CurrentAttack -= Config.AtkBoost;
        _aura.Despawn();
    }

    protected void AssignConfig(EffectConfig.RageConfig cfg)
    {
        Config = cfg;
    }
    
    private void AddAura()
    {
        Prefab auraPrefab = VFXPrefabs.RageAura;
        _aura = auraPrefab.Instantiate().GetComponent<AttachmentObject>();
        _auraAnimator = _aura.Entity.GetComponent<Spine_Animator>();
        _aura.Spawn(FightPlayer.Entity,new Vector2(-0.3f, 0.9f), false, 9999);

        var auraFade = _aura.Entity.GetComponent<FadeAfterStart>();
        if (auraFade != null)
        {
            auraFade.SetPersistFadeTime(DurationRemaining-1f, DurationRemaining);
        }
        
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        _auraAnimator.LocalEnabled = FightPlayer.SpineAnimator.LocalEnabled;
    }
    
    public void Extend(float sec)
    {
        var auraFade = _aura.Entity.GetComponent<FadeAfterStart>();
        auraFade?.ExtendLifetime(sec);
        DurationRemaining += sec;
    }

    public static EffectRage CastOrExtendRage(FightPlayer fp, float duration)
    {
        EffectRage rg = fp.GetEffect<EffectRage>();
        if (rg.Alive())
        {
            rg.Extend(duration);
        }
        else
        {
            rg = fp.AddEffect<EffectRage>(fp, duration);
        }

        return rg;
    }
}