using AO;

namespace Assembly.scripts.SceneObjects;

public static class SFXKeys
{
    public static AudioAsset BearTrapSetAudio = Assets.GetAsset<AudioAsset>("SFX/Objects/bear_trap-appear_set_up.wav");
    public static AudioAsset BearTrapSnapAudio = Assets.GetAsset<AudioAsset>("SFX/Objects/bear_trap-snap_closed.wav");

    public static AudioAsset HealingStartAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/heal_start.wav");
    public static AudioAsset HealingLoopAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/healing_loop.wav");
    public static AudioAsset HealingEndAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/heal_end.wav");
    
    public static AudioAsset ProjectileLThrowAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/throw_weapon.wav");
    public static AudioAsset ProjectileLoopAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/sound_wave_projectile_loop.wav");
    public static AudioAsset ShurikenLoopAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/kunai_shuriken_projectile_loop.wav");
    
    
    public static AudioAsset PsyboltShootAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/psybolt_shoot.wav");
    public static AudioAsset PsyboltLoopAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/psybolt_projectile_loop.wav");
    public static AudioAsset PsyboltHitAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/psybolt_hit.wav");
    
    public static AudioAsset BefuddleThrowAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/confusion_ball.wav");
    
    public static AudioAsset PsiRayAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/pisonic_beam_loop.wav");

    public static AudioAsset InvisAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/invisibility_activate.wav");
}