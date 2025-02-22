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

public class EffectBearTrap : FightEffect
{
    public override bool IsActiveEffect => false;


    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        DurationRemaining = 0.1f;
        FightStateMachine.SetTrigger("place_trap");
    }

    public override void OnEffectEnd(bool interrupt)
    {
        Vector2 trapPos = FightPlayer.Entity.Position + AbilityDirection * AbilityMagnitude;
        base.OnEffectEnd(interrupt);
        int lv = FightPlayer.GetSkillTree().GetSkillLevel("BearTrap");
        float lifeTime = EffectConfig.BearTrapConfig.TrapLifeTime +
                         (lv-1) * EffectConfig.BearTrapConfig.LifeTimeGrowth;
        FightClubGameManager.Instance.ServerSpawn(EffectConfig.BearTrapConfig.TrapPrefabPath, trapPos,
            entity =>
            {
                BearTrap trap = entity.GetComponent<BearTrap>();
                trap.CallClient_Initialization(FightPlayer.Entity, lifeTime);
                trap.CallClient_SetSize(lv > 4 ? 1+EffectConfig.BearTrapConfig.FourStarSizeBonus : 1);
            });
    }
}

public class EffectBearTrapSnare : FightEffectWithNoFlinch
{
    public override bool IsActiveEffect => false;
    public override bool FreezePlayer => true;

    public override bool BlockAbilityActivation => true;
    public override bool IsCC => true;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        
        FightPlayer.SetAnimTrigger("beartrapped");
        FightPlayer caster = Caster as FightPlayer;
        int dmg = caster == null ? EffectConfig.BearTrapConfig.TrapBaseDamage : EffectConfig.BearTrapConfig.GetDefault(caster.CurrentAttack).Damage;
        FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(dmg, DamageType.Melee, FightPlayer.DamageInfo.StunInterruptLevel);
        info.ReactionInfo.Flinch = false;
        info.SkillKey = SkillConfig.BearTrapConfig.SkillKey;
        FightPlayer.TakeDamage(caster, info);

        DurationRemaining = 3.8f;  //MainLayer.GetCurrentStateLength();
        // Remove Dash / Bump
        FightPlayer.AddBump(Vector2.Zero, true);
    }
    
}