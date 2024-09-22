using AO;
using Assembly.scripts.VFX;

namespace Assembly.scripts.SceneObjects.Crates;

public class CrateItemDrop : Component
{
    // Drops an item on the ground with a trigger.
    // After 0.5s it will ping nearby players and start to receive OnTriggerEnter
    // We do this because we want to make yoinking other players' drop possible

    public Action<FightPlayer> OnItemGrant; // Handle function to apply some effect to a player
    [Serialized] protected Circle_Collider PickupTrigger;
    [Serialized] public FadeAfterStart Fade;

    private Vector2 _bump;
    private float _bumpStrength = 80f;
}