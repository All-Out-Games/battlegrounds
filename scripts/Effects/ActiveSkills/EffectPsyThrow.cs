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
    
    public override float Cooldown => EffectConfig.PsyThrowConfig.Cooldown;

    public static readonly string LaunchSkillKey = "PsyThrowLaunch";
}

public class AbilityPsyThrowLaunch : FightAbility
{
    public override string SkillKey => AbilityPsyThrow.LaunchSkillKey;
    public override string SkillIconPath => "ability_icon_tmp/PsyThrow_Tmp.png"; // TODO

    public override Type Effect => typeof(EffectPsyThrowLaunch);
    public override bool MonitorEffectDuration => true;
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



public class EffectPsyThrow : EffectStun
{
    public override bool IsActiveEffect => false;

    private FightPlayer _casterFp;

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        DurationRemaining = EffectConfig.PsyThrowConfig.GrabTime;
        _casterFp = Caster as FightPlayer;
        UIManager.CallClient_SetPlayerPopup(FightPlayer.Entity.NetworkId, $"You are grabbed by {Caster.Entity.Name}!", 1f);
        if (_casterFp != null)
        {
            _casterFp.AddEffect<EffectPsyThrowReady>();
        }
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

public class EffectPsyThrowReady : FightEffect
{
    // Temporarily replace the caster's throw ability so that they can launch the grabbed player
    public override bool IsActiveEffect => false;
    public override List<Type> AbilityWhitelist => Wl;
    private static readonly List<Type> Wl = new List<Type>() { typeof(AbilityPsyThrowLaunch) };

    private int _originalIndex = -1;
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        var slotsMgr = FightPlayer.GetSkillSlots();
        var f = typeof(AbilityPsyThrow);
        _originalIndex = slotsMgr.GetAbilityIndex(f);
        if (_originalIndex > 0)
        {
            slotsMgr.ReplaceSlot(_originalIndex, slotsMgr.GetAbilityInstance(typeof(AbilityPsyThrowLaunch)));
            // FightPlayer.CurrentTargettingAbility = slotsMgr.GetAbilityInstance(typeof(AbilityPsyThrowLaunch));
            // TODO: Cannot immediately target the grabbed target
        }
        else
        {
            Log.Error("PsyThrow: Skill Replacement Error! The player does not have the primary skill equipped.");
        }
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
    }
}

public class EffectPsyThrowLaunch : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;

    private EffectConfig.PsyThrowConfig _config;

    private List<Entity> _interactedEntities;

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        if (FightPlayer.HasEffect<EffectPsyThrow>())
        {
            FightPlayer.AddBump(AbilityPositionOrDirection * EffectConfig.PsyThrowConfig.ThrowStrength, false);
            FightPlayer.GetEffectMgr().AddNoMovement(Caster.Entity, 1f);
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(0) with {InterruptLevel = 0}; // Cause a flinch with no dmg
            FightPlayer.TakeDamage(Caster as FightPlayer, info);
            DurationRemaining = 1f;
            _interactedEntities = new List<Entity>() {FightPlayer.Entity, FightPlayer.CollisionEntity};
            
            FightPlayer.AddPlayerCollisionFunction(OnThrowCollision);
        }
        else
        {
            FightPlayer.RemoveEffect<EffectPsyThrowLaunch>(true);
        }
        
    }

    public override void NetworkDeserialize(StreamReader reader)
    {
        base.NetworkDeserialize(reader);
        FightPlayer.AddPlayerCollisionFunction(OnThrowCollision);
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        if (!interrupt)
        {
            FightPlayer.RemovePlayerCollisionFunction(OnThrowCollision);
        }
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