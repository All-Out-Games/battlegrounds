using AO;
using Assembly.scripts.SkillSlots.Abilities;

public partial class FightPlayerSkillSlotsManager : FightPlayerComponent
{

    private bool _localDrawAbility;
    private List<FightAbility> ActiveAbilities = new List<FightAbility>();
    public bool AllSilent;
    
    public override void Update()
    {
        if (_player.IsLocal)
        {
            if (_localDrawAbility)
            {
                _player.DrawDefaultAbilityUI(new Player.AbilityDrawOptions()
                {
                    Abilities = GetAbilityArray(),
                    AbilityElementSize = 100
                });
            }
        }
        
    }
    

    public override void Start()
    {
        if (_player.IsLocal)
        {
            SkillSlotsPanelEnable(false);
            // TODO: Ability book & Load Slot from save
            ActiveAbilities.Add(_player.GetFightAbility<AbilityPunch>());
            ActiveAbilities.Add(_player.GetFightAbility<AbilityRollOut>());
            ActiveAbilities.Add(_player.GetFightAbility<AbilityShoulderCrash>());
            ActiveAbilities.Add(_player.GetFightAbility<AbilityShield>());
            ActiveAbilities.Add(_player.GetFightAbility<AbilitySpoonThrow>());
            ActiveAbilities.Add(_player.GetFightAbility<FightAbility>());
        }
    }

    private Ability[] GetAbilityArray()
    {
        Ability[] abi = new Ability[ActiveAbilities.Count];
        for (int i = 0; i < ActiveAbilities.Count; i++)
        {
            abi[i] = ActiveAbilities[i];
        }
        return abi;
    }

    public void SkillSlotsPanelEnable(bool enable)
    {
        if (_player.IsLocal)
        {
            TestServerRPC.LogSomethingOnServer($"Set slot panel status to {enable}, compID = {Id}");
            _localDrawAbility = enable;
        }
    }

    public void RemoveSlot(int index)
    {
        ReplaceSlot<FightAbility>(index); // Replace with an empty ability
    }

    public void ReplaceSlot<T>(int index) where T : FightAbility
    {
        ActiveAbilities[index] = _player.GetFightAbility<T>();
    }

    public List<FightAbility> GetCurrentAbilities()
    {
        return ActiveAbilities;
    }
}