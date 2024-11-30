using AO;
using Assembly.scripts.SceneObjects.AdCrates;
using Assembly.scripts.SceneObjects.Crates;

public struct AdCrateConfig
{
    public string RewardId;
    public Vector4 Tint;
    public float Chance;
    public string InteractableText;
    public string AdPromptText;
    public string AdPromptTexture;
}
public class AdCrateSpawner : System<AdCrateSpawner>
{
    public Entity[] CrateSpawnLocation; // TODO: Configs for Coin / Glory / XP / XP Booster / Spectral Spawn
    private bool _enabled;
    public static AdCrateConfig[] CrateConfigs = new []
    {
        new AdCrateConfig()
        {
            RewardId = "xpBoost",
            Tint = Vector4.One,
            Chance = 1,
            InteractableText = "XP Booster",
            AdPromptText = "Watch an Ad to claim 5 min 3x XP boost.",
            AdPromptTexture = "Props/DropItems/ExpPotionS.png"
        },
        new AdCrateConfig()
        {
            RewardId = "xp200",
            Tint = Vector4.One,
            Chance = 1,
            InteractableText = "200 XP",
            AdPromptText = "Watch an Ad to claim 200 XP. You get double if you are lower than Lv. 15.",
            AdPromptTexture = "Props/DropItems/ExpPotionM.png"
        }
    };
    

    private float _crateAdSpawnTimer = 0;

    public override void Update()
    {
        base.Update();
        if (Network.IsServer)
        {
            _crateAdSpawnTimer += Time.DeltaTime;
            if (_crateAdSpawnTimer > GlobalData.AdCrateSpawnTime)
            {
                _crateAdSpawnTimer = 0;
                SpawnAdCrate();
            }
        }

        
    }

    public override void Awake()
    {
        base.Awake();
        if (Network.IsServer)
        {
            Ads.SetRewardHandler(AdRewardHandler);
        }
    }

    private bool AdRewardHandler(Player p, string id)
    {
        FightPlayer player = p as FightPlayer;
        if (player.Alive())
        {
            int lv = player.Level;
            switch (id)
            {
                case "xpBoost":
                    player.AddExpBoostTime(5, 3);
                    break;
                case "xp200":
                    int xp = 200;
                    if (lv < 15)
                    {
                        xp *= 2;
                    }

                    player.Exp += xp;
                    break;
            }
        }
        return false;
    }

    private void SpawnAdCrate()
    {
        CrateSpawnLocation ??= FightClubGameManager.References.SceneAdCrateSpawnLocations;
        if (CrateSpawnLocation.Length < CrateConfigs.Length)
        {
            Log.Error("Ad Crate has less spawn location configured than AdCrateConfigs! Will not spawn Ad Crates!");
            _enabled = false;
        }
        else
        {
            _enabled = true;
        }
        if (_enabled)
        {
            float chance = Random.Shared.NextFloat();
            for(int i = 0; i < CrateConfigs.Length; i++)
            {
                var cfg = CrateConfigs[i];
                if (chance < cfg.Chance)
                {
                    FightClubGameManager.Instance.ServerSpawn(AdCrate.AdCratePrefab, CrateSpawnLocation[i].Position,
                        entity =>
                        {
                            AdCrate adCrate = entity.GetComponent<AdCrate>();
                            if (adCrate == null)
                            {
                                Log.Error("There is no AdCrate component on AdCrate Prefab! Aborting");
                                entity.Destroy();
                                return;
                            }
                            adCrate.CallClient_Initialization(cfg.Tint, cfg.RewardId, cfg.AdPromptText, cfg.AdPromptTexture, cfg.InteractableText);
                        });
                }
            }
        }
    }
}