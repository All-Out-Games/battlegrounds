

public abstract class SkillTreeNode
{
    public enum NodeType
    {
        AttrBoost,
        SkillUnlock,
        SkillReplace,
        SkillEnhance
    }
    
    protected int Level;
    protected NodeType NType;

    public int GetLevel()
    {
        return Level;
    }

    public NodeType GetNodeType()
    {
        return NType;
    }
    
    public abstract void Initialization(); // Method called when the skill tree is loaded. We perform a BFS from the root node and put all nodes to a queue
    public abstract void HandleEffect(); // Method called after the entire skill tree is loaded. i.e. the nodes take effect on this stage
}