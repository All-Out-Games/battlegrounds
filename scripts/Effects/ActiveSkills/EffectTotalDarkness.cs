using AO;
using Assembly.scripts.SceneObjects;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityTotalDarkness : FightAbility
{
    public override string SkillKey => "TotalDarkness";
    
    public override Type Effect => typeof(EffectTotalDarkness);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        int lv = fp.GetSkillTree().GetSkillLevel("TotalDarkness");
        if (lv > 2)
        {
            return EffectConfig.TotalDarknessConfig.Cooldown - 1;
        }
        else
        {
            return EffectConfig.TotalDarknessConfig.Cooldown;
        }
    }
}

public class EffectTotalDarkness : FightEffect
{
    public override bool IsActiveEffect => false;
    private EffectConfig.TotalDarknessConfig _config;


    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        DurationRemaining = EffectConfig.TotalDarknessConfig.BlindTime;
        FightPlayer.SpineAnimator.OnEvent += OnAnimationEvent;
        FightPlayer.SetAnimTrigger("total_darkness");
        SoundId = SFX.Play(SFXKeys.TotalDarknessAudio, DefaultSoundDesc);
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
            //FightClubGameManager.Instance.ClientSpawn(VFXPrefabKeys.LeapSlamCraterVfxPath, FightPlayer.Entity.Position);
            DarkAttack();
        }
    }

    public void DarkAttack()
    {
        // Blind Every Player in combat
        var fpList = FightClubGameManager.Instance.OverlapCircleForDamageables(FightPlayer.Entity.Position, EffectConfig.TotalDarknessConfig.Range, Player);
        
        _config = EffectConfig.TotalDarknessConfig.GetConfig(FightPlayer.CurrentAttack, FightPlayer.GetSkillTree().GetSkillLevel("TotalDarkness"));
        FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.Damage, DamageType.None);
        info.SkillKey = SkillConfig.TotalDarknessConfig.SkillKey;
        info.CrateImmediateDestroy = true;
        
        foreach (var dmg in fpList)
        {
            if(!dmg.Damageable()) continue;
            if (dmg is PlayerCollisionChild fp)
            {
                if (fp.Player != FightPlayer)
                {
                    var player = fp.Player;
                    if (player.HasEffect<EffectBlinded>())
                    {
                        player.GetEffect<EffectBlinded>().DurationRemaining = EffectConfig.TotalDarknessConfig.BlindTime; // Refresh if already blinded
                    }
                    else
                    {
                        player.AddEffect<EffectBlinded>(FightPlayer, EffectConfig.TotalDarknessConfig.BlindTime);
                    }
                    if (_config.LifeSteal)
                    {
                        FightPlayer.DamageInfo healInfo = FightPlayer.DamageInfo.CreateHealInfo((int)(EffectConfig.TotalDarknessConfig.FourStarLifeStealMultiplier * info.ReactionInfo.Amount));
                        FightPlayer.TakeDamage(FightPlayer, healInfo);
                    } 
                }
            
            }
            dmg.TakeDamage(FightPlayer, info);
        }
        FightPlayer.SpineAnimator.OnEvent -= OnAnimationEvent;
    }
    
}