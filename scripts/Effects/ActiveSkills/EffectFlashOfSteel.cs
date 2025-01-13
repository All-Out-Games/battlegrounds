using AO;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityFlashOfSteel : FightAbility
{
    public override string SkillKey => "FlashOfSteel";

    public override Type Effect => typeof(EffectFlashOfSteel);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => 5f;

    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        return fp.GetSkillTree().GetSkillLevel("FlashOfSteel") > 1 ? EffectConfig.FlashOfSteelConfig.Cooldown - 1 : EffectConfig.FlashOfSteelConfig.Cooldown;
    }
}

public class TrailEffect : FightEffect
{
    public override bool IsActiveEffect => false;
    public override float DefaultDuration => 1f;
    Trail_Renderer _trail;
    Entity _entity;

    public static Texture TrailTexture = Assets.GetAsset<Texture>("Props/ParticleA.png");
    public override void OnEffectEnd(bool interrupt)
    {
        if (_entity != null)
        {
            _entity.Destroy();
        }
    }

    public override void OnEffectStart(bool isDropIn)
    {
        _entity = Entity.Create();
        _entity.SetParent(Player.Entity, false);
        _entity.LocalPosition = new Vector2(0,.2f);
        _trail = _entity.AddComponent<Trail_Renderer>();
        _trail.TargetLength = 20;
        _trail.Width = 3;
        _trail.Tint = Vector4.Red;
        _trail.Texture = TrailTexture;
        _trail.DepthOffset = 1;
    }
}

public class EffectFlashOfSteel : FightEffectWithImmunity
{
    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => false;

    public EffectConfig.FlashOfSteelConfig _config;

    protected override bool PreventMovement => true;
    protected override string InvincibilityReason => "FlashOfSteel";

    private bool _dash;
    
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        _config = EffectConfig.FlashOfSteelConfig.GetDefault(FightPlayer.CurrentAttack,
            FightPlayer.GetSkillTree().GetSkillLevel("FlashOfSteel"));

        // The player is invincible and not allowed to input movement during the dash
        //FightStateMachine.UnsetTrigger("shoulder_crash_end");
        FightPlayer.SetAnimTrigger("melee");
        if (!isDropIn)
        {
            //SFX.Play(SFXKeys.ShoulderCrashAudio, DefaultSoundDesc);
            DurationRemaining = EffectConfig.FlashOfSteelConfig.DashTime + EffectConfig.FlashOfSteelConfig.DashDelay + 0.1f;
            FightPlayer.SetAnimTrigger("fos_start", true);
            FightPlayer.AddEffect<TrailEffect>(null, 1f);
        }
        
    }

    public override void OnEffectUpdate()
    {
        if (Util.OneTime(ElapsedTime > EffectConfig.FlashOfSteelConfig.DashDelay, ref _dash))
        {
            Vector2 dir = AbilityDirection;
            FightPlayer.SetFacingDirection(dir.X > 0);
            FightPlayer.AddDash(dir * EffectConfig.FlashOfSteelConfig.DashSpeed, EffectConfig.FlashOfSteelConfig.DashTime);
        }
        
        // TODO SFX
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.SetAnimTrigger("RESET");
        FightPlayer.RemoveEffect<TrailEffect>(false);
        //TODO DAMAGE
    }
}