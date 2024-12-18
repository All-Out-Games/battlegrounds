
using Assembly.scripts.UI;

namespace Assembly.scripts.Effects;
using AO;

public class EffectAdWatched : AEffect
{
    public override bool IsActiveEffect => false;
    public string Info = "Ads reward received.";
    public bool _poped;

    public override void OnEffectStart(bool isDropIn)
    {
        //base.OnEffectStart(isDropIn);
        if (!isDropIn)
        {
            DurationRemaining = 10f;
        }
        
    }

    public override void OnEffectUpdate()
    {
        if (Util.OneTime(ElapsedTime > 1, ref _poped))
        {
            if (Player.IsLocal)
            {
                Notifications.Show(Info);
                var wd = UIManager.Instance.GetOverlayWindow<ResourceOverlayWindow>(UniqueWindowKeys.ResourcesOverlayWindowPath);
                wd.PopSparkles();
            }
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        
    }

    public override void NetworkSerialize(StreamWriter writer)
    {
        base.NetworkSerialize(writer);
        writer.WriteString(Info);
    }

    public override void NetworkDeserialize(StreamReader reader)
    {
        base.NetworkDeserialize(reader);
        Info = reader.ReadString();
    }
}