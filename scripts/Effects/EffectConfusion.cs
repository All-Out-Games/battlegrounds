using AO;
namespace Assembly.scripts.Effects;


/// <summary>
/// This debuff prevents the player from using abilities and block their movement input
/// and it will force the player to move to a random direction each second
/// </summary>
public class EffectConfusion : EffectStun
{
    protected float NextDmgTick = 0;
    protected bool Ticked = false;
    public float ConfusionIntensity = 10f;
    

    public override void OnEffectUpdate()
    {
        if (Util.OneTime(ElapsedTime > NextDmgTick, ref Ticked))
        {
            Confuse();
            NextDmgTick += 1;
            Ticked = false;
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        base.OnEffectEnd(interrupt);
        FightPlayer.AddDash(Vector2.Zero, 0);
    }

    public void Confuse()
    {
        if (Network.IsServer)
        {
            // We want this random direction to be synced, so we use RPC here
            Vector2 dir = Util.RandomPositionInBox(-Vector2.One, Vector2.One, Random.Shared).Normalized;
            FightPlayer.CallClient_AddDash(dir * ConfusionIntensity, 1.25f);
        }
    }
}