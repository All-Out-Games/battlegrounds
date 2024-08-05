using AO;
using Assembly.scripts.VFX;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityIronSkin : FightAbility
{
    public override string SkillKey => "IronSkin";
    
    public override Type Effect => typeof(EffectIronSkin);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => EffectConfig.IronSkinConfig.Cooldown;
}

public class EffectIronSkin : FightEffect
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
        _aura.Spawn(FightPlayer.Entity,new Vector2(-0.3f, 0.9f), false, DurationRemaining);

        var auraFade = _aura.Entity.GetComponent<FadeAfterStart>();
        if (auraFade != null)
        {
            auraFade.SetPersistFadeTime(DurationRemaining-1.5f, DurationRemaining-0.75f);
        }
        
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if (FightPlayer.SpineAnimator.LocalEnabled)
        {
            _auraAnimator.LocalEnabled = true;
        }
        else
        {
            _auraAnimator.LocalEnabled = false;
        }
        
    }
}