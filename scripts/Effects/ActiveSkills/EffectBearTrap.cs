using AO;
using Assembly.scripts.SceneObjects.TriggersAndInteractions;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityBearTrap : FightAbility
{
    public override string SkillKey => "BearTrap";
    
    public override Type Effect => typeof(EffectBearTrap);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => EffectConfig.BearTrapConfig.Cooldown;
}

public class EffectBearTrap: FightEffect
{
    public override bool IsActiveEffect => false;


    public override void OnEffectStart()
    {
        base.OnEffectStart();
        DurationRemaining = 0.1f;
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightClubGameManager.Instance.ServerSpawn(EffectConfig.BearTrapConfig.TrapPrefabPath, FightPlayer.Entity.Position,
            entity =>
            {
                BearTrap trap = entity.GetComponent<BearTrap>();
                trap.CallClient_Initialization(FightPlayer.Entity, EffectConfig.BearTrapConfig.TrapLifeTime);
            });
    }
}

public class EffectBearTrapSnare : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool FreezePlayer => true;

    public override void OnEffectStart()
    {
        base.OnEffectStart();
        DurationRemaining = EffectConfig.BearTrapConfig.TrapHoldTime;
    }

    public override void OnEffectEnd(bool interrupt)
    {
        // TODO
        base.OnEffectEnd(interrupt);
    }
}