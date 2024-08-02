using System.Collections;
using AO;
using Assembly.scripts;
using Assembly.scripts.SceneObjects;

public class AbilityPunch : FightAbility
{
    public override string SkillKey => "Punch";
    public override Type Effect => typeof(EffectPunch);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override string SkillIconPath
    {
        get
        {
            FightPlayer ??= Network.LocalPlayer as FightPlayer;
            return FightPlayer == null ? SkillConfig.GetPunchAbilityIconPath(1) : SkillConfig.GetPunchAbilityIconPath(FightPlayer.PunchLevel);
        }
    }
}

public class EffectPunch : FightEffect
{
    protected EffectConfig.PunchConfig Config;
    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => true;

    protected bool Activated = false;

    protected Vector2 punchDir;
    
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);

        AssignConfig(EffectConfig.GetPlayerPunchConfig(FightPlayer.PunchLevel, FightPlayer.CurrentAttack));
        FightPlayer.SetAnimTrigger(Config.AnimationTrigger); ;
    }
    

    public override void OnEffectUpdate()
    {
        if (Util.OneTime(ElapsedTime > EffectConfig.PunchConfig.PunchActivationTime, ref Activated))
        {
            Punch();
            Activated = true;
        }
    }

    public void AssignConfig(EffectConfig.PunchConfig cfg)
    {
        DurationRemaining = EffectConfig.PunchConfig.PunchAnimationTime;
        Config = cfg;
    }
    public void Punch()
    {
        //Log.Debug($"Punch! Dmg: {Config.PunchDamage}");
        Physics.RaycastHit rc;
        
        // Re-adjust aiming
        punchDir = FightPlayer.GetPunchDirection();
        FightPlayer.SetAimTarget(Entity.Position + punchDir);
        
        // SFX based on punch lvl
        SFX.Play(SFXKeys.GetPunchSFXByLevel(FightPlayer.PunchLevel), DefaultSoundDesc);
        
        var hit = Physics.RaycastWithWhitelist(Entity.Position, punchDir.Normalized,
            EffectConfig.PunchConfig.PunchRange, FightClubGameManager.Instance.GetCombatPlayersCollisionEntities(), new Entity[]{ },out rc);

        
        if (hit) // If players are too close, always hit
        {
            var other = rc.Entity.GetComponent<DamageableObject>();
            if (other != null)
            {
                FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Config.PunchDamage);
                // other.Player.TakeDamage(FightPlayer, info);
                other.TakeDamage(FightPlayer, info);
            }
        }
        else
        {
            // Damage the object anyways if they are very very close, if no rays hit
            var closeTargets =
                FightClubGameManager.Instance.OverlapCircleForCombatPlayers(Entity.Position,
                    EffectConfig.PunchConfig.PunchMustHitRange);
            closeTargets.Remove(FightPlayer);
            //Log.Warn($"{closeTargets.Count}");
            if (closeTargets.Count > 0)
            {
                foreach (var fp in closeTargets)
                {
                    FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Config.PunchDamage);
                    // other.Player.TakeDamage(FightPlayer, info);
                    fp.TakeDamage(FightPlayer, info);
                }
            }
        }
    }
    


}