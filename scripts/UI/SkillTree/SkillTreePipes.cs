using AO;

namespace Assembly.scripts.UI.SkillTree;

public class SkillTreePipes : Component
{
    [Serialized] private Entity _filledPipes;
    private UIRect _rect;

    public override void Awake()
    {
        base.Awake();
        _rect = GetComponent<UIRect>();
    }

    public void SetFilled(bool enable)
    {
        _filledPipes.LocalEnabled = enable;
    }

    /// <summary>
    /// Set the offset of the pipe rect. Should not be called other than when initializing the skill tree
    /// </summary>
    /// <param name="offset">Offset should be [Half a grid interval below the skill icon]</param>
    public void SetOffset(Vector2 offset)
    {
        _rect.Offset = offset;
    }
    
}