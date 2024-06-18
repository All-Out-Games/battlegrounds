
using AO;

/// <summary>
/// Provide some interface to trigger server functions from client, TEST ONLY, DO NOT CALL IN GAMEPLAY
/// </summary>
public partial class TestServerRPC
{
    [ServerRpc]
    public static void AddBumpToNetworkID(ulong netID, Vector2 add)
    {
        Entity entity = Entity.FindByNetworkId(netID);
        if(entity == null) return;
        FightPlayer targetPly = entity.GetComponent<FightPlayer>();
        if(targetPly.IsAdmin) targetPly.AddBumpFrom(targetPly, add, false);
    }

    [ClientRpc]
    public static void LogSomethingOnClient(string msg)
    {
        Log.Info($"From Server: {msg}");
    }

    [ServerRpc]
    public static void LogSomethingOnServer(string msg)
    {
        Log.Info($"From Client: {msg}");
    }
}