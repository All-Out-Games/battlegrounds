using AO;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.VFX;
using StreamReader = AO.StreamReader;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityPsyThrow : FightAbility
{
    public override string SkillKey => "PsyThrow";

    public override Type Effect => typeof(EffectPsyThrow);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Select;
    public override float MaxDistance => EffectConfig.PsyThrowConfig.Range;
    public override int MaxTargets => 1;

    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        return fp.GetSkillTree().GetSkillLevel("PsyThrow") > 1
            ? EffectConfig.PsyThrowConfig.Cooldown - 1
            : EffectConfig.PsyThrowConfig.Cooldown;
    }
    
    public override bool CanTarget(Player player)
    {
        return GenericCanTarget(player as FightPlayer) && !player.HasEffect<EffectPsyThrow>() && Vector2.Distance(FightPlayer.Position, player.Position) < MaxDistance;
    }

    public static readonly string LaunchSkillKey = "PsyThrowLaunch";
}

public class AbilityPsyThrowLaunch : FightAbility
{
    public override string SkillKey => AbilityPsyThrow.LaunchSkillKey;
    public override string SkillIconPath => "AbilityIcon_Merged/psionic/psythrow_launch.png";

    public override Type Effect => typeof(EffectPsyThrowLaunch);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.DirectionOnNearest;
    public override float MaxDistance => EffectConfig.PsyThrowConfig.ThrowRange;
    public override int MaxTargets => 1;
    
    public override float Cooldown => 1f;
    public override bool CanTarget(Player player)
    {
        return player.HasEffect<EffectPsyThrow>();
    }

    public override bool CanUse()
    {
        // We don't need to check EffectDeath since it will remove PsyThrowReady upon started
        return Player.HasEffect<EffectPsyThrowReady>();
    }
}



public class EffectPsyThrow : FightEffectWithNoFlinch
{
    public override bool IsActiveEffect => false;

    private FightPlayer _casterFp;

    protected override bool PreventMovement => true;
    public override bool BlockAbilityActivation => true;

    public override void OnEffectStart(bool isDropIn)
    {
        // remove Rollout & Parry
        Player.RemoveEffect<EffectRollOut>(true);
        Player.RemoveEffect<EffectParry>(true);
        
        base.OnEffectStart(isDropIn);
        DurationRemaining = EffectConfig.PsyThrowConfig.GrabTime;
        _casterFp = Caster as FightPlayer;
        if (_casterFp != null)
        {
            _casterFp.AddEffect<EffectPsyThrowReady>(FightPlayer, DurationRemaining-0.2f); // Let this effect expire slightly earlier to trigger auto-throw
        }
        
        FightPlayer.SetAnimTrigger("psythrow_grabbed");
        SoundId = SFX.Play(SFXKeys.PsyThrowVictim, DefaultSoundDesc);
        
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        if (_casterFp != null)
        {
            _casterFp.RemoveEffect<EffectPsyThrowReady>(false);
        }

        SFX.FadeOutAndStop(SoundId, 0.3f);
    }
}

public class EffectPsyThrowReady : FightEffectWithNoFlinch
{
    // Temporarily replace the caster's throw ability so that they can launch the grabbed player
    public override bool IsActiveEffect => false;
    public override List<Type> AbilityWhitelist => Wl;
    private static readonly List<Type> Wl = new List<Type>() { typeof(AbilityPsyThrowLaunch) };
    public override bool BlockAbilityActivation => true;
    

    private int _originalIndex = -1;
    
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.UnsetAnimTrigger("psythrow_attack_throw");
        FightPlayer.SetAnimTrigger("psythrow_attack");

        SFX.Play(SFXKeys.PsyThrowStart, DefaultSoundDesc);
        SoundId = SFX.Play(SFXKeys.PsyThrowLoop, new SFX.PlaySoundDesc(){EntityToFollow = FightPlayer.Entity, Loop = true, LoopTimeout = 1+EffectConfig.PsyThrowConfig.GrabTime});

        if (Player.IsLocal)
        {
            var slotsMgr = FightPlayer.GetSkillSlots();
            var f = typeof(AbilityPsyThrow);
            _originalIndex = slotsMgr.GetAbilityIndex(f);
            if (_originalIndex > 0)
            {
                slotsMgr.ReplaceSlot(_originalIndex, slotsMgr.GetAbilityInstance(typeof(AbilityPsyThrowLaunch)), 0.5f);
            }
            else
            {
                Log.Error("PsyThrow: Skill Replacement Error! The player does not have the primary skill equipped.");
            }
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);

