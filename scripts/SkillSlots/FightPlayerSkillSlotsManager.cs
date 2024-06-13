using AO;

public partial class FightPlayerSkillSlotsManager : FightPlayerComponent
{

    private bool _localDrawAbility;
    private List<FightAbility> ActiveAbilities = new List<FightAbility>();
    private string[] _equippedSkillKeys = new string[6];
    
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
        if (Network.IsServer)
        {
            _equippedSkillKeys[0] = "Punch";
            for (int i = 1; i < 6; i++)
            {
                CallClient_SyncEquippedSkills(i, Save.GetString(_player, $"SkillSlot{i}", "Empty"));
            }
        }
        
        if (_player.IsLocal)
        {
            SkillSlotsPanelEnable(false);
            // TODO: Ability book & Load Slot from save
            ActiveAbilities.Add(_player.GetFightAbility<AbilityPunch>());
            for (int i = 1; i < 6; i++)
            {
                ActiveAbilities.Add(GetAbilityInstance(FightAbility.AbilityQueryDict[_equippedSkillKeys[i]]));
            }
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

    public FightAbility GetAbilityInstance(Type f)
    {
        return _player.GetFightAbility(f);
    }
    
    public void ReplaceSlot(int index, FightAbility faInstanc)
    {
        ActiveAbilities[index] = faInstanc;
    }

    [ServerRpc]
    public void SetSavedSkillSlot(int index, string skillKey)
    {
        if (index < 0 || index > 5 || !SkillConfig.GetAllSkillKeys().Contains(skillKey))
        {
            return;
        }
        if(Network.IsServer) Save.SetString(_player, $"SkillSlot{index}", skillKey);
    }

    [ClientRpc] 
    public void SyncEquippedSkills(int index, string skillKey)
    {
        _equippedSkillKeys[index] = skillKey;
    }

    public List<FightAbility> GetCurrentAbilities()
    {
        return ActiveAbilities;
    }
}