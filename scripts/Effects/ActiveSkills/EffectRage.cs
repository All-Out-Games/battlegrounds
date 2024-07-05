using Assembly.scripts.VFX;

namespace Assembly.scripts.Effects.ActiveSkills;
using AO;

public class AbilityRage : FightAbility
{
    public override string SkillKey => "Rage";
    
    public override Type Effect => typeof(EffectRageCast);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => EffectConfig.RageConfig.Cooldown;
}

public class EffectRageCast : FightEffectWithNoFlinch
{
    public override bool BlockAbilityActivation => !_casted; // Block during cast

    private float _animDuration = 1f;
    private bool _casted;

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        FightPlayer.SetAnimTrigger("rage_stomp");
        
        _animDuration = MainLayer.GetCurrentStateLength();
        DurationRemaining = _animDuration + EffectConfig.RageConfig.Duration;
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if (Util.OneTime(ElapsedTime > _animDuration, ref _casted))
        {
            FightPlayer.AddEffect<EffectRage>(FightPlayer, EffectConfig.RageConfig.Duration);
        }
    }
    
}

public class EffectRage : FightEffect
{

    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => false;

    private AttachmentObject _aura;

    // General Atk boost buff
    protected EffectConfig.RageConfig Config;
    
    public override void OnEffectStart()
    {
        base.OnEffectStart();
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

    public override void NetworkDeserialize(StreamReader reader)
    {
        base.NetworkDeserialize(reader);
        AddAura();
    }
    
    private void AddAura()
    {
        Prefab auraPrefab = VFXPrefabs.RageAura;
        _aura = auraPrefab.Instantiate().GetComponent<AttachmentObject>();
        _aura.Spawn(FightPlayer.Entity,new Vector2(-0.3f, 0.9f), false, DurationRemaining);

        var auraFade = _aura.Entity.GetComponent<FadeAfterStart>();
        if (auraFade != null)
        {
            auraFade.SetPersistFadeTime(DurationRemaining-1.5f, DurationRemaining-0.75f);
        }
        
    }

}