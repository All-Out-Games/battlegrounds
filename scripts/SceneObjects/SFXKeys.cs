using AO;

namespace Assembly.scripts.SceneObjects;

public static class SFXKeys
{
    #region Skills

    

    
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

    public static AudioAsset HealingStartAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/heal_start.wav");
    public static AudioAsset HealingLoopAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/healing_loop.wav");
    public static AudioAsset HealingEndAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/heal_end.wav");
    
    public static AudioAsset ProjectileLThrowAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/throw_weapon.wav");
    public static AudioAsset ProjectileLoopAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/sound_wave_projectile_loop.wav");
    public static AudioAsset ShurikenLoopAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/kunai_shuriken_projectile_loop.wav");
    public static AudioAsset SpoonHitAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/player_hit_by_spoon.wav");
    public static AudioAsset ShurikenHitAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/player_hit_by_shuriken.wav");
    
    
    public static AudioAsset PsyboltShootAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/psybolt_shoot.wav");
    public static AudioAsset PsyboltLoopAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/psybolt_projectile_loop.wav");
    public static AudioAsset PsyboltHitAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/psybolt_hit.wav");
    
    public static AudioAsset BefuddleThrowAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/confusion_ball.wav");
    public static AudioAsset BefuddleHitAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/player_hit_by_befuddle.wav");
    public static AudioAsset ConfusedLoopAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/confused_loop.wav");
    
    public static AudioAsset FireballHitAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/fire_projectile_hit_explode.wav");
    public static AudioAsset FireballLoopAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/fire_projectile_loop.wav");
    
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
    public static AudioAsset BattleCryAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/rage_shout_v2.wav");
    
    public static AudioAsset SelfDestructAudio =Assets.GetAsset<AudioAsset>("SFX/Effects/self_destruct.wav");
    public static AudioAsset SelfDestructExplodeAudio =Assets.GetAsset<AudioAsset>("SFX/Effects/self_destruct_explode.wav");
    
    public static AudioAsset RolloutStartAudio =Assets.GetAsset<AudioAsset>("SFX/Effects/rollout_start.wav");
    public static AudioAsset RolloutLoopAudio =Assets.GetAsset<AudioAsset>("SFX/Effects/rollout_loop.wav");
    public static AudioAsset RolloutEndAudio =Assets.GetAsset<AudioAsset>("SFX/Effects/rollout_end.wav");
    public static AudioAsset ShoulderCrashAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/punch_strongest.wav");
    public static AudioAsset ShoulderCrashLoopAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/shoulder_crash_loop.wav");
    
    public static AudioAsset HypnotizeAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/hypnotize.wav");
    public static AudioAsset HypnotizeGetupAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/knocked_down_getup.wav");
    
    
    public static AudioAsset PsyThrowStart = Assets.GetAsset<AudioAsset>("SFX/Effects/psythrow_attack_start.wav");
    public static AudioAsset PsyThrowLoop = Assets.GetAsset<AudioAsset>("SFX/Effects/psythrow_attack_loop.wav");
    public static AudioAsset PsyThrowEnd = Assets.GetAsset<AudioAsset>("SFX/Effects/psythrow_attack_throw.wav");
    public static AudioAsset PsyThrowVictim = Assets.GetAsset<AudioAsset>("SFX/Effects/psythrow_victim_start.wav");
    
    public static AudioAsset TotalDarknessAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/darkness_activate.wav");
    
    public static AudioAsset WoodShieldAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/wood_shield_activate.wav");
    public static AudioAsset SpikeShieldAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/spike_shield_activate.wav");
    public static AudioAsset IronAuradAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/light_feet_activate.wav");
    public static AudioAsset LightFeetAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/iron_aura_activate.wav");

    public static AudioAsset GravityCrushAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/darkness_loop.wav");

    public static AudioAsset ParryStartAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/parry_end.wav"); // I feel like parry_end is a better sfx to start the thing
    public static AudioAsset ParryAttackAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/parry_trigger.wav");
    
    public static AudioAsset LightningBoltAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/lightning_bolt.wav");
    public static AudioAsset LightningBoltSummonAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/summon.wav");
    public static AudioAsset WindPunchAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/wind_punch.wav");

    public static AudioAsset SpectralSpawnAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/enter_pvp_arena.wav");

