using AO;
using Assembly.scripts.SceneObjects;

namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityTotalDarkness : FightAbility
{
    public override string SkillKey => "TotalDarkness";
    
    public override Type Effect => typeof(EffectTotalDarkness);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => EffectConfig.TotalDarknessConfig.Cooldown;
}

public class EffectTotalDarkness : FightEffect
{
    public override bool IsActiveEffect => false;
    private EffectConfig.TotalDarknessConfig _config;


    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        DurationRemaining = EffectConfig.TotalDarknessConfig.BlindTime;
        SoundId = SFX.Play(SFXKeys.TotalDarknessAudio, DefaultSoundDesc);
        
        // Blind Every Player in combat
        var fpList = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(FightPlayer.Entity.Position,
            EffectConfig.TotalDarknessConfig.Range);
        
        // FightPlayer.SetAnimTrigger("total_darkness");
        _config = EffectConfig.TotalDarknessConfig.GetConfig(FightPlayer.CurrentAttack);

        foreach (var fp in fpList)
        {
            if (fp != FightPlayer && fp.Damageable())
            {
                if (fp.HasEffect<EffectBlinded>())
                {
                    fp.GetEffect<EffectBlinded>().DurationRemaining = EffectConfig.TotalDarknessConfig.BlindTime; // Refresh if already blinded
                }
                else
                {
                    fp.AddEffect<EffectBlinded>(FightPlayer, EffectConfig.TotalDarknessConfig.BlindTime);
                }

                FightPlayer.DamageInfo info = FightPlayer.DamageInfo.CreateDamageInfo(_config.Damage, DamageType.None);
                fp.TakeDamage(FightPlayer, info);
            }
        }
    }
    
}