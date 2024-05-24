
using AO;

/// <summary>
/// Provide some interface to trigger server functions from client, TEST ONLY, DO NOT CALL IN GAMEPLAY
/// </summary>
public partial class TestServerRPC
{
    [ServerRpc]
    public static void AddBumpToNetworkID(ulong netID, Vector2 add)
    {
        FightPlayer targetPly = Entity.FindByNetworkId(netID).GetComponent<FightPlayer>();
        targetPly.AddBumpFrom(targetPly, add, false);
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

    [ServerRpc]
    public static void AddDashToNetworkID(ulong netID, Vector2 add, float duration)
    {
        FightPlayer targetPly = Entity.FindByNetworkId(netID).GetComponent<FightPlayer>();
        targetPly.AddDash_Server(add,duration);
    }
}