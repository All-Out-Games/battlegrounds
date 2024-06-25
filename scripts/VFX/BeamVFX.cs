using AO;
namespace Assembly.scripts.VFX;

public class BeamVFX : BaseVFX
{
    public void SetTransformConstraintPosition(string name, Vector2 value)
    {
        Animator.SpineInstance.SetTransformConstraintPosition(name, value);
    }
    
    public void SetTransformConstraintRotation(string name, float value)
    {
        Animator.SpineInstance.SetTransformConstraintRotation(name, value);
    }

    public void SetBonePosition(string name, Vector2 value)
    {
        Animator.SpineInstance.SetBonePosition(name, value);
    }
    
}   