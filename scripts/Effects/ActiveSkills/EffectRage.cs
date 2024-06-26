using Assembly.scripts.VFX;

namespace Assembly.scripts.Effects.ActiveSkills;
using AO;

public class AbilityRage : FightAbility
{
    public override string SkillKey => "Rage";
    
    public override Type Effect => typeof(EffectRage);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => EffectConfig.RageConfig.Cooldown;
}

public class EffectRage : FightEffect
{

    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => false;
    public override bool IsValidTarget => true;

    private AttachmentObject _aura;

    // General Atk boost buff
    protected EffectConfig.RageConfig Config;

    protected FightPlayerUI PlayerUI;
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        PlayerUI = FightPlayer.GetPlayerUIComp();
        AssignConfig(EffectConfig.RageConfig.GetDefault());
        
        
        FightPlayer.CurrentAttack += Config.AtkBoost;
        DurationRemaining = Config.Duration;
        
        AddAura();
        
    }

    public override void OnEffectEnd(bool interrupt)
    {
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
        PlayerUI = FightPlayer.GetPlayerUIComp();
        AddAura();
    }
    
    private void AddAura()
    {
        Prefab auraPrefab = VFXPrefabs.RageAura;
        _aura = auraPrefab.Instantiate().GetComponent<AttachmentObject>();
        _aura.Spawn(FightPlayer.Entity,new Vector2(0, -0.05f), false, DurationRemaining);
    }

}