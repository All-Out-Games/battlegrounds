using System.Collections;
using AO;
using Assembly.scripts;
using Assembly.scripts.SceneObjects;
using Assembly.scripts.VFX;

public class AbilityPunch : FightAbility
{
    public override string SkillKey => "Punch";
    public override Type Effect => typeof(EffectPunch);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override string SkillIconPath => SkillConfig.GetPunchAbilityIconPath(FightPlayer.PunchLevel);
}

public class EffectMagicPunch : FightEffect
{
    public override bool IsActiveEffect => false;
    private StatAuraVFX _aura;
    private Spine_Animator _auraAnimator;
    private bool _faded;
    // Flag-like effect. Does nothing itself but changes how effectPunch behaves!

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        AddAura();
    }
    
    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        _auraAnimator.LocalEnabled = FightPlayer.SpineAnimator.LocalEnabled;

        if (Util.OneTime(DurationRemaining < 1, ref _faded))
        {
            _aura.SetAnimTrigger("disappear");
        }
        
    }
    
    private void AddAura()
    {
        Prefab auraPrefab = VFXPrefabs.StatAura;
        _aura = auraPrefab.Instantiate().GetComponent<StatAuraVFX>();
        _aura.SetSkin("attack", new Vector4(1f, 0.431f, 0.78f, 1));
        _aura.SetAnimTrigger("appear");
        _auraAnimator = _aura.Animator;
        _aura.Spawn(FightPlayer.Entity,new Vector2(0f, 0.2f), false, DurationRemaining+1f);
    }

    public void Extend(float sec)
    {
        _aura.ExtendLifetime(sec);
        DurationRemaining += sec;
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
            EffectConfig.PunchConfig.PunchRange, FightClubGameManager.Instance.GetAllDamagableEntities(Player), new Entity[]{ },out rc);

        FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(Config.PunchDamage);
        info.SkillKey = FightPlayer.PunchLevel == 1 ? "Punch" : $"Punch{FightPlayer.PunchLevel}";
        if (hit) // If players are too close, always hit
        {
            var other = rc.Collider.GetComponent<DamageableObject>();
            
            if (other.Alive() && other.Damageable())
            {
                // other.Player.TakeDamage(FightPlayer, info);
                // Log.Warn($"{rc.Collider.Entity.Name}");
                other.TakeDamage(FightPlayer, info);
            }
        }
        else
        {
            // Damage the object anyways if they are very very close, if no rays hit
            var closeTargets = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(Entity.Position, EffectConfig.PunchConfig.PunchMustHitRange, Player);
            closeTargets.Remove(FightPlayer);
            //Log.Warn($"{closeTargets.Count}");
            if (closeTargets.Count > 0)
            {
                foreach (var fp in closeTargets)
                {
                    // other.Player.TakeDamage(FightPlayer, info);
                    fp.TakeDamage(FightPlayer, info);
                }
            }
        }
    }
    


}