using Assembly.scripts.SceneObjects;

namespace Assembly.scripts.Effects.ActiveSkills;
using AO;

public class AbilityBladeStorm : FightAbility
{
    public override string SkillKey => "BladeStorm";

    public override Type Effect => typeof(EffectBladeStorm);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Line;
    public override float Cooldown => GetCooldown(FightPlayer);
    public override float MaxDistance => 8f;

    public static float GetCooldown(FightPlayer fp)
    {
        //return fp.GetSkillTree().GetSkillLevel("BladeFrenzy") > 1 ? EffectConfig.BladeFrenzyConfig.Cooldown - 1 : EffectConfig.BladeFrenzyConfig.Cooldown;
        return 1;
    }
}

public class EffectBladeStormKnockdown : FightEffectWithNoFlinch
{
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool IsCC => true;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        Vector2 dir = Position - Caster.Position;
        FightPlayer.SetFacingDirection(dir.X > 0);
        FightPlayer.SetAnimTrigger("bladestorm_victim");
    }
}

public partial class EffectBladeStorm: FightEffectWithImmunity
{
    private EffectConfig.ShoulderCrashConfig _config;
    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => false;

    protected override bool PreventMovement => true;
    protected override string InvincibilityReason => "BladeStorm";
    
    private bool _touchedEnemy = false;
    
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        
        FightPlayer.SetAnimBool("bladestorm_fail", false);
        FightPlayer.SetAnimBool("bladestorm_spin_ended", false);
        AssignConfig(EffectConfig.ShoulderCrashConfig.GetDefault(FightPlayer.CurrentAttack, FightPlayer.GetSkillTree().GetSkillLevel("ShoulderCrash")));
        DurationRemaining = _config.DashDuration + 0.1f;
        //FightPlayer.AddPlayerCollisionFunction(OnShoulderCrashCollision);

        Vector2 dir = GetDashDirection();

        FightPlayer.AddDash(dir * _config.DashSpeed, _config.DashDuration);
        // The player is invincible and not allowed to input movement during the dash
        //FightStateMachine.UnsetTrigger("shoulder_crash_end");
        FightPlayer.SetAnimTrigger("bladestorm_charge");
        if (!isDropIn)
        {
            SFX.Play(SFXKeys.ShoulderCrashAudio, DefaultSoundDesc);
        }

        SoundId = SFX.Play(SFXKeys.ShoulderCrashLoopAudio, DefaultSoundDesc with { Loop = true, LoopTimeout = 10f });
        
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        //FightPlayer.RemovePlayerCollisionFunction(OnShoulderCrashCollision);
        FightStateMachine.SetTrigger("shoulder_crash_end");
        SFX.Stop(SoundId);
        if (interrupt)
        {
            FightPlayer.SetAnimBool("bladestorm_fail", false);
        }
        else
        {
            FightPlayer.SetAnimBool("bladestorm_fail", true);
        }
    }



    public void AssignConfig(EffectConfig.ShoulderCrashConfig cfg)
    {
        _config = cfg;
    }

    protected Vector2 GetDashDirection()
    {
        return AbilityDirection;
    }
    

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if (Network.IsServer && !_touchedEnemy)
        {
            var touchedPlayers = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(Entity.Position, 1.5f, Player);
            if (touchedPlayers.Count != 0)
            {
                // Server authoritatively stop this effect (by adding another active effect), and knock touched players down
                CallClient_OnKnockPlayer(FightPlayer);
                _touchedEnemy = true;
            }
        }
    }

    [ClientRpc]
    public static void OnKnockPlayer(FightPlayer caster)
    {
        if (caster.Alive())
        {
            caster.AddDash(Vector2.Zero, 0); // Remove Dash
            var touchedPlayers = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(caster.Position, 1.5f, caster);
            foreach (var fp in touchedPlayers)
            {
                fp.AddEffect<EffectBladeStormKnockdown>(caster, 1f);
            }

            caster.AddEffect<EffectBladeStormSpin>(caster, 0.6f);
            // TODO set damage
        }
    }
}

public class EffectBladeStormSpin : FightEffectWithImmunity
{
    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => false;

    protected override bool PreventMovement => true;
    protected override string InvincibilityReason => "BladeStormSpin";
    public int DamagePerTick = 5;
    
    protected float NextDmgTick = 0.17f;
    protected bool Ticked;

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if (Util.OneTime(ElapsedTime > NextDmgTick, ref Ticked))
        {
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(DamagePerTick, DamageType.Melee, FightPlayer.DamageInfo.KnockBackInterruptLevel);
            foreach (var dmg in FightClubGameManager.Instance.OverlapCircleForDamageables(Entity.Position, 2, Player))
            {
                var touchedPlayers = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(Entity.Position, 1.5f, Player);
                if (touchedPlayers.Count != 0)
                {
                    dmg.TakeDamage(FightPlayer, info);
                }
            }
            NextDmgTick += 0.17f;
            Ticked = false;
        }
    }

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.SetAnimBool("bladestorm_spin_ended", false);
        FightPlayer.SetAnimTrigger("bladestorm_spin");
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.SetAnimBool("bladestorm_spin_ended", true);
    }
}