using System.Collections;
using AO;
using Assembly.scripts.VFX;

namespace Assembly.scripts.SceneObjects.Crates;

public partial class CrateItemDrop : Component
{
    // Drops an item on the ground with a trigger.
    // After 0.5s it will ping nearby players and start to receive OnTriggerEnter
    // We do this because we want to make yoinking other players' drop possible

    public static Prefab DropPrefab = Assets.KeepLoaded<Prefab>("CrateItemDrop.prefab");

    public Action<FightPlayer> OnItemGrant; // Handle function to apply some effect to a player
    [Serialized] private Circle_Collider _pickupTrigger;
    [Serialized] private Sprite_Renderer _renderer;
    [Serialized] public FadeAfterStart Fade;

    private Vector2 _bump;
    private float _bumpStrength = 3f;
    private bool _activated;

    private CrateDropConfig _config;

    public float TimeElapsed => Fade.ElapsedTime;

    public override void Awake()
    {
        if (Fade == null)
        {
            Log.Error("ItemDrop: Prefab does not have a Fade Component! Serialize this field in your prefab!");
            Entity.Destroy();
        }
        base.Awake();
        _pickupTrigger.LocalEnabled = false;
    }

    public override void Start()
    {
        base.Start();
        Fade.OnFaded += () =>
        {
            Fade.OnFaded = null;
            if(Network.IsServer) CallClient_Despawn();
        };
    }

    public override void Update()
    {
        base.Update();
        if(Util.OneTime(TimeElapsed > 0.5f, ref _activated))
        {
            ActivateDropSeek();
        }

        Entity.Position += _bump * Time.DeltaTime;
        BumpDecay();
    }

    private void BumpDecay()
    {
        _bump = Vector2.Lerp(_bump, Vector2.Zero, Time.DeltaTime * 2.0f);
    }

    /// <summary>
    /// Assign Drop config. This config will be used to handle initialization as well as the grant function
    /// </summary>
    /// <param name="dropName"></param>
    /// <param name="bumpDir"></param>
    [ClientRpc]
    public void Initialization(string dropName, Vector2 bumpDir)
    {
        if (CratesConfig.CrateDropConfigs.ContainsKey(dropName))
        {
            _config = CratesConfig.CrateDropConfigs[dropName];
        }
        else
        {
            Log.Error($"{dropName} does not exist in CratesConfig!");
            Entity.Destroy();
        }

        _bump = bumpDir * _bumpStrength;
        Entity.Name = $"{Entity.Name}_{dropName}";
        _renderer.Texture = Assets.KeepLoaded<Texture>(_config.DropTexturePath);
    }
    
    [ClientRpc]
    public void Despawn()
    {
        Log.Debug($"Despawn called for {Entity.Name}");
        if (Network.IsServer)
        {
            Network.Despawn(Entity);
            Entity.Destroy();
        }
    }

    public void ActivateDropSeek()
    {
        if (Network.IsServer)
        {
            _pickupTrigger.LocalEnabled = true;
        }
        Log.Warn($"Drop Activated - {Entity.Name}");
    }

    public void StartSeek(FightPlayer fp)
    {
        
    }

    IEnumerator Seek(FightPlayer fp)
    {
        yield return null;
    }
}