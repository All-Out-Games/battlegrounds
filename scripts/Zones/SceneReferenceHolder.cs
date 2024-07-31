using AO;

/// <summary>
/// This class provide references to Scene Networked Entities and send itself as interface to FightClubGameManager
/// </summary>
public class SceneReferenceHolder : Component
{
    public override void Awake()
    {
        if (FightClubGameManager.References == null)
        {
            FightClubGameManager.References = this;
        }
        else
        {
            Log.Error("You have more than one instances of SceneReferenceHolder in scenes! Some references might break");
        }
        
    }

    [Serialized] public Zone CentralHubZone;
    [Serialized] public Zone PvpZone;
    [Serialized] public Entity TotalDarknessOverlay;
    [Serialized] public Edge_Collider PvpZoneEdge;
}