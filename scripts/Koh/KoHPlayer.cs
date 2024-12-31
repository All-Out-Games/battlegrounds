
using Assembly.Koh;
using AO;
using TinyJson;

public partial class FightPlayer
{
    #region KoH Score

    private SyncVar<int> _roundScore = new SyncVar<int>(0);

    public int RoundScore
    {
        get => _roundScore.Value;
        set
        {
            if (Network.IsServer)
            {
                _roundScore.Set(value);
            }
        }
    }
    
    private SyncVar<int> _kingScore = new SyncVar<int>(0);

    public int KingScore
    {
        get => _kingScore.Value;
        set
        {
            if (Network.IsServer)
            {
                _kingScore.Set(value);
            }
        }
    }

    #endregion

    #region KoH Class and Skills

    private SyncVar<string> _serializedPlayerSkillPackage = new SyncVar<string>("");

    public string SerializedPlayerSkillPackage
    {
        get => _serializedPlayerSkillPackage.Value;
        set
        {
            if (Network.IsServer)
            {
                _serializedPlayerSkillPackage.Set(value);
            }
        }
    }

    public SkillPackage PlayerSkillPackage;

    private SyncVar<int> _playerClassId = new SyncVar<int>(-1); // None = -1, Random = 0, other classes are 1, 2, ...

    public int PlayerClassId
    {
        get => _playerClassId.Value;
        set
        {
            if (Network.IsServer)
            {
                _playerClassId.Set(value);
            }
        }
    }

    #endregion
    
    

    /// <summary>
    /// [Server Only] Generate a skill package for this player.
    /// When a player joins this server, we also check that if the server (KoHManager) has the cache of this field.
    /// Because we don't want player quit / rejoin to reroll. They need to buy the reroll!
    /// This function will also be called when the player request a reroll, and when the round concludes.
    /// </summary>
    public string GetSkillPackage()
    {
        PlayerSkillPackage = KohClassData.GenerateSkillPackage();
        SerializedPlayerSkillPackage = PlayerSkillPackage.ToJson();
        return SerializedPlayerSkillPackage;
    }

    private void KohAwake()
    {
        if (Network.IsServer)
        {
            // Routine 1: Check Skill Packages
            if (KohManager.Instance.PlayerSkillPackages.TryGetValue(Name, out var value))
            {
                SerializedPlayerSkillPackage = value;
                PlayerSkillPackage = SerializedPlayerSkillPackage.FromJson<SkillPackage>();
            }
            else
            {
                // First time joining this server this round
                // PlayerClass is cleared when round ends, then generated again for players on the server
                string skillPkgJson = GetSkillPackage();
                KohManager.Instance.PlayerSkillPackages[Name] = skillPkgJson;
            }
            //  Originally planned to save player classes too, but I feel like I don't have to. If they quit then rejoin, they lost all the scores.
        }
    }

    private void KohLazyInit()
    {
        _serializedPlayerSkillPackage.OnSync += OnSkillPackageSync;
        _playerClassId.OnSync += OnClassIdSync;
    }

    private void OnSkillPackageSync(string spo, string spn)
    {
        if (spn.IsNullOrEmpty())
        {
            PlayerSkillPackage = new SkillPackage();
            return;
        }
        PlayerSkillPackage = spn.FromJson<SkillPackage>();
        SkillSlotsManager.KoHSkillReady();
        KohClassData.DebugSkillPackage(this);
    }

    private void OnClassIdSync(int idxo, int idxn)
    {
        SkillSlotsManager.KoHEquipClass(idxn); // Called on client, when the player equip a class or reconnects, skills are equipped for them.
    }
    
}