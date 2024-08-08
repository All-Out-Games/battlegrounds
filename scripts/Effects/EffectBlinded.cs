using AO;

namespace Assembly.scripts.Effects;

public class EffectBlinded : FightEffect
{
    public override bool IsActiveEffect => false;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        if (FightPlayer.IsLocal)
        {
            FightClubGameManager.References.TotalDarknessOverlay.LocalEnabled = true;
            foreach (var pl in Player.AllPlayers)
            {
                if (!pl.IsLocal)
                {
                    pl.AddNameInvisibilityReason("Blinded");
                }
            }
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        if (FightPlayer.IsLocal)
        {
            FightClubGameManager.References.TotalDarknessOverlay.LocalEnabled = false;
            foreach (var pl in Player.AllPlayers)
            {
                if (!pl.IsLocal)
                {
                    pl.RemoveNameInvisibilityReason("Blinded");
                }
            }
        }
        
    }
}