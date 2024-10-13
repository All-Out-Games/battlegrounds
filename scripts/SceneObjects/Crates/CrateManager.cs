using AO;

namespace Assembly.scripts.SceneObjects.Crates;

public class CrateManager : System<CrateManager>
{
    /// <summary>
    /// Server Only List of crates
    /// </summary>
    public List<Crate> CrateRegistry = new List<Crate>();
    public int AliveCrateCount;
    

    public void SpawnCrate()
    {
        if (Network.IsServer)
        {
            // Util.Assert(AliveCrateCount == CrateRegistry.Count, "AliveCrateCount == CrateRegistry.Count");
            // Random pos in combat zone
            Zone combatZone = FightClubGameManager.References.PvpZone;
            FightClubGameManager.Instance.ServerSpawn(Crate.CratePrefab, FightClubUtils.RandomPositionInCircle(combatZone.Entity.Position, combatZone.Entity.LocalScaleX),
                entity =>
                {
                    Crate crt = entity.GetComponent<Crate>();
                    if (!crt.Alive())
                    {
                        Log.Error("Crate Manager: Crate Component Not Found!");
                        return;
                    }
                    var cfg = Util.SampleWeightedList(CratesConfig.AllPossibleItems, config => config.Prob, Random.Shared);
                    crt.CallClient_Initialization(cfg.Item2);
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
            //Log.Warn($"Box Count: {AliveCrateCount}");
            _crateSpawnTimer = 0;
        }
    }

    /// <summary>
    /// [Server Only]
    /// </summary>
    /// <param name="crt"></param>
    public void Register(Crate crt)
    {
        if (!CrateRegistry.Contains(crt))
        {
            CrateRegistry.Add(crt);
            AliveCrateCount++;
        }
        else
        {
            Log.Warn($"Crate (nwid {crt.Entity.NetworkId}) registration duplicated!");
        }
    }

    public void Deregister(Crate crt)
    {
        if (!CrateRegistry.Contains(crt))
        {
            Log.Warn($"Crate (nwid {crt.Entity.NetworkId}) does not exist in registry!");
        }
        else
        {
            CrateRegistry.Remove(crt);
            AliveCrateCount--;
        }
    }

    private int GetMaxCrateCount()
    {
        return Int32.Min(2 * Player.AllPlayers.Count, 10);
    }

    public List<Entity> GetCratesEntity()
    {
        return CrateRegistry.Select(item => item.Entity).ToList();
    }

    public List<Crate> OverlapCircleForCrates(Vector2 center, float radius)
    {
        List<Crate> crts = new ();
        foreach (var other in CrateRegistry)
        {
            if (Vector2.Distance(center, other.Entity.Position) < radius)
            {
                crts.Add(other);
            }
        }
        return crts;
    }
}