using AO;

namespace Assembly.scripts.UI.LevelUp;



public class ChampionInfoWindow : UniqueUIWindow
{
    public static string PreChampionText = "Congratulations! You reached level 50! Fill your exp bar to be promoted to champion and unlock exclusive perks!";
    public static string ChampionHint = "You are now a Champion! Play honorably and only target higher-level players, you will receive glory for each kill!"; 
    [Serialized] public UIText RemainingXp;
    [Serialized] public UIText ChampionText;
    [Serialized] public Entity TogoEntity;

    public void SetLocalChampionData()
    {
        var fp = Network.LocalPlayer as FightPlayer;
        if (!fp.Alive())
        {
            return;
        }

        if (fp.IsChampion)
        {
            ChampionText.Text = ChampionHint;
            TogoEntity.LocalEnabled = false;
            RemainingXp.Text = "0";
        }
        else
        {
            ChampionText.Text = PreChampionText;
            TogoEntity.LocalEnabled = true;
            RemainingXp.Text = $"{LevelingData.NextLevelXp[LevelingData.MaxLevel] - fp.Exp}";
        }
    }
}