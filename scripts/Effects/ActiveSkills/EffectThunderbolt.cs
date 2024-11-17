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

    private bool _attacked;
    
    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        _config = EffectConfig.ThunderboltConfig.GetDefault(FightPlayer.CurrentAttack, FightPlayer.GetSkillTree().GetSkillLevel("Thunderbolt"));
        FightClubGameManager.Instance.ClientSpawn(ThunderRecticlePrefabPath,
            FightPlayer.Position + AbilityDirection * AbilityMagnitude,
            entity => { ThunderReticle = entity.GetComponent<ReticleObject>();}
            );
        
        if (!isDropIn)
        {
            DurationRemaining = EffectConfig.ThunderboltConfig.TotalSummonTime;
            FightPlayer.SetAnimTrigger("summon_thunder");
            SFX.Play(SFXKeys.LightningBoltSummonAudio, DefaultSoundDesc);
            if (ThunderReticle.Alive())
            {
                ThunderReticle.Animator.Entity.LocalEnabled = false;
                ThunderReticle.PlayReticleLerpAnimation(new Vector2(3f, 3f), Vector2.One, 
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
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if (Util.OneTime(
                ElapsedTime > EffectConfig.ThunderboltConfig.ThunderSummonTime+ EffectConfig.ThunderboltConfig.ThunderStartTime, ref _attacked))
        {
            ThunderAttack();
            if (!ThunderReticle.Alive()) return;
            SFX.Play(SFXKeys.LightningBoltAudio, DefaultSoundDesc with { EntityToFollow = ThunderReticle.Entity });
            ThunderReticle.SetAnimation("end", false);
        }
    }

    public void ThunderAttack()
    {
        // TODO
    }
}