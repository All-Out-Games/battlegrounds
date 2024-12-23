using AO;

namespace Assembly.Koh;

public class BattleReportWindow : UniqueUIWindow
{
    [Serialized] private UIText _kingTimeTxt;
    [Serialized] private UIText _scoreTxt;
    [Serialized] private UIText _coinTxt;
    [Serialized] private UIText _expTxt;
    [Serialized] private UIText _gloryTxt;
    [Serialized] private UIText _winTxt;
    [Serialized] private UIText _winnerTxt;
    [Serialized] private UIText _tipTxt;

    public void SetYourReport(int kingTime, int score, KohGlobalData.LastRoundReport rpt, bool win)
    {
        Log.Warn("Report set!");
        _winnerTxt.Text = rpt.Winner;
        _winTxt.Text = win ? "Congratulations! You won!" : "Better luck next time!";
        _kingTimeTxt.Text = $"{kingTime}s";
        _scoreTxt.Text = $"{score}";
        _coinTxt.Text = $"+{rpt.YourCoin}";
        _expTxt.Text = $"+{rpt.YourExp}";
        _gloryTxt.Text = $"+{rpt.YourGlory}";
        _tipTxt.Text = $"{KohGlobalData.Tips[rpt.TipIndex]}";
    }

    public override void OpenWindow()
    {
        base.OpenWindow();
        var rpt = KohManager.Instance.RoundReport;
        var lp = Network.LocalPlayer as FightPlayer;
        if (rpt.HasReport && lp.Alive())
        {
            SetYourReport(lp.KingScore, lp.RoundScore, rpt, rpt.Winner == lp.Name);
        }
        else
        {
            Log.Error("There is not report to show!");
            CloseWindow();
        }
    }
}