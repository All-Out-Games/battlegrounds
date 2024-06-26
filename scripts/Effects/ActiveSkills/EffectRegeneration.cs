using AO;
using Assembly.scripts.VFX;
using StreamReader = AO.StreamReader;

namespace Assembly.scripts.Effects.ActiveSkills;


public class AbilityRegeneration : FightAbility
{
    public override string SkillKey => "Regeneration";

    public override Type Effect => typeof(EffectRegeneration);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    
    public override float Cooldown => EffectConfig.RegenerateConfig.Cooldown;
}

public class EffectRegeneration : FightEffect
{
    public override bool IsActiveEffect => false;
    
    // Same as bleed but do heal instead of damage
    public int PerSecondHeal = 0;
    protected float NextDmgTick = 0;
    protected bool Ticked = false;
    protected FightPlayerUI PlayerUI;

    private AttachmentObject _aura;
    
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        
        PerSecondHeal = EffectConfig.RegenerateConfig.PerSecondHeal;
        DurationRemaining = EffectConfig.RegenerateConfig.HealTime;
        AddAura();
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        _aura.Despawn();
    }

    public override void OnEffectUpdate()
    {
        if (Util.OneTime(ElapsedTime > NextDmgTick, ref Ticked))
        {
            Regenerate();
            NextDmgTick += 1;
            Ticked = false;
        }
    }

    public override void NetworkDeserialize(StreamReader reader)
    {
        base.NetworkDeserialize(reader);
        AddAura();
    }

    private void Regenerate()
    {
        FightPlayer.DamageInfo selfHealInfo = FightPlayer.DamageInfo.CreateHealInfo(PerSecondHeal);
        FightPlayer.TakeDamage(FightPlayer, selfHealInfo); // You can create self damage that comes from other players.
    }

    private void AddAura()
    {
        Prefab auraPrefab = VFXPrefabs.RegenerationAura;
        _aura = auraPrefab.Instantiate().GetComponent<AttachmentObject>();
        _aura.Spawn(FightPlayer.Entity,new Vector2(0, -0.05f), false, DurationRemaining);
    }
}