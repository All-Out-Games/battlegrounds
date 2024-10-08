using AO;

namespace Assembly.scripts.VFX;

public static class VFXPrefabKeys
{
    // All prefabs here must contain a BaseVFX class or derived class
    
    public static string SelfDestructExplosionPath = "SelfDestructExplosion.prefab";
    public static string ClawSlashVFXPath = "ClawSwipe.prefab";
    
    // VFX
    
    public static string HitVfxPath = "HitVfx.prefab";
    public static string BefuddleHitVfxPath = "Befuddle_Hit.prefab";
    public static string PsionicBeamExplosionPath = "PsionicBeamExplosion.prefab";
    public static string PsionicRayPath = "PsionicRay.prefab";
    public static string ShadowStepVfxPath = "ShadowStepVFX.prefab";
    public static string LeapSlamCraterVfxPath = "LeapSlamCrater.prefab";
    public static string BattleCryVfxPath = "BattleCry_Shockwave.prefab";
    public static string InvisibilityVfxPath = "Invisibility_VFX.prefab";
    
    // Aura (AttachmentObjects)
    public static string RageAuraPath = "Rage_Aura.prefab";
    public static string RegenerationAuraPath = "RegenerationAura.prefab";
    public static string BloodSplurtPath = "BloodVFX.prefab";
    public static string ShieldVFXPath = "Shield_FX.prefab";
    public static string SpikeShieldVFXPath = "SpikeShield_FX.prefab";
    public static string GravityCrushVFXPath = "GravityCrushVFX.prefab";
    public static string InvisibilityAuraPath = "Invisibility_Aura.prefab";
    public static string StatAuraPath = "Stat_Aura.prefab";
}

public static class VFXPrefabs
{
    public static Prefab HitVFX = Assets.GetAsset<Prefab>(VFXPrefabKeys.HitVfxPath);
    public static Prefab BefuddleHitVFX = Assets.GetAsset<Prefab>(VFXPrefabKeys.BefuddleHitVfxPath);
    public static Prefab PsionicBeamHitVFX = Assets.GetAsset<Prefab>(VFXPrefabKeys.PsionicBeamExplosionPath);
    public static Prefab PsionicRayVFX = Assets.GetAsset<Prefab>(VFXPrefabKeys.PsionicRayPath);
    public static Prefab ShadowStepVFX = Assets.GetAsset<Prefab>(VFXPrefabKeys.ShadowStepVfxPath);
    public static Prefab InvisibilityVFX = Assets.GetAsset<Prefab>(VFXPrefabKeys.InvisibilityVfxPath);

    public static Prefab RegenerationAura = Assets.GetAsset<Prefab>(VFXPrefabKeys.RegenerationAuraPath);
    public static Prefab RageAura = Assets.GetAsset<Prefab>(VFXPrefabKeys.RageAuraPath);
    public static Prefab StatAura = Assets.GetAsset<Prefab>(VFXPrefabKeys.StatAuraPath);
    public static Prefab InvisibilityAura = Assets.GetAsset<Prefab>(VFXPrefabKeys.InvisibilityAuraPath);
    public static Prefab BloodSplurt = Assets.GetAsset<Prefab>(VFXPrefabKeys.BloodSplurtPath);
    public static Prefab ShieldFx = Assets.GetAsset<Prefab>(VFXPrefabKeys.ShieldVFXPath);
    public static Prefab SpikeShieldFx = Assets.GetAsset<Prefab>(VFXPrefabKeys.SpikeShieldVFXPath);
    public static Prefab GravityCrushFx = Assets.GetAsset<Prefab>(VFXPrefabKeys.GravityCrushVFXPath);
}