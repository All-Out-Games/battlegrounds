using AO;

namespace Assembly.scripts.SceneObjects;

public class AudioObject : Component
{
    [Serialized] public AudioAsset clip;
    [Serialized] public bool IsLoop;
    [Serialized] public float TimeOutForLoop;

    public ulong SoundId;

    public override void Awake()
    {
        base.Start();
        SoundId = SFX.Play(clip, new SFX.PlaySoundDesc() { Loop = IsLoop, LoopTimeout = TimeOutForLoop, EntityToFollow = Entity});
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        SFX.Stop(SoundId);
    }
}