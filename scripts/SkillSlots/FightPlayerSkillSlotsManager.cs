using AO;
using Assembly.Koh;
using TinyJson;

public partial class FightPlayerSkillSlotsManager : FightPlayerComponent
{

    private bool _localDrawAbility;
    private List<FightAbility> ActiveAbilities = new List<FightAbility>();
    private string[] _equippedSkillKeys = new string[6];
    public bool Ready = false;
    
    public override void Update()
    {
        if (_player.IsLocal)
        {
            if (_localDrawAbility)
            {
                var opt = new Player.AbilityDrawOptions()
                {
                    Abilities = GetAbilityArray(),
                    AbilityElementSize = 75
                };
                if (opt.Abilities.Length > 0)
                {
                    _player.DrawDefaultAbilityUI(opt);
                }
            }
        }

        if (Util.OneTime(true, ref _lazyInited))
        {
            LazyInit();
        }
        
    }


    private bool _lazyInited;
    public void LazyInit()
    {
        // Not using saved slots in KoH. Moved to KoHSkillReady() below
        /*if (Network.IsServer)
        {
            _equippedSkillKeys[0] = "Punch";
            for (int i = 1; i < 6; i++)
            {
                _equippedSkillKeys[i] = Save.GetString(_player, $"SkillSlot{i}", "Empty");
            }
            _player.SerializedSkillLoadout = _equippedSkillKeys.ToJson(); 
        }*/
        SkillSlotsPanelEnable(false);

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

    public int GetAbilityIndex(Type f)
    {
        var abi = _player.GetFightAbility(f);
        for (int i = 1; i < ActiveAbilities.Count; i++)
        {
            if (ActiveAbilities[i] == abi)
            {
                return i;
            }
        }
        return -1;
    }
    
    /// <summary>
    /// Replace Slot. Note that this function alone does not save the slot.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="faInstanc"></param>
    /// <param name="cooldownAfterReplace"></param>
    public void ReplaceSlot(int index, FightAbility faInstanc, float cooldownAfterReplace = 0)
    {
        // just to be sure
        if (index < 0 || index > ActiveAbilities.Count)
        {
            return;
        }
        ActiveAbilities[index] = faInstanc;
        if (cooldownAfterReplace != 0)
        {
            faInstanc.CooldownRemaining = cooldownAfterReplace;
        }
    }

    [ServerRpc]
    public void SetSavedSkillSlot(int index, string skillKey)
    {
        if (Network.IsServer)
        {
            // Not valid index
            if (index < 0 || index > 5)
            {
                return;
            }
            // Not valid skill key
            if (skillKey != FightAbility.DefaultSkillKey && !SkillConfig.GetAllSkillKeys().Contains(skillKey))
            {
                return;
            }
            _equippedSkillKeys[index] = skillKey;
            Save.SetString(_player, $"SkillSlot{index}", skillKey);
            _player.SerializedSkillLoadout = _equippedSkillKeys.ToJson();
        }
    }
    
    public void SyncCompleted(string skillKeyJson)
    {
        _equippedSkillKeys = skillKeyJson.FromJson<string[]>();
        if (_player.IsLocal)
        {
            ActiveAbilities.Clear();
            ActiveAbilities.Add(_player.GetFightAbility<AbilityPunch>());
            for (int i = 1; i < 6; i++)
            {
                ActiveAbilities.Add(GetAbilityInstance(FightAbility.AbilityQueryDict[_equippedSkillKeys[i]]));
            }

            Ready = true;
        }
    }

    public void KoHSkillReady()
    {
        Ready = true;
    }

    public void OnKoHEnterCombat()
    {
        if (_player.PlayerClassId == -1)
        {
            KoHEquipClass(0); // Equip random class, if the player entered combat with None class
        }
    }

    public void KoHEquipClass(int cidx)
    {
        var sk = _player.GetSkillTree();
        
        // Remove Passives
        if (_player.PlayerClassId > 0) // Current class is not None or Random 
        {
            var currentSkillPkg = KohClassData.GetClassPackage(_player.PlayerClassId);
            sk.StatRemover("Passive", currentSkillPkg.Passive);
        }
        _player.PlayerClassId = cidx;
        ActiveAbilities.Clear();
        // Slot 0 - Always Punch
        _equippedSkillKeys[0] = "Punch";
        ActiveAbilities.Add(_player.GetFightAbility<AbilityPunch>());
        List<string> abilityNames;
        if (cidx == -1)
        {
            // Equip None class (happens to all players when round ends)
            // Don't have to do anything
            _player.GetPlayerUIComp().ClassDisplayName = "";
        }
        else if (cidx == 0)
        {
            // Equip random skills in Player's skill package
            abilityNames = KohClassData.GetRandomSkillKeys(_player.PlayerSkillPackage);
            for (int i = 0; i < 4; i++)
            {
                ActiveAbilities.Add(GetAbilityInstance(FightAbility.AbilityQueryDict[abilityNames[i]]));
            }

            _player.GetPlayerUIComp().ClassDisplayName = "Random";
        }
        else
        {
            var newPkg = KohClassData.GetClassPackage(cidx);
            abilityNames = newPkg.SkillKeys.ToList();
            for (int i = 0; i < 4; i++)
            {
                ActiveAbilities.Add(GetAbilityInstance(FightAbility.AbilityQueryDict[abilityNames[i]]));
            }
            // Passive
            sk.StatAdder(0, "Passive", newPkg.Passive);
            _player.GetPlayerUIComp().ClassDisplayName = newPkg.Name;
        }
    }

    public List<FightAbility> GetCurrentAbilities()
    {
        return ActiveAbilities;
    }

    public string[] GetEquippedSkillkeys()
    {
        return _equippedSkillKeys;
    }
}