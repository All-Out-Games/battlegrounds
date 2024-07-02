using AO;
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
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        AddInvis(FightPlayer.IsLocal);
        
        DurationRemaining = EffectConfig.InvisibilityConfig.InvisTime;

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
            FightPlayer.GetPlayerUIComp().AddPlayerUIInvisibleReason(_skillKey);
        }
        else
        {
            UIManager.Instance.SetPopup("You are invisible! Other players cannot see you", 1.5f, FightPlayer);
        }
        FightPlayer.AddNameInvisibilityReason(_skillKey);
    }

    private void RemoveInvis(bool local)
    {
        if (!local)
        {
            FightPlayer.RemoveInvisibilityReason(_skillKey);
            FightPlayer.GetPlayerUIComp().RemovePlayerUIInvisibleReason(_skillKey);
        }
        FightPlayer.RemoveNameInvisibilityReason(_skillKey);
    }
    
    
}