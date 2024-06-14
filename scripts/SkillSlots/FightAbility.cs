using AO;
using SC = SkillConfig;
using Assembly.scripts.Effects.ActiveSkills;

public class FightAbility : Ability
{
    public static string DefaultIconPath = "$AO/allouticon1.png";
    public static string DefaultSkillKey = "Empty";
    
    public FightPlayer FightPlayer;
    public virtual string SkillKey => "Empty";
    public virtual string SkillIconPath => SkillConfig.GetIconPath(SkillKey); // TODO Get an lock icon somewhere?

    public sealed override Texture Icon => Assets.GetAsset<Texture>(SkillIconPath);

    public FightAbility LoadFightAbility<T>(Player player) where T : FightAbility
    {
        var fa  = player.GetAbility<T>() as FightAbility; // Not very costly, it's just a GetComponent, but we still need to avoid calling this in Update() alike
        return fa;
    }
    public override bool CanUse()
    {
        if (SkillKey == "Empty") return false;
        FightPlayer = (FightPlayer)Player;
        return FightPlayer.SkillCastGeneralCheck() && FightPlayer.GetSkillTree().SkillLevelDict[SkillKey] > 0;
    }

    public override bool CanTarget(Player player)
    {
        return true;
    }

    
    public static readonly Dictionary<string, Type> AbilityQueryDict = new Dictionary<string, Type>()
    {
        {"Empty", typeof(FightAbility)},
        {SC.PunchNodeConfig.SkillKey, typeof(AbilityPunch)},
        {SC.RollOutNodeConfig.SkillKey, typeof(AbilityRollOut)},
        {SC.ShieldConfig.SkillKey, typeof(AbilityShield)},
        {SC.ShoulderCrashNodeConfig.SkillKey, typeof(AbilityShoulderCrash)},
        {SC.SpoonThrowConfig.SkillKey, typeof(AbilitySpoonThrow)},
        {SC.GroundStompConfig.SkillKey, typeof(AbilityGroundStomp)},
        {SC.RageConfig.SkillKey, typeof(AbilityRage)},
        {SC.DoublePunchConfig.SkillKey, typeof(AbilityDoublePunch)},
        {SC.SelfDestructConfig.SkillKey, typeof(AbilitySelfDestruct)},
        {SC.BattleCryConfig.SkillKey, typeof(AbilityBattleCry)},
        {SC.ClawSlashConfig.SkillKey, typeof(AbilityClawSlash)}
    };
}

