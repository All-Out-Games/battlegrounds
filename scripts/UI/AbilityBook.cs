using AO;

namespace Assembly.scripts.UI;

public class AbilityBook : UniqueUIWindow
{
    [Serialized] private UIImage Icon0; // Always punch. Cannot be changed
    [Serialized] private UIImage Icon1;
    [Serialized] private UIImage Icon2;
    [Serialized] private UIImage Icon3;
    [Serialized] private UIImage Icon4;
    [Serialized] private UIImage Icon5;

    private UIDirectionalLayout _skillList;
    private UIDirectionalLayoutElement _skillItem;

}