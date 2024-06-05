using AO;

namespace Assembly.scripts.SkillSlots.Abilities;

public class FightAbility : Ability
{
    public FightPlayer FightPlayer;
    public virtual string SkillKey => "Empty";
    public virtual string SkillIconPath => "$AO/allouticon1.png";

    public sealed override Texture Icon => Assets.GetAsset<Texture>(SkillIconPath);

    public FightAbility LoadFightAbility<T>(Player player) where T : FightAbility
    {
        var fa  = player.GetAbility<T>() as FightAbility;
        return fa;
    }
    public override bool CanUse()
    {
        FightPlayer = (FightPlayer)Player;
        return FightPlayer.SkillCastGeneralCheck() && FightPlayer.GetSkillTree().SkillLevelDict[SkillKey] > 0;
    }

    public override bool CanTarget(Player player)
    {
        return true;
    }
}