using AO;
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

    public override float Cooldown => 1;//EffectConfig.PsyThrowConfig.Cooldown;

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
    public override bool IsActiveEffect => true;

    private FightPlayer _casterFp;

    protected override bool PreventMovement => true;
    public override bool BlockAbilityActivation => true;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        DurationRemaining = EffectConfig.PsyThrowConfig.GrabTime;
        _casterFp = Caster as FightPlayer;
        if (Network.IsServer)
        {
            UIManager.CallClient_SetPlayerPopup(FightPlayer.Entity.NetworkId, $"You are grabbed by {Caster.Entity.Name}!", 1f);
        }
        if (_casterFp != null)
        {
            _casterFp.AddEffect<EffectPsyThrowReady>(FightPlayer, DurationRemaining-0.2f); // Let this effect expire slightly earlier to trigger auto-throw
        }
        FightPlayer.SetAnimTrigger("psythrow_grabbed");
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        if (_casterFp != null)
        {
            _casterFp.RemoveEffect<EffectPsyThrowReady>(false);
        }
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
        var slotsMgr = FightPlayer.GetSkillSlots();
        var f = typeof(AbilityPsyThrow);
        _originalIndex = slotsMgr.GetAbilityIndex(f);
        if (_originalIndex > 0)
        {
            slotsMgr.ReplaceSlot(_originalIndex, slotsMgr.GetAbilityInstance(typeof(AbilityPsyThrowLaunch)));
        }
        else
        {
            Log.Error("PsyThrow: Skill Replacement Error! The player does not have the primary skill equipped.");
        }
        FightPlayer.UnsetAnimTrigger("psythrow_attack_throw");
        FightPlayer.SetAnimTrigger("psythrow_attack");
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
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

        if (!interrupt)
        {
            // If this effect is not interrupted, the grabber hasn't choose a direction to throw their victim.
            // So, we throw the victim toward the grabber automatically.
            Caster?.AddEffect<EffectPsyThrowLaunch>(FightPlayer, 1f, launch =>
            {
                launch.AbilityPositionOrDirection = (FightPlayer.Entity.Position - Caster.Entity.Position).Normalized;
            });
        }
        FightPlayer.SetAnimTrigger("psythrow_attack_throw");
    }
}

public class EffectPsyThrowLaunch : FightEffectWithNoFlinch
{
    // Casted on the victim of psythrow
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;

    private EffectConfig.PsyThrowConfig _config;

    private List<Entity> _interactedEntities;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        if (FightPlayer.HasEffect<EffectPsyThrow>())
        {
            Caster.RemoveEffect<EffectPsyThrowReady>(true);
            
            FightPlayer.AddBump(AbilityPositionOrDirection * EffectConfig.PsyThrowConfig.ThrowStrength, false);
            
            //FightPlayer.GetEffectMgr().AddNoMovement(Caster.Entity, 1f);
            DurationRemaining = 1f;
            
            _interactedEntities = new List<Entity>() {FightPlayer.Entity, FightPlayer.CollisionEntity};
            AssignConfig(EffectConfig.PsyThrowConfig.GetDefault(FightPlayer.CurrentAttack));
            
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.Damage) with {InterruptLevel = 0};
            info.ReactionInfo.Flinch = false;
            FightPlayer.TakeDamage(Caster as FightPlayer, info);
            
            FightPlayer.UnsetAnimTrigger("sentfly_end");
            FightPlayer.SetAnimTrigger("sentfly");
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
    
    protected void OnThrowCollision(Entity other)
    {
        if(_interactedEntities.Contains(other)) return;
        _interactedEntities.Add(other);
        
        PlayerCollisionChild otherPlayer = other.GetComponent<PlayerCollisionChild>();
        if (otherPlayer != null)
        {
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.Damage) with {InterruptLevel = FightPlayer.DamageInfo.KnockBackInterruptLevel, DmgType = DamageType.None};
            FightPlayer.TakeDamage(Caster as FightPlayer, info);
            
            info.ReactionInfo.Amount = other.NetworkId == Caster.Entity.NetworkId ? _config.SelfDamage : _config.Damage;
            otherPlayer.Player.TakeDamage(Caster as FightPlayer, info);
        }
    }
}