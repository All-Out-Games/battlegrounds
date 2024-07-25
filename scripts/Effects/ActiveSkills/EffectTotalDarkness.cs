using AO;
namespace Assembly.scripts.Effects.ActiveSkills;

public class AbilityTotalDarkness : FightAbility
{
    public override string SkillKey => "TotalDarkness";
    
    public override Type Effect => typeof(EffectTotalDarkness);
    public override bool MonitorEffectDuration => true;
    public override TargettingMode TargettingMode => TargettingMode.Self;

    public override float Cooldown => EffectConfig.RageConfig.Cooldown;
}

public class EffectTotalDarkness : FightEffect
{
    public override bool IsActiveEffect => false;


    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        DurationRemaining = EffectConfig.TotalDarknessConfig.BlindTime;
        
        // Blind Every Player in combat
        var fpList = FightClubGameManager.Instance.OverlapCircleForCombatPlayers(FightPlayer.Entity.Position,
            EffectConfig.TotalDarknessConfig.Range);

        foreach (var fp in fpList)
        {
            if (fp != FightPlayer)
            {
                if (fp.HasEffect<EffectBlinded>())
                {
                    fp.GetEffect<EffectBlinded>().DurationRemaining = EffectConfig.TotalDarknessConfig.BlindTime; // Refresh if already blinded
                }
                else
                {
                    fp.AddEffect<EffectBlinded>(FightPlayer, EffectConfig.TotalDarknessConfig.BlindTime);
                }
                
            }
        }
    }
    
}