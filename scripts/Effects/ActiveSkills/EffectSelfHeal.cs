using AO;
using Assembly.scripts.SceneObjects;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilitySelfHeal : FightAbility
{
    public override string SkillKey => "SelfHeal";

    public override Type Effect => typeof(EffectSelfHeal);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;
    
    public override float Cooldown => GetCooldown(FightPlayer);

    public static float GetCooldown(FightPlayer fp)
    {
        int lv = int.Min(4, fp.GetSkillTree().GetSkillLevel("SelfHeal"));
        return EffectConfig.SelfHealConfig.Cooldown + 1 - lv;
    }
}



public class EffectSelfHeal : FightEffect
{
    public override bool IsActiveEffect => true;
    protected override int InterruptLevel => 1000;
    public override bool BlockAbilityActivation => true;

    protected override bool PreventMovement => true;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        FightPlayer.OnReceiveDamage += OnDamageEvent;
        DurationRemaining = EffectConfig.SelfHealConfig.ChannelTime;
        FightPlayer.UnsetAnimTrigger("selfheal_end");
        FightPlayer.SetAnimTrigger("selfheal");

        SFX.Play(SFXKeys.HealingStartAudio, DefaultSoundDesc);
        SoundId = SFX.Play(SFXKeys.HealingLoopAudio, new SFX.PlaySoundDesc() { EntityToFollow = FightPlayer.Entity, RangeMultiplier = 0.5f});
    }
    

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.OnReceiveDamage -= OnDamageEvent;

        bool enhanced = FightPlayer.HasSkill("Concentrate");
        if (!interrupt)
        {
            
            FightPlayer.DamageInfo healInfo = FightPlayer.DamageInfo.CreateHealInfo(EffectConfig.SelfHealConfig.HealAmtBase + (enhanced ? EffectConfig.SelfHealConfig.ConcentrateExtraHealth : 0));
            if (FightPlayer.GetSkillTree().GetSkillLevel("SelfHeal") > 4)
            {
                healInfo.ReactionInfo.Amount -= 5;
            }
            FightPlayer.TakeDamage(FightPlayer, healInfo);
            FightPlayer.SetAnimTrigger("selfheal_end");
            SFX.Play(SFXKeys.HealingEndAudio, DefaultSoundDesc);
            FightPlayer.AddEffect<EffectGenericPostActionDelay>(Caster, MainLayer.GetCurrentStateLength());
        }
        else
        {
            if (enhanced)
            {
                FightPlayer.DamageInfo healInfo =
                    FightPlayer.DamageInfo.CreateHealInfo(EffectConfig.SelfHealConfig.ConcentrateExtraHealth);
                FightPlayer.TakeDamage(FightPlayer, healInfo);
                FightPlayer.AddEffect<EffectRage>(FightPlayer, EffectConfig.SelfHealConfig.ConcentrateRageTime);
            }
        }

        
        SFX.Stop(SoundId);
    }
}