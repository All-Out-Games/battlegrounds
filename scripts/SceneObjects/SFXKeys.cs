using AO;

namespace Assembly.scripts.SceneObjects;

public static class SFXKeys
{
    public static AudioAsset Punch1Audio = Assets.GetAsset<AudioAsset>("SFX/Effects/punch_small.wav");
    public static AudioAsset Punch2Audio = Assets.GetAsset<AudioAsset>("SFX/Effects/punch_strong.wav");
    public static AudioAsset Punch3Audio = Assets.GetAsset<AudioAsset>("SFX/Effects/punch_strongest.wav");
    

    public static AudioAsset GetPunchSFXByLevel(int lvl)
    {
        if (lvl > 1)
        {
            return Punch2Audio;
        }

        /*
        if (lvl == 3)
        {
            return Punch3Audio; // Mas said this is annoying
        }*/

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
    
    public static AudioAsset ClawSlashAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/claw_swipe.wav");
    public static AudioAsset BackStabTeleportAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/backstab_teleport.wav");
    public static AudioAsset BackStabCasterAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/backstab_attack.wav");
    public static AudioAsset BackStabVictimAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/backstab_victim.wav");
    
    public static AudioAsset GroundStompAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/ground_stomp.wav");
    public static AudioAsset RageAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/rage_stomp.wav");
    public static AudioAsset SelfDestructAudio =Assets.GetAsset<AudioAsset>("SFX/Effects/self_destruct.wav");
    
    public static AudioAsset RolloutStartAudio =Assets.GetAsset<AudioAsset>("SFX/Effects/rollout_start.wav");
    public static AudioAsset RolloutLoopAudio =Assets.GetAsset<AudioAsset>("SFX/Effects/rollout_loop.wav");
    public static AudioAsset RolloutEndAudio =Assets.GetAsset<AudioAsset>("SFX/Effects/rollout_end.wav");
    
    public static AudioAsset HypnotizeAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/hypnotize.wav");
    public static AudioAsset HypnotizeGetupAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/knocked_down_getup.wav");
    
    
    public static AudioAsset PsyThrowStart = Assets.GetAsset<AudioAsset>("SFX/Effects/psythrow_attack_start.wav");
    public static AudioAsset PsyThrowLoop = Assets.GetAsset<AudioAsset>("SFX/Effects/psythrow_attack_loop.wav");
    public static AudioAsset PsyThrowEnd = Assets.GetAsset<AudioAsset>("SFX/Effects/psythrow_attack_throw.wav");
    public static AudioAsset PsyThrowVictim = Assets.GetAsset<AudioAsset>("SFX/Effects/psythrow_victim_start.wav");
    
}