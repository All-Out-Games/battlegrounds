using AO;

namespace Assembly.scripts.SkillSlots.Abilities;

public class FightAbility : Ability
{
    public FightPlayer FightPlayer;
    
    
    public override bool CanUse()
    {
        return true;
    }

    public override bool CanTarget(Player player)
    {
        return true;
    }
}