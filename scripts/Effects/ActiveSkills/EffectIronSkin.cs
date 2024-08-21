using AO;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.VFX;
using Microsoft.VisualBasic.CompilerServices;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityIronSkin : FightAbility
{
    public override string SkillKey => "IronSkin";
    
    public override Type Effect => typeof(EffectIronSkin);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        float cd = EffectConfig.IronSkinConfig.Cooldown;
        int lv = fp.GetSkillTree().GetSkillLevel("IronSkin");
        if (lv > 1)
        {
            cd -= 1;
        }

        return cd;
    }
}

public class EffectIronSkin : FightEffect
{

    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => false;

    private StatAuraVFX _aura;
    private Spine_Animator _auraAnimator;
    private bool _faded;

    private EffectConfig.IronSkinConfig _config;
    
    
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        _config = EffectConfig.IronSkinConfig.GetDefault(FightPlayer.GetSkillTree().GetSkillLevel("IronSkin"));
        FightPlayer.RegisterPreDamageEvent(this);
        if (!isDropIn)
        {
            DurationRemaining = _config.Duration;
            SoundId = SFX.Play(SFXKeys.IronAuradAudio, DefaultSoundDesc);
        }
        
        AddAura();
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        _aura.Despawn();
        FightPlayer.RemovePreDamageEvent(this);
    }
    
    
    private void AddAura()
    {
        Prefab auraPrefab = VFXPrefabs.StatAura;
        _aura = auraPrefab.Instantiate().GetComponent<StatAuraVFX>();
        _aura.SetSkin("defense", Vector4.Blue);
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

    public override void PreDamageMod(ref FightPlayer.DamageInfo info)
    {
        base.PreDamageMod(ref info);
        info.ReactionInfo.Amount = (int)float.Floor(info.ReactionInfo.Amount * _config.DmgModifer);
    }
}