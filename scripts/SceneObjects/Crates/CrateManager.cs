using AO;

namespace Assembly.scripts.SceneObjects.Crates;

public class CrateManager : System<CrateManager>
{
    /// <summary>
    /// Server Only List of crates
    /// </summary>
    public int AliveCrateCount;

    public void SpawnCrate()
    {
        if (Network.IsServer)
        {
            // Random pos in combat zone
            Zone combatZone = FightClubGameManager.References.PvpZone;
            FightClubGameManager.Instance.ServerSpawn(Crate.CratePrefab, FightClubUtils.RandomPositionInCircle(combatZone.Entity.Position, combatZone.Entity.LocalScaleX),
                entity =>
                {
                    Crate crt = entity.GetComponent<Crate>();
                    if (!crt.Alive())
                    {
                        Log.Error("Crate Manager: Crate Component Not Found!");
                    }
                });
        }
    }

    private float _crateSpawnTimer = 0;

    public override void Update()
    {
        base.Update();
        _crateSpawnTimer += Time.DeltaTime;
        if (AliveCrateCount <= GetMaxCrateCount() && _crateSpawnTimer > GlobalData.CrateSpawnTime)
        {
            SpawnCrate();
            Log.Warn($"Box Count: {AliveCrateCount}");
            _crateSpawnTimer = 0;
        }
    }

    private int GetMaxCrateCount()
    {
        return Int32.Min(2 * Player.AllPlayers.Count, 10);
    }
}