        if (!interrupt)
        {
            // If this effect is not interrupted, the grabber hasn't choose a direction to throw their victim.
            // So, we throw the victim toward the grabber automatically.
            Caster?.AddEffect<EffectPsyThrowLaunch>(FightPlayer, 1f, launch =>
            {
                launch.AbilityDirection = (FightPlayer.Entity.Position - Caster.Entity.Position).Normalized;
            });
        }
        FightPlayer.SetAnimTrigger("psythrow_attack_throw");
        SFX.Stop(SoundId);
        SFX.Play(SFXKeys.PsyThrowEnd, DefaultSoundDesc);

        if (Player.IsLocal)
        {
            var slotsMgr = FightPlayer.GetSkillSlots();
            var f = typeof(AbilityPsyThrowLaunch);
            _originalIndex = slotsMgr.GetAbilityIndex(f);
            if (_originalIndex > 0)
            {
                slotsMgr.ReplaceSlot(_originalIndex, slotsMgr.GetAbilityInstance(typeof(AbilityPsyThrow)));
            }
            else
            {
                Log.Error("PsyThrow: Skill Replacement Error! The player does not have the primary skill equipped.");
            }
        }
    }
}

public class EffectPsyThrowLaunch : FightEffectWithNoFlinch
{
    // Casted on the victim of psythrow
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;

    private EffectConfig.PsyThrowConfig _config;
    

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        if (FightPlayer.HasEffect<EffectPsyThrow>())
        {
            Caster.RemoveEffect<EffectPsyThrowReady>(true);
            
            int? lv = (Caster as FightPlayer)?.GetSkillTree().GetSkillLevel("PsyThrow");
            FightPlayer.AddBump(AbilityDirection * EffectConfig.PsyThrowConfig.ThrowStrength, false);
            
            //FightPlayer.GetEffectMgr().AddNoMovement(Caster.Entity, 1f);
            DurationRemaining = 1f;
            
            AssignConfig(EffectConfig.PsyThrowConfig.GetDefault(FightPlayer.CurrentAttack, lv.GetValueOrDefault(1)));
            
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.Damage) with {InterruptLevel = 0};
            info.ReactionInfo.Flinch = false;
            info.SkillKey = SkillConfig.PsyThrowConfig.SkillKey;
            FightPlayer.TakeDamage(Caster as FightPlayer, info);
            
            FightPlayer.UnsetAnimTrigger("sentfly_end");
            FightPlayer.SetAnimTrigger("sentfly");
            // Lv.4 Effect - Explode after throw
            if (lv.GetValueOrDefault(1) > 4)
            {
                FightPlayer.AddEffect<EffectPsyExplosion>(Caster, 1f, explosion =>
                {
                    explosion.Damage = (int)float.Ceiling(0.2f * info.ReactionInfo.Amount);
                    explosion.Radius = 3;
                    explosion.SkillKey = "PsyThrow";
                });
            }
        }
        else
        {
            FightPlayer.RemoveEffect<EffectPsyThrowLaunch>(true);
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.SetAnimTrigger("sentfly_end");
        FightPlayer.AddEffect<EffectGenericPostActionDelay>();
    }

    private void AssignConfig(EffectConfig.PsyThrowConfig cfg)
    {
        _config = cfg;
    }
}

/// <summary>
/// Addon effect for Psionic Beam Lv.4 & PsyThrow Lv.4
/// </summary>
public class EffectPsyExplosion : FightEffect
{
    public override bool IsActiveEffect => false;

    public int Damage;
    public int Radius;
    public string SkillKey = "PsyThrow";

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        if (!interrupt)
        {
            // Spawn an explosion
            FightClubGameManager.Instance.ClientSpawn(VFXPrefabs.PsionicBeamHitVFX, FightPlayer.Position, entity => entity.LocalScale = Vector2.One * 3);
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Damage, DamageType.AOE);
            info.AwardCoin = false;
            info.SkillKey = SkillKey;
            var cbPlayers = FightClubGameManager.Instance.OverlapCircleForDamageables(FightPlayer.Position, Radius, Caster);
            SFX.Play(SFXKeys.PsyboltHitAudio, DefaultSoundDesc);
            foreach (var other in cbPlayers)
            {
                if(other.Entity == Caster.Entity) continue;
                
                other.TakeDamage(Caster as FightPlayer, info);
            }
        }
    }
}