    public static AudioAsset IceStormStartAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/ice_start.wav");
    public static AudioAsset IceStormLoopAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/ice_loop.wav");
    public static AudioAsset IceStormEndAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/ice_end.wav");
    public static AudioAsset IceProjectileHitAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/ice_projectile_hit.wav");
    public static AudioAsset IceHitAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/ice_hit.wav");
    
    public static AudioAsset ChargingStationAppearAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/charging_station_appear.wav");
    public static AudioAsset ChargingStationExplodeAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/charging_station_explosion.wav");
    public static AudioAsset ChargingStationChargeAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/charging_station_activated.wav");

    public static AudioAsset FoSPrepareAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/FoS_pre.wav");
    public static AudioAsset FoSChargeAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/FoS_charge.wav");
    public static AudioAsset FoSHitAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/FoS_hit.wav");
    
    public static AudioAsset Katana1Audio = Assets.GetAsset<AudioAsset>("SFX/Effects/katana_slash_01.wav");
    public static AudioAsset Katana2Audio = Assets.GetAsset<AudioAsset>("SFX/Effects/katana_slash_02.wav");
    public static AudioAsset Katana3Audio = Assets.GetAsset<AudioAsset>("SFX/Effects/katana_slash_03.wav");
    public static List<AudioAsset> KatanaSlashes = new() {Katana1Audio, Katana2Audio, Katana3Audio};
    public static AudioAsset IllusionSlashAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/illusion_slash_wave.wav");

    public static AudioAsset GetRandomKatanaSound()
    {
        return KatanaSlashes.GetRandom();
    }
    
    public static AudioAsset BladeStorm1Audio = Assets.GetAsset<AudioAsset>("SFX/Effects/bladestorm_hit_01.wav");
    public static AudioAsset BladeStorm2Audio = Assets.GetAsset<AudioAsset>("SFX/Effects/bladestorm_hit_02.wav");
    public static AudioAsset BladeStorm3Audio = Assets.GetAsset<AudioAsset>("SFX/Effects/bladestorm_hit_03.wav");
    public static List<AudioAsset> BladeStormCuts = new() { BladeStorm1Audio, BladeStorm2Audio, BladeStorm3Audio };
    public static AudioAsset BladeStormSwingAudio = Assets.GetAsset<AudioAsset>("SFX/Effects/bladestorm_swing.wav");

    public static AudioAsset GetRandomBladeStormSound()
    {
        return BladeStormCuts.GetRandom();
    }
    
    #endregion

    #region UI

    public static AudioAsset SkillShopAudio = Assets.GetAsset<AudioAsset>("SFX/UI/open_shop.wav");
    public static AudioAsset SkillPageAudio = Assets.GetAsset<AudioAsset>("SFX/UI/open_skill_menu.wav");
    public static AudioAsset AFKAudio = Assets.GetAsset<AudioAsset>("SFX/UI/gain_exp_afk_area.wav");
    public static AudioAsset EliminationAudio = Assets.GetAsset<AudioAsset>("SFX/UI/kill_elimination.wav");

    #endregion

    #region Objects

    public static AudioAsset BearTrapSetAudio = Assets.GetAsset<AudioAsset>("SFX/Objects/bear_trap-appear_set_up.wav");
    public static AudioAsset BearTrapSnapAudio = Assets.GetAsset<AudioAsset>("SFX/Objects/bear_trap-snap_closed.wav");

    public static AudioAsset CrateHitAudio = Assets.GetAsset<AudioAsset>("SFX/Objects/crate-hit.wav");
    public static AudioAsset CrateAppearAudio = Assets.GetAsset<AudioAsset>("SFX/Objects/crate-appear.wav");
    public static AudioAsset CrateBreakAudio = Assets.GetAsset<AudioAsset>("SFX/Objects/crate-break.wav");

    #endregion

    #region KoH

    public static AudioAsset CountdownAudio = Assets.GetAsset<AudioAsset>("SFX/UI/contest_countdown.wav");
    public static AudioAsset LoadoutOpenAudio = Assets.GetAsset<AudioAsset>("SFX/UI/ui_open_shop.wav");
    public static AudioAsset LoadoutCloseAudio = Assets.GetAsset<AudioAsset>("SFX/UI/ui_insufficient_funds.wav");

    #endregion
}