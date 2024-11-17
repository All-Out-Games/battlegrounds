using System.Collections;
using AO;

namespace Assembly.scripts.SceneObjects;

public class ReticleObject : Component
{
    [Serialized] public Spine_Animator Animator;
    [Serialized] public Entity Reticle;

    public override void Awake()
    {
        if (Animator == null)
        {
            Log.Error($"Animator is not assigned for {Entity.Name}!");
            Despawn();
        }

        if (Reticle == null)
        {
            Log.Error($"Reticle is not assigned for {Entity.Name}!");
            Despawn();
        }
        base.Awake();
    }

    IEnumerator LerpReticleSize(Vector2 start, Vector2 end, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            Vector2 s = new Vector2(Util.Lerp(start.X, end.X, t), Util.Lerp(start.Y, end.Y, t));
            t += Time.DeltaTime;
            Reticle.LocalScale = s;
            yield return null;
        }
        yield return null;
    }

    public void Despawn()
    {
        Entity.Destroy();
    }

    public void PlayReticleLerpAnimation(Vector2 start, Vector2 end, float duration)
    {
        Coroutine.Start(Entity, LerpReticleSize(start, end, duration));
    }

    public void SetAnimation(string aName, bool loop)
    {
        Animator.SpineInstance.SetAnimation(aName, loop);
    }
}