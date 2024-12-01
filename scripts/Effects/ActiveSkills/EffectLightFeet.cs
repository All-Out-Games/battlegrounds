using AO;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.VFX;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityLightFeet : FightAbility
{
    public override string SkillKey => "LightFeet";
    
    public override Type Effect => typeof(EffectLightFeet);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => EffectConfig.LightFeetConfig.Cooldown;
}

public class EffectLightFeet : FightEffect
{
    public override bool IsActiveEffect => false;
    private EffectConfig.LightFeetConfig _cfg;

    private StatAuraVFX _aura;
    private Spine_Animator _auraAnimator;
    private bool _faded;

    public override float SpeedModifier => _cfg.SpeedMtp;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        int lv = FightPlayer.GetSkillTree().GetSkillLevel("LightFeet");
        _cfg = EffectConfig.LightFeetConfig.GetDefault(lv);
        // Log.Error($"Level = {FightPlayer.GetSkillTree().GetSkillLevel("LightFeet")}");
        if (!isDropIn)
        {
            DurationRemaining = _cfg.BuffTime;
            SoundId = SFX.Play(SFXKeys.LightFeetAudio, DefaultSoundDesc);
        }
        FightPlayer.RegisterSpeedModify(this);
        AddAura();
        if (lv > 4)
        {
            FightPlayer.AddEffect<EffectDodge>(FightPlayer, DurationRemaining, dodge => dodge.DodgeChance = 0.1f);
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.RemoveSpeedModify(this);
    }
    
    private void AddAura()
    {
        Prefab auraPrefab = VFXPrefabs.StatAura;
        _aura = auraPrefab.Instantiate().GetComponent<StatAuraVFX>();
        _aura.SetSkin("speed", new Vector4(0.95f, 0.95f, 0, 1));
        _aura.SetAnimTrigger("appear");
        _auraAnimator = _aura.Animator;
        _aura.Spawn(FightPlayer.Entity,new Vector2(0f, 0.2f), false, DurationRemaining);
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

        if (Util.OneTime(DurationRemaining < 1, ref _faded))
        {
            _aura.SetAnimTrigger("disappear");
        }
        
    }
}