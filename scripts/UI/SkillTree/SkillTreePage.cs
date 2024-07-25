using AO;

namespace Assembly.scripts.UI.SkillTree;

public class SkillTreePage : UniqueUIWindow
{
    public static Vector2 GetGridPosition(int x, int y)
    {
        return new Vector2(180*(x-1), 600-150*y);
    }

    [Serialized] private Entity _skillPageParent;
    [Serialized] private AbilityInfoScreen _infoScreen;
}