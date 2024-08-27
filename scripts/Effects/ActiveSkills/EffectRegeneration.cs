using AO;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.VFX;
using StreamReader = AO.StreamReader;

namespace Assembly.scripts.Effects.ActiveSkills;


public class AbilityRegeneration : FightAbility
{
    public override string SkillKey => "Regeneration";

    public override Type Effect => typeof(EffectRegeneration);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    
    public override float Cooldown => GetCooldown(FightPlayer);
    
    public static float GetCooldown(FightPlayer fp)
    {
        int lv = int.Min(3, fp.GetSkillTree().GetSkillLevel("Regeneration")); // Reduce 1 cooldown for the first two level
        return EffectConfig.RegenerateConfig.Cooldown - lv + 1;
    }
}

public class EffectRegeneration : FightEffect
{
    public override bool IsActiveEffect => false;
    
    // Same as bleed but do heal instead of damage
    public int PerSecondHeal = 0;
    protected float NextDmgTick = 0;
    protected bool Ticked = false;

    private RegenerationVFX _aura;
    private EffectConfig.RegenerateConfig _config;
    private bool _boosted;
    
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        _config = EffectConfig.RegenerateConfig.GetDefault(FightPlayer.GetSkillTree().GetSkillLevel("Regeneration"));
        PerSecondHeal = EffectConfig.RegenerateConfig.PerSecondHeal;
        _boosted = _config.ProvideBoost;
        
        
        if (!isDropIn)
        {
            DurationRemaining = _config.RegenTime;
            SoundId = SFX.Play(SFXKeys.HealingLoopAudio, new SFX.PlaySoundDesc() { EntityToFollow = FightPlayer.Entity, RangeMultiplier = 0.5f});
        }
        
        AddAura();
        if (FightPlayer.HasSkill("Concentrate"))
        {
            PerSecondHeal += EffectConfig.RegenerateConfig.ConcentrateExtraHealth;
        }

        if (_boosted)
        {
            FightPlayer.AddSpeedModifier(1.03f);
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        _aura.SetAnimTrigger("disappear");
        SFX.Stop(SoundId);
        if (_boosted)
        {
            FightPlayer.RemoveSpeedModifier(1.03f);
        }
    }

    public override void OnEffectUpdate()
    {
        if (Util.OneTime(ElapsedTime > NextDmgTick, ref Ticked))
        {
            Regenerate();
            NextDmgTick += 1;
            Ticked = false;
        }
        _aura.Entity.LocalEnabled = FightPlayer.SpineAnimator.LocalEnabled;
    }
    
    private void Regenerate()
    {
        FightPlayer.DamageInfo selfHealInfo = FightPlayer.DamageInfo.CreateHealInfo(PerSecondHeal);
        FightPlayer.TakeDamage(FightPlayer, selfHealInfo);
    }

    private void AddAura()
    {
        Prefab auraPrefab = VFXPrefabs.RegenerationAura;
        _aura = auraPrefab.Instantiate().GetComponent<RegenerationVFX>();
        _aura.Spawn(FightPlayer.Entity,new Vector2(0, 0.05f), false, DurationRemaining + 1);
        _aura.SetAnimTrigger("appear");
    }
}