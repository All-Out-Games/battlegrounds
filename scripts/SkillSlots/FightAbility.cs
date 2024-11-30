using AO;
using Assembly.scripts.Effects.ActiveSkills;

internal class DefaultTargettingEffect : FightEffect
{
    public override bool IsActiveEffect => false;
    public override bool GetInterruptedByNewActiveEffects => true;
    public override void OnEffectStart(bool isDropIn) {}

    public override void OnEffectUpdate()
    {
    }
    public override void OnEffectEnd(bool interrupt) {}

    protected override int InterruptLevel => 1000;
}

public partial class FightAbility : Ability
{
    public static string DefaultIconPath = "$AO/new/Player Inventory/abilities_inventory_bar/inv_square_empty_25.png";
    public static string DefaultSkillKey = "Empty";
    public static string DefaultAbilityIcon = "AbilityIcon_Separate/x_icon_ability.png";
    
    private FightPlayer _fightPlayer;

    public FightPlayer FightPlayer
    {
        get
        {
            if (_fightPlayer == null)
            {
                _fightPlayer = Player as FightPlayer;
            }

            return _fightPlayer;
        }
    }
    public virtual string SkillKey => DefaultSkillKey;
    public virtual string SkillIconPath => SkillConfig.GetAbilityIconPath(SkillKey);
    public virtual int Interruptlevel => 1;

    public sealed override Texture Icon => Assets.GetAsset<Texture>(SkillIconPath);

    public override Type TargettingEffect => TargettingMode == TargettingMode.Self ? null : typeof(DefaultTargettingEffect);

    public override bool CanUse()
    {
        if (SkillKey == "Empty" || SkillKey == null) return false;
        return FightPlayer.SkillCastGeneralCheck() && FightPlayer.GetSkillTree().SkillLevelDict[SkillKey] > 0;
    }

    public override bool CanTarget(Player player)
    {
        return true;
    }

    protected bool GenericCanTarget(FightPlayer player)
    {
        return player.Targetable();
    }

    public override bool OnTryActivate(List<Player> targetPlayers, Vector2 positionOrDirection, float magnitude)
    {
        base.OnTryActivate(targetPlayers, positionOrDirection, magnitude);
        FightPlayer.OnSkillActivate?.Invoke(FightPlayer.SkillActivationInfo.GetActivationInfo(Interruptlevel, SkillKey));
        return true;
    }
    

}

