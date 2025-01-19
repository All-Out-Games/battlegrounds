using Assembly.scripts.SceneObjects;
using Assembly.scripts.SceneObjects.Projectiles;

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

public class AbilityIllusionSlash : FightAbility
{
    public override string SkillKey => "IllusionSlash";
    public override bool MonitorEffectDuration => false;
    public override Type Effect => typeof(EffectIllusionSlash);
    public override float Cooldown => EffectConfig.BladeFrenzyConfig.IllusionSlashCooldown;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float MaxDistance => 6f;
    public override bool CanUse()
    {
        return FightPlayer.SkillCastGeneralCheck() && Player.HasEffect<EffectBladeFrenzy>();
    }

    public override string SkillIconPath => "AbilityIcon_Separate/defense/illusion_slash.png";
}

public class EffectBladeFrenzy : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => false;
    public EffectConfig.BladeFrenzyConfig Config;
    private int _originalIndex;
    
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
            if (Config.GiveIllusionSlash)
            {
                var slotsMgr = FightPlayer.GetSkillSlots();
                var f = typeof(AbilityBladeFrenzy);
                _originalIndex = slotsMgr.GetAbilityIndex(f);
                if (_originalIndex > 0)
                {
                    slotsMgr.ReplaceSlot(_originalIndex, slotsMgr.GetAbilityInstance(typeof(AbilityIllusionSlash)));
                }
                else
                {
                    Log.Error("Blade Frenzy: Replacement Error! The player does not have the primary skill equipped.");
                }
            }
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);

        if (FightPlayer.IsLocal)
        {
            FightPlayer.GetSkillSlots().ReplaceSlot(0, FightPlayer.GetFightAbility<AbilityPunch>());
            if (Config.GiveIllusionSlash)
            {
                var slotsMgr = FightPlayer.GetSkillSlots();
                slotsMgr.ReplaceSlot(_originalIndex, slotsMgr.GetAbilityInstance(typeof(AbilityBladeFrenzy)));
            }
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
    public int Damage;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        DurationRemaining = EffectConfig.BladeFrenzyConfig.DefaultSlashAnimationTime;
        
        Damage = EffectConfig.BladeFrenzyConfig.GetDefault(FightPlayer.CurrentAttack,FightPlayer.GetSkillTree().GetSkillLevel("BladeFrenzy")).Damage;
        _slashDir = FightPlayer.GetPunchDirection();
        FightPlayer.SetAimTarget(Entity.Position + _slashDir);
        FightPlayer.SetAnimTrigger("bf_slash", true);
        SFX.Play(SFXKeys.GetRandomKatanaSound(), DefaultSoundDesc);
    }

    private void Slash()
    {
        Vector2 damagePos = _slashDir.Normalized + Position;
        FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Damage);
        info.SkillKey = "BladeFrenzy";
        info.CrateImmediateDestroy = true;

        var damageables = FightClubGameManager.Instance.OverlapCircleForDamageables(damagePos, 2, FightPlayer);
        foreach (var dmg in damageables)
        {
            dmg.TakeDamage(FightPlayer, info);
        }
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
    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;

    private bool _launched;

    private EffectConfig.ProjectileConfig Config;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        Config = EffectConfig.BladeFrenzyConfig.GetIllusionSlashCfg(FightPlayer.CurrentAttack);
        if (!isDropIn)
        {
            DurationRemaining = 0.4f;
        }
        FightPlayer.SetAnimTrigger("illusion_slash", true);
        FightPlayer.SetFacingDirection(AbilityDirection.X > 0);
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if (Util.OneTime(ElapsedTime > 0.15f, ref _launched))
        {
            ProjectileThrow();
        }
    }
    
    private void ProjectileThrow()
    {
        // This function can be overwritten to create different projectile throwing behaviors
        // However, you should try to build the logic of the projectile within itself
        // i.e. inherit the Projectile component and put it on your prefab.

        Entity proj = Game.SpawnProjectile(FightPlayer, Config.ProjectilePrefabKey,
            $"{Config.ProjectilePrefabKey}",
            Position + AbilityDirection, AbilityDirection);
        //proj.Position = Entity.Position;
        InitializeProjectile(proj);
    }

    private void InitializeProjectile(Entity proj)
    {
        Projectile projComp = proj.GetComponent<Projectile>();
        projComp.Speed = Config.Speed;
        projComp.Lifetime = Config.ProjectileLifetime;
        IllusionWaveProjectile supplementProjectileComp = proj.GetComponent<IllusionWaveProjectile>();
        supplementProjectileComp.LifeTime = Config.ProjectileLifetime;
        supplementProjectileComp.InitializeProjectile(FightPlayer, Config.Damage, true);
    }
}