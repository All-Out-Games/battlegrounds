using AO;

namespace Assembly.scripts.UI.Loadout;

public class AbilityPreview : Component
{
    [Serialized] private UIImage _previewImg;
    [Serialized] private Entity _nopreviewScreen;

    public void SetPreview(string previewPath = "Empty")
    {
        //_nopreviewScreen.LocalEnabled = true;
        //_previewImg.Entity.LocalEnabled = false;
        if (previewPath == "Empty")
        {
            _nopreviewScreen.LocalEnabled = true;
            _previewImg.Entity.LocalEnabled = false;
        }
        else
        {
            _nopreviewScreen.LocalEnabled = false;
            _previewImg.Entity.LocalEnabled = true;
            var gif = Assets.GetAsset<Texture>(previewPath); // This used to be a gif, but it took too much memory...
            if (gif == null)
            {
                Log.Warn($"{previewPath} is specified, but GIF asset is not found!");
                _nopreviewScreen.LocalEnabled = true;
                _previewImg.Entity.LocalEnabled = false;
            }
            else
            {
                _previewImg.Sprite = gif;
            }
        }
    }
}