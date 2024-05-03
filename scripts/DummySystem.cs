
using AO;

public class DummySystem : System<DummySystem> {
    public override void Start() 
    {
        Log.Info("Registering player events");
        Player.OnPlayerJoin += OnPlayerJoin;
        Player.OnPlayerLeave += OnPlayerLeave;
        Log.Info($"Current Directory: {System.IO.Directory.GetCurrentDirectory()}");
    }

    public override void Update() 
    {

    }

    public override void Shutdown() 
    {

    }

    public void OnPlayerJoin(Player player) 
    {
        Log.Info("Player joined!");
        Save.SetInt(player, "test", Save.GetInt(player, "test", 0) + 1);
        
    }

    public void OnPlayerLeave(Player player)
    {
        Log.Info("Player left!");
    }
}
