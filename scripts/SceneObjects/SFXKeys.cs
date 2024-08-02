using AO;

namespace Assembly.scripts.SceneObjects;

public static class SFXKeys
{
    public static AudioAsset Punch1Audio = Assets.GetAsset<AudioAsset>("SFX/Effects/punch_small.wav");
    public static AudioAsset Punch2Audio = Assets.GetAsset<AudioAsset>("SFX/Effects/punch_strong.wav");
    public static AudioAsset Punch3Audio = Assets.GetAsset<AudioAsset>("SFX/Effects/punch_strongest.wav");
    

    public static AudioAsset GetPunchSFXByLevel(int lvl)
    {
        if (lvl == 2)
        {
            return Punch2Audio;
        }

        if (lvl == 3)
        {
            return Punch3Audio;
        }

        return Punch1Audio;
    }
    
    public static AudioAsset DoublePunchAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/punch_double.wav");
    public static AudioAsset IcePunchAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/punch_ice.wav");
    
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
    
    public static AudioAsset LeapSlamAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/leaping_slam.wav");
    public static AudioAsset LeapSlamKnockAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/knocked_down2.wav");
    
}