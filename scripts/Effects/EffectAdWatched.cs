
using Assembly.scripts.UI;

namespace Assembly.scripts.Effects;
using AO;

public class EffectAdWatched : AEffect
{
    public override bool IsActiveEffect => false;
    public string Info = "Ads reward received.";

    public override void OnEffectStart(bool isDropIn)
    {
        //base.OnEffectStart(isDropIn);
        if (!isDropIn)
        {
            DurationRemaining = 10f;
        }
        if (Player.IsLocal)
        {
            Notifications.Show(Info);
            var wd = UIManager.Instance.GetOverlayWindow<ResourceOverlayWindow>(UniqueWindowKeys.ResourcesOverlayWindowPath);
            wd.PopSparkles();
        }
    }

    public override void OnEffectUpdate()
    {
        
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