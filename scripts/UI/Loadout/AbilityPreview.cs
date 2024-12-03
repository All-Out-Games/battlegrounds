using AO;

namespace Assembly.scripts.UI.Loadout;

public class AbilityPreview : Component
{
    [Serialized] private UIGif _previewGif;
    [Serialized] private Entity _nopreviewScreen;

    public void SetPreview(string previewPath = "Empty")
    {
        _nopreviewScreen.LocalEnabled = true;
        _previewGif.Entity.LocalEnabled = false;
        return;
        if (previewPath == "Empty")
        {
            _nopreviewScreen.LocalEnabled = true;
            _previewGif.Entity.LocalEnabled = false;
        }
        else
        {
            _nopreviewScreen.LocalEnabled = false;
            _previewGif.Entity.LocalEnabled = true;
            var gif = Assets.GetAsset<Gif>(previewPath);
            if (gif == null)
            {
                Log.Warn($"{previewPath} is specified, but GIF asset is not found!");
                _nopreviewScreen.LocalEnabled = true;
                _previewGif.Entity.LocalEnabled = false;
            }
            else
            {
                _previewGif.Gif = gif;
            }
        }
    }
}