public class FightPlayerSkillSlots : FightPlayerComponent
{

    protected Dictionary<string, SkillSlot> ActiveSkillSlots = new(); // A slot becomes active if the player purchased the first active skill

    public override void Awake()
    {
        // Technically we can use reflection to derive type here but that might cause cross-platform issues... So hard code for now
        // Slots are finite anyways.
        var punch = new PunchSkillSlot();
        punch.InitSlot("Punch", this);
        ActiveSkillSlots["Punch"] = punch;
        
        // TODO
        var rollout = new PunchSkillSlot();
        rollout.InitSlot("RollOut", this);
        ActiveSkillSlots["RollOut"] = rollout;
    }
    

    public void UpdateSlot(string mainKey, int level, string skillKey = null)
    {
        ActiveSkillSlots[mainKey].AssignSkill(skillKey ?? mainKey, level);
    }

    public FightPlayer GetPlayer()
    {
        return _player;
    }
}