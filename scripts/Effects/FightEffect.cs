using AO;
using StreamReader = AO.StreamReader;


/// <summary>
/// Base class derived from AEffect to get FightPlayer 
/// </summary>
public abstract class FightEffect : AEffect
{
    protected FightPlayer FightPlayer;
    
    protected StateMachineLayer FightLayer;
    protected StateMachineLayer MainLayer;
    protected StateMachine FightStateMachine;

    protected virtual int InterruptLevel => 0;
    // This is different from FreezePlayer. PreventMovement will only block movement input, but the player under such an effect is still susceptible to bumps & confusion.
    protected virtual bool PreventMovement => false;
    protected virtual bool PreventDamage => false;
    /// <summary>
    /// Get the owner as FightPlayer & the slot the skill has been triggered from.
    /// If you want to use these fields you must call base.OnEffectStart!
    /// </summary>
    public override void OnEffectStart()
    {
        FightPlayer = (FightPlayer)Player;
        FightStateMachine = Player.SpineAnimator.SpineInstance.StateMachine;
        MainLayer = FightStateMachine.TryGetLayerByName("main");
        FightLayer = FightStateMachine.TryGetLayerByName("fight_layer");

        if (PreventMovement)
        {
            FightPlayer.AddSpeedModifier(0);
        }

        if (PreventDamage)
        {
            FightPlayer.CollisionEntity.LocalEnabled = false;
        }
    }

    public override void OnEffectEnd(bool interrupt)
    {
        if (PreventMovement)
        {
            FightPlayer.RemoveSpeedModifier(0);
        }
        if (PreventDamage)
        {
            FightPlayer.CollisionEntity.LocalEnabled = true;
        }
    }

    public override void OnEffectUpdate()
    {
        
    }

    public virtual bool Interruptable(int incoming)
    {
        return incoming >= InterruptLevel;
    }

    /// <summary>
    /// Subscribe this function to FightPlayer.OnReceiveDamage if you need some effect to react to damage.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="info"></param>
    protected virtual void OnDamageEvent(FightPlayer source, FightPlayer.DamageInfo info)
    {
        // Default behavior: Compare interruption level
        
        if (InterruptLevel == 0 || info.InterruptLevel == 0)
        {
            // ILv = 0 means uninterruptible by any damage. You shouldn't subscribe in this case anyway
            return;
        }

        // You can override Interruptable() to use specific interruption level (e.g. only interruptable by ILv = 10001)
        // Or override this function to implement more complex behavior.
        if (Interruptable(info.InterruptLevel))
        {
            FightPlayer.RemoveEffect(this, true);
        }
        
    }

    /// <summary>
    /// Register this effect instance using FightPlayer.RegisterPreDamageEvent and this function will be called in TakeDamage
    /// You can modify the damage event received, namely remove the flinch or reduce the damage
    /// </summary>
    /// <param name="info"></param>
    public virtual void PreDamageMod(ref FightPlayer.DamageInfo info)
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
    }
}