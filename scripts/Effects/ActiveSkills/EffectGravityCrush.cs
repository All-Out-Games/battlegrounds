using AO;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilitySpikeShield : FightAbility
{
    public override string SkillKey => "GravityCrush";

    public override Type Effect => typeof(EffectSpikeShield);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    
    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        float cd = EffectConfig.SpikeShieldConfig.Cooldown;
        int lv = fp.GetSkillTree().GetSkillLevel("GravityCrush");
        if (lv > 1)
        {
            cd -= 1;
        }

        return cd;
    }
}

public class EffectGravityCrush : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => false;
    public override bool IsValidTarget => true;
    

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        
        AssignConfig(EffectConfig.SpikeShieldConfig.GetDefault(FightPlayer.GetSkillTree().GetSkillLevel("SpikeShield")));
        FightPlayer.OnReceiveDamage += OnDamageEvent;

        AddGravityFx();
        if (!isDropIn)
        {
            
        }
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        
    }

    public void AssignConfig(EffectConfig.SpikeShieldConfig cfg)
    {
        
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
    }
    

    protected virtual void AddGravityFx()
    {
        
    }
}