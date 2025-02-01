using AO;
using Assembly.scripts.Effects;
using Assembly.scripts.SceneObjects.AdCrates;
using Assembly.scripts.SceneObjects.Crates;
using Assembly.scripts.UI;

public struct AdCrateConfig
{
    public string RewardId;
    public Vector4 Tint;
    public float Chance;
    public string InteractableText;
    public string AdPromptText;
    public string AdPromptTexture;
}
public partial class AdCrateSpawner : System<AdCrateSpawner>
{
    public Entity[] CrateSpawnLocation;
    public Entity[] CrabSpawnLocation;
    private bool _enabled;
    public static AdCrateConfig[] CrateConfigs = new[]
    {
        new AdCrateConfig()
        {
            RewardId = "xpBoost",
            Tint = Vector4.One,
            Chance = 0.25f,
            InteractableText = "XP Booster",
            AdPromptText = "Watch an Ad to claim 5 min 3x XP boost.",
            AdPromptTexture = "Props/DropItems/ExpPotionS.png"
        },
        new AdCrateConfig()
        {
            RewardId = "xp200",
            Tint = Vector4.One,
            Chance = 0.25f,
            InteractableText = "200 XP",
            AdPromptText = "Watch an Ad to claim 200 XP. You get double if you are lower than Lv. 15.",
            AdPromptTexture = "Props/DropItems/ExpPotionM.png"
        },
        new AdCrateConfig()
        {
            RewardId = "spectral",
            Tint = new Vector4(0.4f, 0f, 0.99f, 1f),
            Chance = 0.18f,
            InteractableText = "Spectral Potion",
            AdPromptText = "Watch an Ad to claim Spectral Potion.",
            AdPromptTexture = "Props/DropItems/SpectralPotion.png"
        }
    };

    public static AdCrateConfig[] CrabConfigs = new[]
    {
        new AdCrateConfig()
        {
            RewardId = "coin100",
            Tint = Vector4.One,
            Chance = 0.18f,
            InteractableText = "100 Coins",
            AdPromptText = "Watch an Ad to claim 100 coins.",
            AdPromptTexture = "Props/Shop/Pack1.png"
        },
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
                SpawnAdCrab();
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
            string info = "";
            int lv = player.Level;
            switch (id)
            {
                case "xpBoost":
                    player.AddExpBoostTime(5, 3);
                    info = "XP Boost Granted!";
                    break;
                case "xp200":
                    int xp = 200;
                    if (lv < 15)
                    {
                        xp *= 2;
                    }
                    info = $"{xp} XP Granted!";
                    player.Exp += xp;
                    break;
                case "spectral":
                    player.SpectralCount += 1;
                    info = "Spectral Potion Granted!";
                    break;
                case "coin100":
                    player.Coins += 100;
                    info = "You caught the crab and sold it for 100 coins!";
                    break;
            }

            //p.AddEffect<EffectAdWatched>(p, 10f, watched => watched.Info = info);
            player.GetEffectMgr().CallClient_AddAdWatchedEffect(info, 10f);
            return true;
        }
        return false;
    }

    private void SpawnAdCrate()
    {
        CrateSpawnLocation = FightClubGameManager.References.SceneAdCrateSpawnLocations;
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
            for (int i = 0; i < CrateConfigs.Length; i++)
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

    private void SpawnAdCrab()
    {
        CrabSpawnLocation = FightClubGameManager.References.SceneAdCrabSpawnLocations;
        if (CrateSpawnLocation.Length < CrabConfigs.Length)
        {
            Log.Error("Ad Crab has less spawn location configured than AdCrateConfigs! Will not spawn Ad Crates!");
            _enabled = false;
        }
        else
        {
            _enabled = true;
        }
        if (_enabled)
        {
            float chance = Random.Shared.NextFloat();
            for (int i = 0; i < CrabConfigs.Length; i++)
            {
                var cfg = CrabConfigs[i];
                if (chance < cfg.Chance)
                {
                    FightClubGameManager.Instance.ServerSpawn(AdCrab.AdCrabPrefab, CrabSpawnLocation[i].Position,
                        entity =>
                        {
                            AdCrab adCrab = entity.GetComponent<AdCrab>();
                            if (adCrab == null)
                            {
                                Log.Error("No AdCrab component found!");
                                entity.Destroy();
                                return;
                            }
                            adCrab.CallClient_Initialization(cfg.Tint, cfg.RewardId, cfg.AdPromptText, cfg.AdPromptTexture, cfg.InteractableText);
                        });
                }
            }
        }
    }


}