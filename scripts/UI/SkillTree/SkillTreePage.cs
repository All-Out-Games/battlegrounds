using AO;

namespace Assembly.scripts.UI.SkillTree;

public class SkillTreePage : UniqueUIWindow
{
    public static Vector2 GetGridPosition(int x, int y)
    {
        return new Vector2(80 + 150*x, 600-150*y);
    }
}