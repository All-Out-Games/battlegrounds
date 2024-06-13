using System.Collections;

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

public class EffectBattleCry : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => true;
    public override bool FreezePlayer => true;

    protected EffectConfig.BattleCryConfig Config;
    public override void OnEffectStart()
    {
        base.OnEffectStart();
        AssignConfig(EffectConfig.BattleCryConfig.GetDefault(FightPlayer.CurrentAttack));

        BattleCry();
    }
    
    public override void OnEffectEnd(bool interrupt)
    {
        
    }

    public void AssignConfig(EffectConfig.BattleCryConfig cfg)
    {
        Config = cfg;
        DurationRemaining = EffectConfig.BattleCryConfig.RoarAnimationTime;
    }

    private void BattleCry()
    {
        if (FightPlayer.IsLocal)
        {
            // TODO: Get some real animation
            FightPlayer.SetAnimTrigger("wave"); // Animation can be done locally first...
        }
        if (Network.IsServer)
        {
            // Damage and broadcast animation
            FightPlayer.CallClient_SetAnimTriggerBroadcast("wave");
            Coroutine.Start(Entity, DelayActiveBattleCry(EffectConfig.BattleCryConfig.RoarActivationTime));
        }
    }

    IEnumerator DelayActiveBattleCry(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (Network.IsServer)
        {
            Log.Debug($"ROAR! Dmg = {Config.RoarDamage}");
            Vector2 selfPos = FightPlayer.Entity.Position;
            var cbPlayers = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(selfPos, Config.RoarRadius);
            
            foreach (var fp in cbPlayers)
            {
                if (fp.Entity.NetworkId == FightPlayer.Entity.NetworkId)
                {
                    continue;
                }

                FightPlayer.DamageInfo info = new FightPlayer.DamageInfo { DmgType = DamageType.AOE};
                fp.TakeDamage(Config.RoarDamage, FightPlayer, info);
                fp.GetEffectMgr().CallClient_AddStun(FightPlayer.Entity.NetworkId, Config.StunTime);

            }
        }
    }
}