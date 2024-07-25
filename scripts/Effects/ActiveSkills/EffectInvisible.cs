using AO;
using Assembly.scripts.VFX;
using StreamReader = AO.StreamReader;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityInvisible : FightAbility
{
    public override string SkillKey => "Invisibility";

    public override Type Effect => typeof(EffectInvisible);
    public override bool MonitorEffectDuration => true;
    
    public override TargettingMode TargettingMode => TargettingMode.Self;
    public override float Cooldown => EffectConfig.InvisibilityConfig.Cooldown;
}

public class EffectInvisible : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool IsValidTarget => false;
    protected override int InterruptLevel => 1; // Interrupted by any damage or skill activation

    private string _skillKey = "Invisibility";

    private AttachmentObject _aura;
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        DurationRemaining = EffectConfig.InvisibilityConfig.InvisTime;
        AddInvis(FightPlayer.IsLocal);
        
        FightPlayer.OnSkillActivate += OnSkillActivationEvent;
        FightPlayer.OnReceiveDamage += OnDamageEvent;
    }

    public override void NetworkDeserialize(StreamReader reader)
    {
        base.NetworkDeserialize(reader);
        AddInvis(FightPlayer.IsLocal);
        
        FightPlayer.OnSkillActivate += OnSkillActivationEvent;
        FightPlayer.OnReceiveDamage += OnDamageEvent;
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        RemoveInvis(FightPlayer.IsLocal);
        
        FightPlayer.OnSkillActivate -= OnSkillActivationEvent;
        FightPlayer.OnReceiveDamage -= OnDamageEvent;
    }

    private void OnSkillActivationEvent(FightPlayer.SkillActivationInfo info)
    {
        if (info.SkillKey != _skillKey)
        {
            if (info.InterruptLevel >= InterruptLevel)
            {
                FightPlayer.RemoveEffect<EffectInvisible>(true);
            }
        }
    }

    private void AddInvis(bool local)
    {
        if (!local)
        {
            FightPlayer.AddInvisibilityReason(_skillKey);
            FightPlayer.AddNameInvisibilityReason(_skillKey);
            FightPlayer.GetPlayerUIComp().AddPlayerUIInvisibleReason(_skillKey);

            var invisFX = VFXPrefabs.InvisibilityVFX.Instantiate(); // This thing will despawn itself shortly after
            invisFX.Position = FightPlayer.Entity.Position;
        }
        else
        {
            AddLocalAura();
            UIManager.Instance.SetPopup("You are invisible! Other players cannot see you", 1.5f, FightPlayer);
        }
    }

    private void RemoveInvis(bool local)
    {
        if (!local)
        {
            FightPlayer.RemoveInvisibilityReason(_skillKey);
            FightPlayer.GetPlayerUIComp().RemovePlayerUIInvisibleReason(_skillKey);
            FightPlayer.RemoveNameInvisibilityReason(_skillKey);
        }
        else
        {
            _aura.Despawn();
        }
    }
    
    private void AddLocalAura()
    {
        Prefab auraPrefab = VFXPrefabs.InvisibilityAura;
        _aura = auraPrefab.Instantiate().GetComponent<AttachmentObject>();
        _aura.Spawn(FightPlayer.Entity,new Vector2(0f, -0.2f), false, DurationRemaining);

        var auraFade = _aura.Entity.GetComponent<FadeAfterStart>();
        if (auraFade != null)
        {
            auraFade.SetPersistFadeTime(DurationRemaining-1.0f, DurationRemaining-0.5f);
        }
    }
    
}