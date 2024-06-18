using AO;

namespace Assembly.scripts.VFX;

public class BaseVFX : Component
{
    [Serialized] protected string StartAnimationStr;
    [Serialized] protected bool Loop;
    [Serialized] protected Spine_Animator Animator;

    public override void Awake()
    {
        base.Awake();
        Animator = Entity.GetComponent<Spine_Animator>();
        Animator.DepthOffset = 3;
    }

    public override void Start()
    {
        base.Start();
        Animator.SpineInstance.SetAnimation("StartAnimationStr", Loop);
    }
}