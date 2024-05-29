using AO;
using Assembly.scripts.SkillSlots.Abilities;

public partial class FightPlayerSkillSlotsManager : FightPlayerComponent
{

    private bool _localDrawAbility;
    private List<Ability> ActiveAbilities = new List<Ability>();
    public bool AllSilent;
    
    public override void Update()
    {
        if (_player.IsLocal)
        {
            if (_localDrawAbility)
            {
                _player.DrawDefaultAbilityUI(new Player.AbilityDrawOptions()
                {
                    Abilities = ActiveAbilities.ToArray(),
                    AbilityElementSize = 100
                });
            }
        }
        
    }

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        if (_player.IsLocal)
        {
            SkillSlotsPanelEnable(false);
            ActiveAbilities.Add(_player.GetAbility<AbilityPunch>());
            ActiveAbilities.Add(_player.GetAbility<AbilityRollOut>());
            ActiveAbilities.Add(_player.GetAbility<AbilityShoulderCrash>());
            ActiveAbilities.Add(_player.GetAbility<AbilityShield>());
            ActiveAbilities.Add(_player.GetAbility<AbilitySpoonThrow>());
        }
    }


    public void InitKeybind()
    {
        
    }
    

    public void AssignKeybindToSlot(Keybind slotKeybind, string mainKey)
    {
        
    }

    public void UpdateSlot(string mainKey, int level, string skillKey = null)
    {

    }

    public void RemoveSlot(string mainKey)
    {

    }

    [ClientRpc]
    public void SilentAllSlot(bool silent)
    {
        AllSilent = silent;
        /*foreach (var kv in ActiveSkillSlots)
        {
            kv.Value.SilentSlot(silent); // This is stupid
        }*/
    }
    

    public FightPlayer GetPlayer()
    {
        return _player;
    }

    public void SkillSlotsPanelEnable(bool enable)
    {
        if (_player.IsLocal)
        {
            TestServerRPC.LogSomethingOnServer($"Set slot panel status to {enable}, compID = {Id}");
            _localDrawAbility = enable;
        }
    }
}