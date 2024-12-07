using Assembly.scripts.VFX;

namespace Assembly.scripts.Effects.ActiveSkills;
using AO;
using Assembly.scripts.SceneObjects.TriggersAndInteractions;


public class AbilityChargingStation : FightAbility
{
    public override string SkillKey => "ChargingStation";
    
    public override Type Effect => typeof(EffectChargingStation);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.CircleAOE;

    public override float Cooldown => GetCooldown(FightPlayer);

    public override float MaxDistance => EffectConfig.ChargingStationConfig.DeployRange;
    
    public static float GetCooldown(FightPlayer fp)
    {
        int lv = int.Min(4, fp.GetSkillTree().GetSkillLevel("ChargingStation"));
        float cd = EffectConfig.ChargingStationConfig.Cooldown;
        if (lv > 3) cd -= 2;
        return cd;
    }
}

public class AbilityChargingStationDetonation : FightAbility
{
    public override string SkillKey => "ChargingStationDetonate";

    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => 6f;
    
    public override Type Effect => typeof(EffectChargingStationDetonate);
    
    public override string SkillIconPath => "AbilityIcon_Separate/elemental/detonator.png";
    
    public override bool CanUse() => Player.HasEffect<EffectChargingStation>();
}

public class EffectChargingStation : FightEffect
{
    public override bool IsActiveEffect => false;
    private ChargingStation _station;
    private EffectConfig.ChargingStationConfig _config;
    private int _originalIndex;
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        _config = EffectConfig.ChargingStationConfig.GetDefault(FightPlayer.CurrentAttack, FightPlayer.GetSkillTree().GetSkillLevel("ChargingStation"));
        if (!isDropIn)
        {
            DurationRemaining = EffectConfig.ChargingStationConfig.LifeTime;
            
            Vector2 trapPos = FightPlayer.Entity.Position + AbilityDirection * AbilityMagnitude;
            // int lv = FightPlayer.GetSkillTree().GetSkillLevel("ChargingStation");
            FightClubGameManager.Instance.ServerSpawn(EffectConfig.ChargingStationConfig.StationPrefabPath, trapPos,
                entity =>
                {
                    _station = entity.GetComponent<ChargingStation>();
                    _station.CallClient_Initialization(FightPlayer.Entity, DurationRemaining);
                    _station.CallClient_SetShield(_config.ShieldAmt);
                });
        }
        
        if (FightPlayer.IsLocal)
        {
            var slotsMgr = FightPlayer.GetSkillSlots();
            var f = typeof(AbilityChargingStation);
            _originalIndex = slotsMgr.GetAbilityIndex(f);
            if (_originalIndex > 0)
            {
                slotsMgr.ReplaceSlot(_originalIndex, slotsMgr.GetAbilityInstance(typeof(AbilityChargingStationDetonation)), 1);
            }
            else
            {
                Log.Error("Charging Station: Skill Replacement Error! The player does not have the primary skill equipped.");
            }
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        if (_station.Alive() && FightPlayer.Alive())
        {
            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.Damage, DamageType.AOE, FightPlayer.DamageInfo.KnockBackInterruptLevel);
            info.SkillKey = SkillConfig.ChargingStationNodeConfig.SkillKey;
            info.SpecialDeathAnimation = true;
            info.CrateImmediateDestroy = true;
            info.ReactionInfo.Flinch = false;
        
            var damageables = FightClubGameManager.Instance.OverlapCircleForDamageables(_station.Position, EffectConfig.ChargingStationConfig.AoeRange, Player);
            foreach (var dmg in damageables)
            {
                if(!dmg.Damageable()) continue;
            
                dmg.TakeDamage(FightPlayer, info);

                if (_config.Shock &&  dmg is PlayerCollisionChild fp)
                {
                    var other = fp.Player;
                    other.GetEffectMgr().AddElectrocute(FightPlayer.Entity, EffectConfig.ThunderboltConfig.ShockTime);
                }
            }
            
            _station.Despawn();
        }
        if (_originalIndex > 0 && FightPlayer.IsLocal)
        {
            var slotsMgr = FightPlayer.GetSkillSlots();
            slotsMgr.ReplaceSlot(_originalIndex, slotsMgr.GetAbilityInstance(typeof(AbilityChargingStation)));
        }
    }
}

public class EffectChargingStationDetonate : FightEffect
{
    public override bool IsActiveEffect => false;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        DurationRemaining = 0.01f;
        FightPlayer.RemoveEffect<EffectChargingStation>(true);
    }
}