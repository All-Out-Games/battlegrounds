using AO;
using Assembly.scripts.SceneObjects.TriggersAndInteractions;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityBearTrap : FightAbility
{
    public override string SkillKey => "BearTrap";
    
    public override Type Effect => typeof(EffectBearTrap);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.CircleAOE;

    public override float Cooldown => EffectConfig.BearTrapConfig.Cooldown;

    public override float MaxDistance => EffectConfig.BearTrapConfig.MaxSetupDistance;
}

public class EffectBearTrap: FightEffect
{
    public override bool IsActiveEffect => false;


    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        DurationRemaining = 0.1f;
    }

    public override void OnEffectEnd(bool interrupt)
    {
        Vector2 trapPos = FightPlayer.Entity.Position + AbilityPositionOrDirection * EffectConfig.BearTrapConfig.MaxSetupDistance * AbilityMagnitude;
        base.OnEffectEnd(interrupt);
        FightClubGameManager.Instance.ServerSpawn(EffectConfig.BearTrapConfig.TrapPrefabPath, trapPos,
            entity =>
            {
                BearTrap trap = entity.GetComponent<BearTrap>();
                trap.CallClient_Initialization(FightPlayer.Entity, EffectConfig.BearTrapConfig.TrapLifeTime);
            });
    }
}

public class EffectBearTrapSnare : FightEffectWithNoFlinch
{
    public override bool IsActiveEffect => false;
    public override bool FreezePlayer => true;

    public override bool BlockAbilityActivation => true;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.SetAnimTrigger("beartrapped");
        
        
        FightPlayer caster = Caster as FightPlayer;
        int dmg = caster == null ? EffectConfig.BearTrapConfig.TrapBaseDamage : EffectConfig.BearTrapConfig.GetDefault(caster.CurrentAttack).Damage;
        FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(dmg, DamageType.Melee, FightPlayer.DamageInfo.StunInterruptLevel);
        info.ReactionInfo.Flinch = false;
        FightPlayer.TakeDamage(caster, info);
        
        DurationRemaining = MainLayer.GetCurrentStateLength();
    }
    
}