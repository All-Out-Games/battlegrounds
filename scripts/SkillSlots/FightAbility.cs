using AO;
using Assembly.scripts.Effects.ActiveSkills;

internal class DefaultTargettingEffect : AEffect 
{
    public override bool IsActiveEffect => true;
    public override void OnEffectStart(bool isDropIn) {}
    public override void OnEffectUpdate() {}
    public override void OnEffectEnd(bool interrupt) {}
}

public partial class FightAbility : Ability
{
    public static string DefaultIconPath = "$AO/new/Player Inventory/abilities_inventory_bar/inv_square_empty_25.png";
    public static string DefaultSkillKey = "Empty";
    public static string DefaultAbilityIcon = "AbilityIcon_Separate/x_icon_ability.png";
    
    public FightPlayer FightPlayer;
    public virtual string SkillKey => DefaultSkillKey;
    public virtual string SkillIconPath => SkillConfig.GetAbilityIconPath(SkillKey);
    public virtual int Interruptlevel => 1;

    public sealed override Texture Icon => Assets.GetAsset<Texture>(SkillIconPath);

    public override Type TargettingEffect => TargettingMode == TargettingMode.Self ? null : typeof(DefaultTargettingEffect);

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

    public override bool OnTryActivate(List<Player> targetPlayers, Vector2 positionOrDirection, float magnitude)
    {
        base.OnTryActivate(targetPlayers, positionOrDirection, magnitude);
        FightPlayer = (FightPlayer)Player;
        FightPlayer.OnSkillActivate?.Invoke(FightPlayer.SkillActivationInfo.GetActivationInfo(Interruptlevel, SkillKey));
        return true;
    }
    
}

