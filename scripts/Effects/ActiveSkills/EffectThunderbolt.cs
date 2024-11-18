using AO;
using Assembly.scripts.SceneObjects;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityThunderbolt : FightAbility
{
    public override string SkillKey => "Thunderbolt";
    
    public override Type Effect => typeof(EffectThunderbolt);
    public override bool MonitorEffectDuration => false;
    public override TargettingMode TargettingMode => TargettingMode.CircleAOE;

    public override float Cooldown => EffectConfig.ThunderboltConfig.Cooldown;

    public override float MaxDistance => GetRange(FightPlayer);

    public static float GetRange(FightPlayer fp)
    {
        float range = EffectConfig.ThunderboltConfig.DefaultRange;
        if (fp.GetSkillTree().GetSkillLevel("Thunderbolt") > 3) range += 2;
        return range;
    }
}

public class EffectThunderbolt : FightEffect
{
    public override bool IsActiveEffect => true;
    public override bool BlockAbilityActivation => true;
    public override bool IsValidTarget => true;
    protected override bool PreventMovement => true;

    public ReticleObject ThunderReticle;
    public static string ThunderRecticlePrefabPath = "ThunderStrike.prefab";

    private EffectConfig.ThunderboltConfig _config;
    private Vector2 _position;

    private bool _attacked;
    
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        _config = EffectConfig.ThunderboltConfig.GetDefault(FightPlayer.CurrentAttack, FightPlayer.GetSkillTree().GetSkillLevel("Thunderbolt"));
        _position = FightPlayer.Position + AbilityDirection * AbilityMagnitude;
        FightClubGameManager.Instance.ClientSpawn(ThunderRecticlePrefabPath,_position
            ,
            entity => { ThunderReticle = entity.GetComponent<ReticleObject>();}
            );

        if (_config.GetImmunity)
        {
            FightPlayer.RegisterPreDamageEvent(this);
            FightPlayer.AddInvincibilityReason("Thunderbolt5");
        }
        
        if (!isDropIn)
        {
            DurationRemaining = EffectConfig.ThunderboltConfig.TotalSummonTime;
            FightPlayer.SetAnimTrigger("summon_thunder");
            SFX.Play(SFXKeys.LightningBoltSummonAudio, DefaultSoundDesc);
            if (ThunderReticle.Alive())
            {
                ThunderReticle.Animator.Entity.LocalEnabled = false;
                ThunderReticle.PlayReticleLerpAnimation(new Vector2(3f, 3f), Vector2.Zero, 
                    EffectConfig.ThunderboltConfig.ThunderSummonTime + EffectConfig.ThunderboltConfig.ThunderStartTime);
            }
        }

        FightPlayer.SpineAnimator.OnEvent += OnAnimationEvent;
    }

    public override void OnAnimationEvent(string eventName)
    {
        base.OnAnimationEvent(eventName);
        if (eventName == "Attack" && MainLayer.CurrentState.Name == "BAT_003/summon")
        {
            if (ThunderReticle.Alive())
            {
                ThunderReticle.Animator.Entity.LocalEnabled = true;
                ThunderReticle.SetAnimation("start", false);
            }
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        if (ThunderReticle.Alive())
        {
            ThunderReticle.Despawn();
        }

        FightPlayer.SpineAnimator.OnEvent -= OnAnimationEvent;
        if (_config.GetImmunity)
        {
            FightPlayer.RemovePreDamageEvent(this);
            FightPlayer.RemoveInvincibilityReason("Thunderbolt5");
        }
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if (Util.OneTime(
                ElapsedTime > EffectConfig.ThunderboltConfig.ThunderSummonTime+ EffectConfig.ThunderboltConfig.ThunderStartTime, ref _attacked))
        {
            ThunderAttack();
            if (!ThunderReticle.Alive()) return;
            ThunderReticle.Reticle.LocalEnabled = false;
            SFX.Play(SFXKeys.LightningBoltAudio, DefaultSoundDesc with { EntityToFollow = ThunderReticle.Entity });
            ThunderReticle.SetAnimation("end", false);
        }
    }

    public void ThunderAttack()
    {
        // TODO
        FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.Damage, DamageType.AOE, FightPlayer.DamageInfo.KnockBackInterruptLevel);
        info.SkillKey = SkillConfig.ThunderboltNodeConfig.SkillKey;
        info.SpecialDeathAnimation = true;
        info.CrateImmediateDestroy = true;
        info.ReactionInfo.Flinch = false;
        
        var damageables = FightClubGameManager.Instance.OverlapCircleForDamageables(_position, EffectConfig.ThunderboltConfig.AoeRange, Player);
        foreach (var dmg in damageables)
        {
            if(!dmg.Damageable()) continue;
            
            dmg.TakeDamage(FightPlayer, info);

            if (dmg is PlayerCollisionChild fp)
            {
                var other = fp.Player;

                //other.AddEffect<EffectKnockDown>(FightPlayer, EffectConfig.LeapSlamConfig.KnockDownTime + 0.5f);
                //other.GetEffectMgr().AddLeapSlamKnockdown(FightPlayer.Entity, EffectConfig.LeapSlamConfig.KnockDownTime + 0.5f, EffectConfig.LeapSlamConfig.KnockDownTime);
                other.GetEffectMgr().AddElectrocute(FightPlayer.Entity, EffectConfig.ThunderboltConfig.ShockTime);
            }
        }
        
        FightPlayer.SpineAnimator.OnEvent -= OnAnimationEvent;
    }
    
    public override void PreDamageMod(ref FightPlayer.DamageInfo info)
    {
        base.PreDamageMod(ref info);
        info.ReactionInfo.Flinch = false;
        info.AwardCoin = false;
        
        if(info.ReactionInfo.Amount > 0) {
            info.ReactionInfo.Amount = 0; // Does not affect healing
            info.OverrideDamageNumber = FightPlayer.DamageInfo.DamageNumberOverrideType.Immune;
        }

    }
}

public class EffectElectricShock : FightEffect
{
    public override bool IsActiveEffect => true;
    protected override bool PreventMovement => true;
    public override bool BlockAbilityActivation => true;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.SetAnimTrigger("shocked_start", true);
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.SetAnimTrigger("shocked_end", true);
    }
}