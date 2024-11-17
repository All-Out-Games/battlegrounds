using AO;
using Assembly.scripts.SceneObjects;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityWindPunch : FightAbility
{
    public override string SkillKey => "WindPunch";
    public override Type Effect => typeof(EffectWindPunch);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        int lv = fp.GetSkillTree().GetSkillLevel("WindPunch");
        float cd = EffectConfig.WindPunchConfig.Cooldown;
        if (lv > 3)
        {
            cd -= 1;
        }

        return cd;
    }
}

public class EffectWindPunch : FightEffect
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
        var lvl = FightPlayer.GetSkillTree().GetSkillLevel("WindPunch");
        AssignConfig(EffectConfig.GetWindPunchConfig(FightPlayer.CurrentAttack, lvl));
        FightPlayer.SetAnimTrigger(Config.AnimationTrigger); ;

        SoundId = SFX.Play(SFXKeys.WindPunchAudio, DefaultSoundDesc);
        float boost = lvl > 4 ? 1.4f : EffectConfig.WindPunchConfig.BoostModifier;
        Player.AddEffect<EffectMovementSpeedChange>(FightPlayer, Config.PunchActivationTime,
            change =>
            {
                change.SpdModifier = boost;
            });
    }
    

    public override void OnEffectUpdate()
    {
        if (Util.OneTime(ElapsedTime > Config.PunchActivationTime, ref Activated))
        {
            Punch();
            Activated = true;
        }
    }

    public void AssignConfig(EffectConfig.PunchConfig cfg)
    {
        DurationRemaining = cfg.PunchAnimationTime;
        Config = cfg;
    }

    public void AfterWindPunchHit(FightPlayer fp)
    {
        if (fp.Alive())
        {
            fp.GetEffectMgr().AddLeapSlamKnockdown(FightPlayer.Entity, 1.0f, 0.5f);
            fp.AddBump(punchDir.Normalized * 75f, false);
        }
    }
    public void Punch()
    {
        //Log.Debug($"Punch! Dmg: {Config.PunchDamage}");
        Physics.RaycastHit rc;
        
        // Re-adjust aiming
        punchDir = FightPlayer.GetPunchDirection();
        FightPlayer.SetAimTarget(Entity.Position + punchDir);

        int lv = FightPlayer.GetSkillTree().GetSkillLevel("WindPunch");
        var hit = Physics.RaycastWithWhitelist(Entity.Position, punchDir.Normalized,
            EffectConfig.PunchConfig.PunchRange, FightClubGameManager.Instance.GetAllDamagableEntities(Player), new Entity[]{ },out rc);

        
        if (hit) // If players are too close, always hit
        {
            var other = rc.Collider.GetComponent<DamageableObject>();
            if (other != null)
            {
                FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Config.PunchDamage);
                info.SkillKey = SkillConfig.IceFistNodeConfig.SkillKey;
                // other.Player.TakeDamage(FightPlayer, info);
                other.TakeDamage(FightPlayer, info);
                FightPlayer fp = rc.Collider.GetComponent<PlayerCollisionChild>().Player;
                if (info.OverrideDamageNumber == FightPlayer.DamageInfo.DamageNumberOverrideType.None)
                {
                    AfterWindPunchHit(fp);
                }
                
            }
        }
        else
        {
            // Damage the object anyways if they are very very close, if no rays hit
            var closeTargets =
                FightClubGameManager.Instance.OverlapCircleForCombatPlayers(Entity.Position, EffectConfig.PunchConfig.PunchMustHitRange, Player);
            closeTargets.Remove(FightPlayer);

            if (closeTargets.Count > 0)
            {
                foreach (var fp in closeTargets)
                {
                    FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Config.PunchDamage);
                    info.SkillKey = SkillConfig.IceFistNodeConfig.SkillKey;
                    // other.Player.TakeDamage(FightPlayer, info);
                    fp.TakeDamage(FightPlayer, info);
                    if (info.OverrideDamageNumber == FightPlayer.DamageInfo.DamageNumberOverrideType.None)
                    {
                        AfterWindPunchHit(fp);
                    }
                    
                }
            }
        }
    }
}