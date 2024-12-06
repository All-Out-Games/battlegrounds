namespace Assembly.scripts.Effects.ActiveSkills;
using AO;

public class AbilityIceStorm : FightAbility
{
    public override string SkillKey => "IceStorm";

    public override Type Effect => typeof(EffectIceStorm);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    
    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        float cd = EffectConfig.IceStormConfig.Cooldown;
        int lv = fp.GetSkillTree().GetSkillLevel("IceStorm");
        if (lv > 3)
        {
            cd -= 2;
        }

        return cd;
    }
}

public class EffectIceStorm : FightEffectWithNoFlinch
{
    public override bool IsActiveEffect => true;

    public override bool BlockAbilityActivation => true;

    private bool _endAnimationPlayed;

    private EffectConfig.IceStormConfig _config;
    
    protected float NextDmgTick = 0.5f;
    protected bool Ticked = false;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        _config = EffectConfig.IceStormConfig.GetDefault(FightPlayer.CurrentAttack, FightPlayer.GetSkillTree().GetSkillLevel("IceStorm"));
        if (!isDropIn)
        {
            DurationRemaining = EffectConfig.IceStormConfig.EndAnimationTime + 1f;
        }
        FightPlayer.SetAnimTrigger("ice_storm_start");
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if(Util.OneTime(ElapsedTime > EffectConfig.IceStormConfig.EndAnimationTime, ref _endAnimationPlayed))
        {
            FightPlayer.SetAnimTrigger("ice_storm_end");
        }
        
        if (Util.OneTime(ElapsedTime > NextDmgTick, ref Ticked))
        {
            NextDmgTick += 1;
            Ticked = false;
            IceAttack();
        }
    }

    private void IceAttack()
    {
        Log.Warn("Wochao! Ice!");
    }
}