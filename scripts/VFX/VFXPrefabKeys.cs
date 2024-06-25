using AO;

namespace Assembly.scripts.VFX;

public static class VFXPrefabKeys
{
    // All prefabs here must contain a BaseVFX class or derived class
    
    public static string SelfDestructExplosionPath = "SelfDestructExplosion.prefab";
    public static string ClawSlashVFXPath = "ClawSwipe.prefab";
    
    public static string HitVfxPath = "HitVfx.prefab";
    public static string PsionicBeamExplosionPath = "PsionicBeamExplosion.prefab";
}

public static class VFXPrefabs
{
    public static Prefab HitVFX = Assets.GetAsset<Prefab>(VFXPrefabKeys.HitVfxPath);
    public static Prefab PsionicBeamHitVFX = Assets.GetAsset<Prefab>(VFXPrefabKeys.PsionicBeamExplosionPath);
}