using AO;
using Assembly.scripts.SceneObjects;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityIceFist : FightAbility
{
    public override string SkillKey => "IceFist";
    public override Type Effect => typeof(EffectIceFist);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    public override float Cooldown => EffectConfig.IceFistConfig.Cooldown;
}

public class EffectIceFist : FightEffect
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

        AssignConfig(EffectConfig.GetIcePunchConfig(FightPlayer.CurrentAttack, FightPlayer.GetSkillTree().GetSkillLevel("IceFist")));
        FightPlayer.SetAnimTrigger(Config.AnimationTrigger); ;

        SoundId = SFX.Play(SFXKeys.IcePunchAudio, DefaultSoundDesc);
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

        int lv = FightPlayer.GetSkillTree().GetSkillLevel("IceFist");
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
                if (lv > 4)
                {
                    rc.Collider.GetComponent<PlayerCollisionChild>()?.Player.AddEffect<EffectMovementSpeedChange>(FightPlayer, 1.5f,
                        change =>
                        {
                            change.SpdModifier = 0.9f;
                        });
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
                    if (lv > 4)
                    {
                        rc.Collider.GetComponent<PlayerCollisionChild>()?.Player.AddEffect<EffectMovementSpeedChange>(FightPlayer, 1.5f,
                            change =>
                            {
                                change.SpdModifier = 0.9f;
                            });
                    }
                }
            }
        }
    }
}