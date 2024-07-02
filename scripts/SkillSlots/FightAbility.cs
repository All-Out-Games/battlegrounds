using AO;
using Assembly.scripts.Effects.ActiveSkills;

public partial class FightAbility : Ability
{
    public static string DefaultIconPath = "$AO/allouticon1.png";
    public static string DefaultSkillKey = "Empty";
    
    public FightPlayer FightPlayer;
    public virtual string SkillKey => DefaultSkillKey;
    public virtual string SkillIconPath => SkillConfig.GetIconPath(SkillKey);
    public virtual int Interruptlevel => 1;

    public sealed override Texture Icon => Assets.GetAsset<Texture>(SkillIconPath);
    
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

    protected bool GenericCanTarget(FightPlayer player)
    {
        return player.CurrentHealth > 0;
    }

    public override void OnActivate(Player targetPlayer, Vector2 positionOrDirection, float magnitude)
    {
        base.OnActivate(targetPlayer, positionOrDirection, magnitude);
        FightPlayer = (FightPlayer)Player;
        FightPlayer.OnSkillActivate?.Invoke(FightPlayer.SkillActivationInfo.GetActivationInfo(Interruptlevel, SkillKey));
    }
    
}

