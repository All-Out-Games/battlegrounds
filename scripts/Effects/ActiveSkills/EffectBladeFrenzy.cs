namespace Assembly.scripts.Effects.ActiveSkills;
using AO;

public class AbilityBladeFrenzy : FightAbility
{
    public override string SkillKey => "BladeFrenzy";

    public override Type Effect => typeof(EffectBladeFrenzy);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        return fp.GetSkillTree().GetSkillLevel("BladeFrenzy") > 1 ? EffectConfig.BladeFrenzyConfig.Cooldown - 1 : EffectConfig.BladeFrenzyConfig.Cooldown;
    }
}

public class AbilityKatanaSlash : FightAbility
{
    public override string SkillKey => "KatanaSlash";

    public override Type Effect => typeof(EffectKatanaSlash);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    public override float MaxDistance => 2f;
    public override bool CanUse()
    {
        return FightPlayer.SkillCastGeneralCheck() && Player.HasEffect<EffectBladeFrenzy>();
    }

    public override string SkillIconPath => "AbilityIcon_Separate/defense/blade_attack.png";
}

// TODO Ability Illusion Slash

public class EffectBladeFrenzy : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => false;
    public EffectConfig.BladeFrenzyConfig Config;
    
    // Replace punch with katana slash
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        Config = EffectConfig.BladeFrenzyConfig.GetDefault(FightPlayer.CurrentAttack,FightPlayer.GetSkillTree().GetSkillLevel("BladeFrenzy"));
        if (!isDropIn)
        {
            DurationRemaining = Config.Duration;
        }

        
        FightPlayer.SetKatana(true);
        FightPlayer.SetAnimTrigger("bf_start");

        if (FightPlayer.IsLocal)
        {
            // Replace skills and stuff
            FightPlayer.GetSkillSlots().ReplaceSlot(0, FightPlayer.GetFightAbility<AbilityKatanaSlash>(), 0.25f);
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);

        if (FightPlayer.IsLocal)
        {
            FightPlayer.GetSkillSlots().ReplaceSlot(0, FightPlayer.GetFightAbility<AbilityPunch>());
        }
        FightPlayer.SetKatana(false);
    }

    
}

public class EffectKatanaSlash : FightEffect
{
    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;

    private bool _slashed;
    private Vector2 _slashDir;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        DurationRemaining = EffectConfig.BladeFrenzyConfig.DefaultSlashAnimationTime;
    }

    private void Slash()
    {
        _slashDir = FightPlayer.GetPunchDirection();
        FightPlayer.SetAimTarget(Entity.Position + _slashDir);
        FightPlayer.SetAnimTrigger("bf_slash");
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if (Util.OneTime(ElapsedTime > EffectConfig.BladeFrenzyConfig.DefaultSlashActivationTime, ref _slashed))
        {
            Slash();
        }
    }
}

public class EffectIllusionSlash : FightEffect
{
    // TODO Projectile attack
    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;
}