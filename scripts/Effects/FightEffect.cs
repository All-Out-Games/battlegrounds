using AO;
using StreamReader = AO.StreamReader;


/// <summary>
/// Base class derived from AEffect to get FightPlayer 
/// </summary>
public abstract class FightEffect : AEffect
{
    protected FightPlayer FightPlayer;
    

    /// <summary>
    /// Get the owner as FightPlayer & the slot the skill has been triggered from.
    /// If you want to use these fields you must call base.OnEffectStart!
    /// </summary>
    public override void OnEffectStart()
    {
        FightPlayer = (FightPlayer)Player;
    }

    public override void OnEffectEnd(bool interrupt)
    {
        
    }

    public override void OnEffectUpdate()
    {
        
    }

    /// <summary>
    /// This function handles join-in-progress stuff. When a new player joins, all existing effects will be synced to them
    /// but the OnEffectStart function won't be called! We need to do things that ensures the OnEffectEnd function will be called error-free
    /// Do you need to actually sync the effect?
    /// Probably not, most effects won't last for longer than a few seconds. Usually you just guarantee them bug-free
    /// You don't need to sync shield/heal/damage, etc. Those are handled by the server SyncVars.
    /// However, proper syncing for the longer effects (e.g. buffs) is still suggested.
    /// [i.e. use NetworkSerialize to append any extra data you need and apply them back through NetworkDeserialize]
    /// </summary>
    /// <param name="reader"></param>
    public override void NetworkDeserialize(StreamReader reader)
    {
        base.NetworkDeserialize(reader);
        FightPlayer = (FightPlayer)Player;
        //Log.Debug("FightEffect Deserialize");
    }
}