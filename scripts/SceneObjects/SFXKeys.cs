using AO;

namespace Assembly.scripts.SceneObjects;

public static class SFXKeys
{
    public static AudioAsset BearTrapSetAudio = Assets.GetAsset<AudioAsset>("SFX/Objects/bear_trap-appear_set_up.wav");
    public static AudioAsset BearTrapSnapAudio = Assets.GetAsset<AudioAsset>("SFX/Objects/bear_trap-snap_closed.wav");

    public static AudioAsset HealingLoopAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/healing_loop.wav");
    
    public static AudioAsset ShurikenLoopAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/kunai_shuriken_projectile_loop.wav");
    public static AudioAsset PsyboltLoopAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/psybolt_projectile_loop.wav");
    public static AudioAsset PsyboltHitAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/psybolt_hit.wav");
}