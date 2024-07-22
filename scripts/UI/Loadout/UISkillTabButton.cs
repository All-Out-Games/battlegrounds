using AO;

namespace Assembly.scripts.UI;

public class UISkillTabButton : Component
{

    private int _index;

    public Action<int> OnTabButtonClicked;

    public void Initialize(int idx, Action<int> onIndexedTabClicked)
    {
        _index = idx;
        OnTabButtonClicked += onIndexedTabClicked;

        UIButton btn = Entity.GetComponent<UIButton>();
        
        if (btn != null)
        {
            btn.OnClicked += () =>
            {
                OnTabButtonClicked.Invoke(_index);
            };
        }
        else
        {
            Log.Error("Skilltab button must have the button component!");
        }
        
    }
}