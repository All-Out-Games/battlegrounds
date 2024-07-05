using AO;

namespace Assembly.scripts.VFX;

public static class VFXPrefabKeys
{
    // All prefabs here must contain a BaseVFX class or derived class
    
    public static string SelfDestructExplosionPath = "SelfDestructExplosion.prefab";
    public static string ClawSlashVFXPath = "ClawSwipe.prefab";
    
    // VFX
    
    public static string HitVfxPath = "HitVfx.prefab";
    public static string PsionicBeamExplosionPath = "PsionicBeamExplosion.prefab";
    public static string PsionicRayPath = "PsionicRay.prefab";
    public static string ShadowStepVfxPath = "ShadowStepVFX.prefab";
    public static string LeapSlamCraterVfxPath = "LeapSlamCrater.prefab";
    public static string BattleCryVfxPath = "BattleCry_Shockwave.prefab";
    
    // Aura (AttachmentObjects)
    public static string RageAuraPath = "Rage_Aura.prefab";
    public static string RegenerationAuraPath = "RegenerationAura.prefab";
}

public static class VFXPrefabs
{
    public static Prefab HitVFX = Assets.GetAsset<Prefab>(VFXPrefabKeys.HitVfxPath);
    public static Prefab PsionicBeamHitVFX = Assets.GetAsset<Prefab>(VFXPrefabKeys.PsionicBeamExplosionPath);
    public static Prefab PsionicRayVFX = Assets.GetAsset<Prefab>(VFXPrefabKeys.PsionicRayPath);
    public static Prefab ShadowStepVFX = Assets.GetAsset<Prefab>(VFXPrefabKeys.ShadowStepVfxPath);

    public static Prefab RegenerationAura = Assets.GetAsset<Prefab>(VFXPrefabKeys.RegenerationAuraPath);
    public static Prefab RageAura = Assets.GetAsset<Prefab>(VFXPrefabKeys.RageAuraPath);
}