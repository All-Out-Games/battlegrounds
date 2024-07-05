using System.Collections;
using Assembly.scripts.VFX;

namespace Assembly.scripts.Effects.ActiveSkills;
using AO;

public class AbilityBattleCry : FightAbility
{
    public override string SkillKey => "BattleCry";
    public override Type Effect => typeof(EffectBattleCry);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    public override float Cooldown => EffectConfig.BattleCryConfig.Cooldown;
}

public class EffectBattleCry : FightEffectWithNoFlinch
{
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool FreezePlayer => true;

    protected EffectConfig.BattleCryConfig Config;
    protected bool Activated = false;
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        AssignConfig(EffectConfig.BattleCryConfig.GetDefault(FightPlayer.CurrentAttack));

        FightPlayer.SetAnimTrigger("battlecry");
        DurationRemaining = MainLayer.GetCurrentStateLength();
        FightPlayer.SpineAnimator.OnEvent += OnAnimationEvent;
    }
    
    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.SpineAnimator.OnEvent -= OnAnimationEvent;
    }

    public override void OnAnimationEvent(string eventName)
    {
        base.OnAnimationEvent(eventName);
        if (eventName == "Attack")
        {
            FightClubGameManager.Instance.ClientSpawn(VFXPrefabKeys.BattleCryVfxPath, FightPlayer.Entity.Position);
            BattleCry();
        }
    }

    public void AssignConfig(EffectConfig.BattleCryConfig cfg)
    {
        Config = cfg;
        DurationRemaining = EffectConfig.BattleCryConfig.RoarAnimationTime;
    }

    private void BattleCry()
    {
        Vector2 selfPos = FightPlayer.Entity.Position;
        var cbPlayers = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(selfPos, Config.RoarRadius);
            
        foreach (var fp in cbPlayers)
        {
            if (fp.Entity.NetworkId == FightPlayer.Entity.NetworkId)
            {
                continue;
            }

            FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Config.RoarDamage, DamageType.AOE) with {InterruptLevel = FightPlayer.DamageInfo.StunInterruptLevel};
            info.ReactionInfo.Flinch = false;
            fp.TakeDamage(FightPlayer, info);
            fp.GetEffectMgr().AddBattleCryStun(FightPlayer.Entity, Config.StunTime);
            fp.AddScreenShake(1f,0.5f);
        }
    }
    
}

public class EffectBattleCryStun : EffectStun
{
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        FightPlayer.SetAnimTrigger("battlecry_stun");
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.SetAnimTrigger("RESET");
    }
}