using AO;



/// <summary>
/// Base class derived from AEffect to get FightPlayer 
/// </summary>
public abstract class FightEffect : AEffect
{
    protected FightPlayer FightPlayer;
    protected string SlotKey;
    protected SkillSlot SkillSlot;

    /// <summary>
    /// Get the owner as FightPlayer & the slot the skill has been triggered from.
    /// If you want to use these fields you must call base.OnEffectStart!
    /// </summary>
    public override void OnEffectStart()
    {
        FightPlayer = (FightPlayer)Player;

        // Effects related to active skills will have the slot key passed in for cooldown process
        // We get their slot here
        if (SlotKey != null)
        {
            FightPlayerSkillSlotsManager slotsMgr = FightPlayer.GetSkillSlots();
            SkillSlot = slotsMgr.GetSkillSlot(SlotKey);
        }
        
    }
    
    public override void OnEffectUpdate()
    {
        
    }
}