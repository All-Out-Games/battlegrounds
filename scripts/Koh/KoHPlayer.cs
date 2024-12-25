
using Assembly.Koh;
using AO;

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
    
    private void KohAwake()
    {
        KoHClassData.KoHSanityCheck();
    }
}