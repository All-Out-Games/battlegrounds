using AO;

namespace Assembly.Koh;

public class EffectWinner : FightEffect
{
    public override bool IsActiveEffect => true;

    private CameraControl _localControl;

    public bool IsScoreWin;

    private float _easeIn;
    private float _easeOut;

    private static float ZoomFactor = 0.55f;
    private bool _animationPlayed;

    public override void OnEffectStart(bool isDropIn)
    {
        base.OnEffectStart(isDropIn);
        if (isDropIn)
        {
            return; // Don't move camera if the effect is dropped in
        }
        else
        {
            DurationRemaining = 5f;
        }
        _localControl = CameraControl.Create(2);
        
        if (IsScoreWin)
        {
            FightPlayer.SetAnimTrigger("win2");
        }
        else
        {
            FightPlayer.SetAnimTrigger("win1");
        }
    }

    public override void OnEffectUpdate()
    {
        base.OnEffectUpdate();
        if (!FightPlayer.Alive() && _localControl != null)
        {
            _localControl.Destroy();
            return;
        }

        if (_localControl != null)
        {
            Vector2 pos = FightPlayer.Position;
            if (ElapsedTime < 1)
            {
                _localControl.Zoom = Util.Lerp(1, ZoomFactor, ElapsedTime);
                pos = Vector2.Lerp(Network.LocalPlayer.Position, FightPlayer.Position, ElapsedTime);
            }
            else if (ElapsedTime > 4)
            {
                _localControl.Zoom = Util.Lerp(ZoomFactor, 1, ElapsedTime - 4);
                pos = Vector2.Lerp(FightPlayer.Position, Network.LocalPlayer.Position, ElapsedTime - 4);
            }
            _localControl.Position = pos + new Vector2(0, 0.5f);
        }
        
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        _localControl.Destroy();
        FightPlayer.SetAnimTrigger("RESET");
    }
}